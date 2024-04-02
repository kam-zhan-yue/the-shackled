using System.Collections;
using System.Collections.Generic;
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
    
    
    public void Initialize_Point(Vector3 _direction, float _speed)
    {
        origin = transform.position;
        direction = _direction;
        speed = _speed;
        SetInitialDirection();
    }
    public void SetInitialDirection() 
    {
        startTime = Time.time; 
        initial_y_position=transform.position.y;
        initial_x_position=transform.position.x;
        rotation=Mathf.Atan2(direction.y, direction.x);
    }

    //if not placed at origin it wont work
    //need to fix the shifting of the origin in the rotation
    void FixedUpdate()
    {
        CheckReachedOrigin();
        moveX =Time.deltaTime*speed;
        Vector3 unrotated_position = RotatePoint(transform.position, (-1)*rotation);

        // calculate the Y value based on time
        moveY = (Mathf.Sin((unrotated_position.x) * frequency * Mathf.PI) * amplitude);
        
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

    public void CheckReachedOrigin()
{
    // Calculate the distance between the current position and the origin
    float distanceToOrigin = Vector3.Distance(transform.position, origin);

    // Define a tolerance threshold for considering the tentacle to have reached the origin

    // If we have a negative speed and the tentacle is close enough to the origin, deactivate it
    if (speed < 0 && distanceToOrigin < tolerance)
    {
        speed = 0;
        gameObject.SetActive(false);
        transform.position = origin;
    }
}

}
