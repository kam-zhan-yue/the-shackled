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
    public float orbitalSpeed = 200f;
    [Range(1f, 5f)]
    public float orbitalRadius = 1f;
    public BodyType bodyType;
    [HideIf("bodyType", BodyType.Center)] 
    public Transform parent;
    private Rigidbody2D _rigidbody;

    private float _angle;

    private Vector2 Velocity { get; set; } = new Vector2();

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

    // Function to convert angle (in degrees) to rotation vector (x, y)
    private Vector2 ConvertAngleToRotation(float angleDegrees)
    {
        // Convert angle from degrees to radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calculate x and y components of the rotation vector
        float x = Mathf.Cos(angleRadians);
        float y = Mathf.Sin(angleRadians);

        return new Vector2(x, y);
    }
    
    private Vector2 GetStartPosition()
    {
        Vector2 rotation = ConvertAngleToRotation(startAngle);
        Vector3 parentPos = parent.transform.position;
        return parentPos + (Vector3)rotation.normalized * orbitalRadius;
    }

    private float GetAngleStep(float deltaTime)
    {
        float angleStep = 360 / orbitalPeriod;
        return angleStep * deltaTime;
    }

    private void FixedUpdate()
    {
        if (bodyType != BodyType.Center)
        {
            _angle += GetAngleStep(Time.fixedDeltaTime);

            Vector2 rotation = ConvertAngleToRotation(_angle);
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