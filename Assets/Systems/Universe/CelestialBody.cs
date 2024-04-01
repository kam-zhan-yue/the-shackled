using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof (Rigidbody2D))]
public class CelestialBody : MonoBehaviour
{
    public bool clockwiseOrbit = false;
    public float orbitalSpeed = 5f;
    public BodyType bodyType;
    [HideIf("bodyType", BodyType.Center)] 
    public CelestialBody parent;
    private Rigidbody2D _rigidbody;

    private float _angle;

    private Vector2 Velocity { get; set; } = new Vector2();

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (bodyType != BodyType.Center)
        {
            SetVelocity();
            _rigidbody.MovePosition(_rigidbody.position + Velocity * Time.fixedDeltaTime);
            Debug.Log($"Distance = {GetRadius()}");
        }
    }

    private void SetVelocity()
    {
        Vector2 parentVelocity = parent.Velocity;
        Vector2 circularVelocity = GetCircularVelocity();
        Velocity = parentVelocity + circularVelocity;
    }

    private float GetRadius()
    {
        Vector2 difference = parent.transform.position - transform.position;
        return difference.magnitude;
    }

    private Vector2 GetCircularVelocity()
    {
        //Get the vector between the parent and object
        Vector2 difference = parent.transform.position - transform.position;
        // Calculate the circular velocity using the perpendicular vector
        Vector2 direction = clockwiseOrbit
            ? new Vector2(-difference.y, difference.x)
            : new Vector2(difference.y, -difference.x);
        Vector2 circularVelocity = direction.normalized * orbitalSpeed;
        return circularVelocity;
    }

    private void OnDrawGizmos()
    {
        if (parent != null)
        {
            Vector2 circularVelocity = GetCircularVelocity();
            Vector3 position = transform.position;
            Vector2 endPoint = (Vector2)position + circularVelocity;
            Gizmos.DrawLine(position, endPoint);
        }
    }
}