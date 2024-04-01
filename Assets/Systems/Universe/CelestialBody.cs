using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
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

    private Vector2 Velocity { get; set; } = new Vector2();

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        InitialiseDistance();
    }

    private void InitialiseDistance()
    {
        transform.position = GetStartPosition();
    }

    // Function to convert angle (in degrees) to rotation vector (x, y)
    public Vector2 ConvertAngleToRotation(float angleDegrees)
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
        Vector2 direction = transform.position - parentPos;
        return parentPos + (Vector3)rotation.normalized * orbitalRadius;
    }

    private void FixedUpdate()
    {
        if (bodyType != BodyType.Center)
        {
            SetVelocity();
            _rigidbody.MovePosition(_rigidbody.position + Velocity * Time.fixedDeltaTime);
        }
    }

    private void SetVelocity()
    {
        // Vector2 parentVelocity = parent.Velocity;
        Vector2 circularVelocity = GetCircularVelocity();
        Velocity =  circularVelocity;
    }

    private Vector2 GetCircularVelocity()
    {
        //Get the vector between the parent and object
        Vector2 difference = parent.transform.position - transform.position;
        // Calculate the circular velocity using the perpendicular vector
        Vector2 direction = clockwiseOrbit
            ? new Vector2(-difference.y, difference.x)
            : new Vector2(difference.y, -difference.x);
        float circularSpeed = 2 * Mathf.PI * orbitalRadius / orbitalPeriod;
        Vector2 circularVelocity = direction.normalized * circularSpeed;
        return circularVelocity;
    }

    private void OnValidate()
    {
        transform.position = GetStartPosition();
    }

    private void OnDrawGizmos()
    {
        if (parent != null)
        {
            Vector2 circularVelocity = GetCircularVelocity();
            Vector3 position = transform.position;
            Vector2 endPoint = (Vector2)position + circularVelocity;
            Gizmos.DrawLine(position, endPoint);
            
            Gizmos.DrawSphere(GetStartPosition(), 0.1f);
        }
    }
}