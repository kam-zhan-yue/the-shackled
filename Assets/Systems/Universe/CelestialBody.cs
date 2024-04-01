using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class CelestialBody : MonoBehaviour, IHookable
{
    [SerializeField] private bool clockwiseOrbit = false;
    [SerializeField, Range(0f, 360f)] private float startAngle = 0f;
    [SerializeField] private float orbitalPeriod = 1f;
    [SerializeField, Range(1f, 5f)] private float orbitalRadius = 1f;
    [SerializeField] private BodyType bodyType;
    [HideIf("bodyType", BodyType.Center)] 
    [SerializeField] private Transform parent;
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
        InitialiseDistance();
        _angle = startAngle;
    }

    private void InitialiseDistance()
    {
        transform.position = GetStartPosition();
    }
    
    private Vector2 GetStartPosition()
    {
        Vector2 rotation = UniverseHelper.ConvertAngleToRotation(startAngle);
        Vector3 parentPos = parent.transform.position;
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
        if (bodyType != BodyType.Center)
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
    }

    private void OnValidate()
    {
        transform.position = GetStartPosition();
    }

    private void OnDrawGizmos()
    {
        if (parent != null)
        {
            Gizmos.DrawSphere(GetStartPosition(), 0.1f);
        }
    }

    public void Hook(Transform pole)
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
}