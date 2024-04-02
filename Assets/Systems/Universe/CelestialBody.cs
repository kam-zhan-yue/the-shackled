using System;
using System.Collections.Generic;
using DG.Tweening;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent (typeof (Rigidbody2D))]
public abstract class CelestialBody : MonoBehaviour, IHookable
{
    [SerializeField, HideLabel] private OrbitalData orbitalData;
    protected Transform parent;
    private Rigidbody2D _rigidbody;

    private State _state = State.Orbiting;
    private CelestialData _data = new CelestialData(0.5f);
    public float Radius => transform.localScale.magnitude;
    public Action<CelestialBody> onAbsorb;

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
    }

    public void SetClockwise(bool clockwise)
    {
        orbitalData.SetClockwise(clockwise);
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = scale;
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
        SetParent();
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
        float angleStep = UniverseHelper.GetAngleStep(Time.fixedDeltaTime, orbitalData.OrbitalPeriod);
        if (orbitalData.ClockwiseOrbit)
            _angle -= angleStep;
        else
            _angle += angleStep;
        
        Vector2 rotation = UniverseHelper.ConvertAngleToRotation(_angle);
        if (parent == null)
        {
            Debug.Log($"ERROR {gameObject.name}");
        }
        else
        {
            Vector2 position = (Vector2)parent.transform.position + rotation * orbitalData.OrbitalRadius;
            _rigidbody.MovePosition(position);
        }
    }

    public virtual void Hook(Transform pole)
    {
        Debug.Log($"{gameObject.name} is hooked to {pole.name} !");
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
        onAbsorb?.Invoke(this);
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        return _data;
    }

    public void ForceMove()
    {
        Vector3 position = ServiceLocator.Instance.Get<IUniverseService>().GetCentre().position;
        transform.DOMove(position, 0.1f).SetEase(Ease.OutQuart);
    }

    private void OnDrawGizmosSelected()
    {
        if(parent != null)
            Gizmos.DrawWireSphere(parent.transform.position, orbitalData.OrbitalRadius);
    }
}