using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent (typeof (Rigidbody2D))]
public abstract class CelestialBody : MonoBehaviour, IHookable
{
    public UnityEvent OnHook;
    [SerializeField] private Vector2 fishingSlots = Vector2.zero;
    [SerializeField] private float sizeMultiplier = 1f;
    [SerializeField, HideLabel] private OrbitalData orbitalData;
    protected Transform parent;

    public float Radius => transform.localScale.magnitude * sizeMultiplier;
    public Action<CelestialBody> onAbsorb;
    
    //Components
    private Rigidbody2D _rigidbody;
    private LineRenderer _lineRenderer;
    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;
    
    //Private Variables
    private State _state = State.Orbiting;
    private CelestialData _data = new CelestialData(0.5f);
    private int _subdivisions = 100;
    private bool parentSet = false;

    private enum State
    {
        Orbiting = 0,
        Hooked = 1,
        Reeling = 2,
    }

    private float _angle;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _lineRenderer = GetComponent<LineRenderer>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.name = $"{gameObject.name} {gameObject.GetInstanceID()}";
        
        SetParent();
        parentSet = true;
    }

    public void SetClockwise(bool clockwise)
    {
        orbitalData.SetClockwise(clockwise);
    }

    public void SetScale(float scaleFactor)
    {
        transform.localScale = Vector3.one * scaleFactor / sizeMultiplier;
    }

    public void SetStartingAngle(float angle)
    {
        orbitalData.SetStartingAngle(angle);
    }

    public void SetOrbitalRadius(float radius)
    {
        orbitalData.SetOrbitalRadius(radius);
    }

    public void SetOrbitalPeriod(float period)
    {
        orbitalData.SetOrbitalPeriod(period);
    }

    public void RandomInit(float maxPeriod, float maxRadius)
    {
        orbitalData.Randomise(maxPeriod, maxRadius);
    }

    private void Start()
    {
        _angle = orbitalData.StartAngle;
        InitialiseDistance();
    }

    protected abstract void SetParent();

    private void InitialiseDistance()
    {
        transform.position = GetStartPosition(parent.position);
    }
    
    protected Vector2 GetStartPosition(Vector3 parentPos)
    {
        Vector2 rotation = UniverseHelper.ConvertAngleToRotation(orbitalData.StartAngle);
        return parentPos + (Vector3)rotation.normalized * orbitalData.OrbitalRadius;
    }

    private void FixedUpdate()
    {
        // Simulate();
    }

    private void Update()
    {
        if(_state == State.Orbiting)
            DrawOrbit();
    }

    public void Simulate()
    {
        switch (_state)
        {
            case State.Orbiting:
                Orbit();
                break;
            case State.Hooked:
                break;
            case State.Reeling:
                break;
        }
    }

    private void Orbit()
    {
        // Calculate the angle step and add it to the angle depending on clockwise orbit
        float angleStep = UniverseHelper.GetAngleStep(Time.fixedDeltaTime, orbitalData.OrbitalPeriod);
        _angle = orbitalData.ClockwiseOrbit ? _angle - angleStep : _angle + angleStep;
        // Reset the angle after a full rotation to prevent overflow
        if (_angle > 360f) _angle = 0f;
        if (_angle < -360f) _angle = 0f;
        // Convert the angle to rotation and set the position of the rigidbody
        Vector2 rotation = UniverseHelper.ConvertAngleToRotation(_angle);
        Vector2 position = (Vector2)parent.transform.position + rotation * orbitalData.OrbitalRadius;
        _rigidbody.MovePosition(position);
    }

    public bool HasGame(out int slots)
    {
        if (fishingSlots == Vector2.zero)
        {
            slots = 0;
            return false;
        }

        slots = Mathf.CeilToInt(Random.Range(fishingSlots.x, fishingSlots.y));
        return true;
    }

    public virtual void Hook(Transform pole)
    {
        OnHook?.Invoke();
        CinemachineShake.Instance.ShakeCamera(sizeMultiplier,0.2f);
        _lineRenderer.positionCount = 0;
        _state = State.Hooked;
        transform.parent = pole;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.velocity = Vector2.zero;
    }

    public void Reel(Transform pole)
    {
        
    }

    public void SetData(CelestialData data)
    {
        _data = data;
    }

    public virtual CelestialData Absorb()
    {
        _circleCollider.enabled = false;
        onAbsorb?.Invoke(this);
        ForceMove();
        transform.DOScale(Vector3.zero, 2f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            _spriteRenderer.enabled = false;
            DestroyAsync(this.GetCancellationTokenOnDestroy()).Forget();
        });
        return _data;
    }

    private void ForceMove()
    {
        Vector3 position = ServiceLocator.Instance.Get<IUniverseService>().GetCentre().position;
        transform.DOMove(position, 2f).SetEase(Ease.OutQuart);
    }

    private void DrawOrbit()
    {
        float angleStep = 2f * Mathf.PI / _subdivisions;
        _lineRenderer.positionCount = _subdivisions;
        Vector3 offset = parent.transform.position;
        for (int i = 0; i < _subdivisions; ++i)
        {
            float xPosition = orbitalData.OrbitalRadius * Mathf.Cos(angleStep * i);
            float yPosition = orbitalData.OrbitalRadius * Mathf.Sin(angleStep * i);
            Vector3 pointPosition = new Vector3(xPosition, yPosition, 0f);
            Vector3 finalPosition = pointPosition + offset;
            _lineRenderer.SetPosition(i, finalPosition);
        }
    }

    public async UniTask Lerp(float orbitalRadius, float scaleFactor, float duration)
    {
        DOTween.To(SetOrbitalRadius, orbitalData.OrbitalRadius, orbitalRadius, duration).SetEase(Ease.OutExpo);
        DOTween.To(SetScale, transform.localScale.magnitude, scaleFactor, duration).SetEase(Ease.OutExpo);
        await UniTask.WaitForSeconds(duration, cancellationToken: this.GetCancellationTokenOnDestroy());
    }

    private async UniTask DestroyAsync(CancellationToken token)
    {
        await UniTask.WaitForSeconds(5f, cancellationToken:token);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if(parent != null)
            Gizmos.DrawWireSphere(parent.transform.position, orbitalData.OrbitalRadius);
    }
}