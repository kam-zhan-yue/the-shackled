using System;
using System.Collections.Generic;
using DG.Tweening;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public abstract class CelestialBody : MonoBehaviour, IHookable
{
    [SerializeField] private bool clockwiseOrbit = false;
    [SerializeField, Range(0f, 360f)] private float startAngle = 0f;
    [SerializeField] private float orbitalPeriod = 1f;
    [SerializeField, Range(1f, 10f)] private float orbitalRadius = 1f;
    protected Transform parent;
    private Rigidbody2D _rigidbody;

    private State _state = State.Orbiting;

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

    private void Start()
    {
        _angle = startAngle;
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
        Vector2 rotation = UniverseHelper.ConvertAngleToRotation(startAngle);
        return parentPos + (Vector3)rotation.normalized * orbitalRadius;
    }

    private void FixedUpdate()
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
        float angleStep = UniverseHelper.GetAngleStep(Time.fixedDeltaTime, orbitalPeriod);
        if (clockwiseOrbit)
            _angle -= angleStep;
        else
            _angle += angleStep;
        
        Vector2 rotation = UniverseHelper.ConvertAngleToRotation(_angle);
        Vector2 position = (Vector2)parent.transform.position + rotation * orbitalRadius;
        _rigidbody.MovePosition(position);
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

    public virtual CelestialData Absorb()
    {
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            Destroy(gameObject);
        });
        return new CelestialData();
    }
}