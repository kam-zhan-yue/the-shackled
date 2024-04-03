using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TentacleMovement : MonoBehaviour
{
    [Header("SinWave Settings")]

    private float speed = 0f;
    [SerializeField] private float amplitude = 1f;        // Amplitude of the sine wave
    [SerializeField] private float frequency = 1f;        // Frequency of the sine wave
    private Vector3 direction;
    private float startTime;            // Start time for the sine wave movement
    private float initial_y_position;   // amount to shift sin wave down
    private float initial_x_position;
    private float rotation;
    private float moveX;
    private float moveY;
    private Vector3 origin;
    [SerializeField] float tolerance = 0.01f;
    public Action ReachedOrigin;
    private Vector3 _destination = Vector3.zero;
    private float _scaleFactor = 1f;
    private CircleCollider2D _circleCollider;

    private float Frequency => frequency * 1/_scaleFactor;
    private float Amplitude => amplitude * _scaleFactor;

    private FishingPole _fishingPole;
    public Action<IHookable> OnHook;
    public Action<IHookable> OnLetGo;
    public Action<IHookable> OnReel;
    public Action<IHookable> OnAutoHook;

    private bool _paused = false;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _fishingPole = GetComponent<FishingPole>();
    }

    private void Start()
    {
        _fishingPole.OnHook += Hook;
        _fishingPole.OnLetGo += LetGo;
        _fishingPole.OnReel += Reel;
        _fishingPole.OnAutoHook += AutoHook;
    }

    private void Hook(IHookable hookable)
    {
        OnHook?.Invoke(hookable);
    }

    private void LetGo(IHookable hookable)
    {
        OnLetGo?.Invoke(hookable);
    }

    private void Reel(IHookable hookable)
    {
        OnReel?.Invoke(hookable);
    }

    private void AutoHook(IHookable hookable)
    {
        Debug.Log("TentacleMovement AutoHook");
        OnAutoHook?.Invoke(hookable);
    }

    public void Initialize_Point(Vector3 _direction, float _speed, float width, Vector3 destination = default)
    {
        _circleCollider.radius = width;
        _destination = destination;
        origin = transform.position;
        direction = _direction;
        speed = _speed;
        SetInitialDirection();
    }

    public void TogglePause(bool pause)
    {
        _paused = true;
    }

    public void SetCanGame(bool canGame)
    {
        _fishingPole.SetCanGame(canGame);
    }

    public void Scale(float scaleFactor)
    {
        _scaleFactor = scaleFactor;
    }

    public void SetInitialDirection()
    {
        startTime = Time.time; 
        initial_y_position=transform.position.y;
        initial_x_position=transform.position.x;
        rotation=Mathf.Atan2(direction.y, direction.x);
    }

    public async UniTask ReachedDestinationAsync(CancellationToken token)
    {
        float distance = Vector3.Distance(_destination, transform.position);
        float timeToDestination = distance / speed;

        float timer = 0f;

        while (timer < timeToDestination)
        {
            if (!_paused)
            {
                timer += Time.deltaTime;
            }
            await UniTask.NextFrame(token);
        }
    }

    //if not placed at origin it wont work
    //need to fix the shifting of the origin in the rotation
    private void FixedUpdate()
    {
        // CheckReachedOrigin();
        moveX =Time.deltaTime*speed;
        Vector3 unrotated_position = RotatePoint(transform.position, (-1)*rotation);

        // calculate the Y value based on time
        moveY = (Mathf.Sin((unrotated_position.x) * Frequency * Mathf.PI) * Amplitude);
        
        transform.position=RotatePoint((new Vector3(unrotated_position.x+moveX,moveY,0)), rotation);
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    
    public Vector3 RotatePoint(Vector3 _point, float _rotation)
    {
        // Calculate sine and cosine of the angle
        float cosTheta = Mathf.Cos(_rotation);
        float sinTheta = Mathf.Sin(_rotation);

        // Perform rotation using the rotation matrix
        float newX = _point.x * cosTheta - _point.y * sinTheta;
        float newY = _point.x * sinTheta + _point.y * cosTheta;

        // Return the rotated point
        return new Vector3(newX, newY, 0);
    }

    public async UniTask ReturnAsync(CancellationToken token)
    {
        float distance = Vector3.Distance(transform.position, origin);
        float timeToDestination = distance / speed;
        await UniTask.WaitForSeconds(timeToDestination, cancellationToken: token);
        
        speed = 0;
        gameObject.SetActive(false);
        transform.position = origin;
        ReachedOrigin?.Invoke();
    }

    public void CheckReachedOrigin()
    {
        // Calculate the distance between the current position and the origin
        float distanceToOrigin = Vector3.Distance(transform.position, origin);
        
        // Define a tolerance threshold for considering the tentacle to have reached the origin

        // If we have a negative speed and the tentacle is close enough to the origin, deactivate it
        if (speed < 0 && distanceToOrigin < tolerance)
        {
            speed = 0;
            //Calculating when it will reach
            // gameObject.SetActive(false);
            transform.position = origin;
            ReachedOrigin?.Invoke();
        }
    }

    private void OnDestroy()
    {
        if (_fishingPole)
        {
            _fishingPole.OnHook -= Hook;
            _fishingPole.OnReel -= Reel;
            _fishingPole.OnLetGo -= LetGo;
            _fishingPole.OnAutoHook -= AutoHook;
        }
    }
}
