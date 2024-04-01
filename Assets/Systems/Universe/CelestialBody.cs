using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
public class CelestialBody : MonoBehaviour
{
    public bool clockwiseOrbit = false;
    [Range(0f, 360f)] public float startAngle = 0f;
    public float orbitalPeriod = 1f;
    [Range(1f, 5f)]
    public float orbitalRadius = 1f;
    public BodyType bodyType;
    [HideIf("bodyType", BodyType.Center)] 
    public Transform parent;
    private Rigidbody2D _rigidbody;

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
}