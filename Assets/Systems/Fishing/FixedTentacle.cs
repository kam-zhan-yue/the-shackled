using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedTentacle : MonoBehaviour
{
    [SerializeField] private int length;
    [SerializeField] private LineRenderer line_renderer;
    [SerializeField] private Vector3[] segment_positions;
    [SerializeField] private Vector3[] segment_velocity;

    //shootTowards has the current states of the movement
    [SerializeField] private ShootTowards tentacle_movement_data;

    [SerializeField] private Transform target_direction;
    //this stops the points from bunching by maintaing this distance
    [SerializeField] private float target_distance;
    [SerializeField] private float smooth_speed;
    [Header("Wiggle Settings")]
    [SerializeField] private float wiggle_speed;
    [SerializeField] private float wiggle_magnitude;
    [SerializeField] private Transform wiggle_direction;
    [SerializeField] private bool retracting;



    void Awake()
    {
        line_renderer = GetComponent<LineRenderer>();
        line_renderer.positionCount = length;
        segment_positions = new Vector3[length];
        segment_velocity = new Vector3[length];
        //ResetPositions();

    }

    void Update()
    {
        switch (tentacle_movement_data.Current_State)
        {
            case Tentacle_State.Ready:
                break;

            case Tentacle_State.Shooting:
                //moves the wiggle direction gameobject that parents the target direction
                wiggle_direction.localRotation = Quaternion.Euler(0,0,Mathf.Sin(Time.time*wiggle_speed)*wiggle_magnitude);

                //set the first position ie the head of the tentacle to the targetdirections position
                segment_positions[0] = target_direction.position;
                //move each point to be where the next one is
                //only loop until one before the end
                for (int i = 1; i<segment_positions.Length-1; i++)
                {
                    Vector3 targetPos = segment_positions[i-1] + (segment_positions[i]-segment_positions[i-1]).normalized*target_distance;
                    segment_positions[i] = Vector3.SmoothDamp(segment_positions[i], targetPos,ref segment_velocity[i],smooth_speed);
                    //segment_positions[i] = Vector3.SmoothDamp(segment_positions[i], segment_positions[i-1]+target_direction.right*target_distance, ref segment_velocity[i], smooth_speed);
                }
                //set the final point to be stuck at the origin
                segment_positions[length-1] = tentacle_movement_data.Origin;
                //sets the linerenderes positions equal to the array of points
                line_renderer.SetPositions(segment_positions);
                break;
            
            case Tentacle_State.Grabbed:
                break;

            case Tentacle_State.Retracting:
                segment_positions[length-1] = tentacle_movement_data.Origin;
                //move each point to be where the next one is
                //only loop until one before the end
                for (int i = length-2; i>=0; i--)
                {
                    segment_positions[i] = Vector3.SmoothDamp(segment_positions[i], segment_positions[i+1], ref segment_velocity[i], smooth_speed);
                }
                
                line_renderer.SetPositions(segment_positions);
                break;
        }
        
    }

    //called in awake to add some distance between the points
    
    public void ResetPositions()
    {
        segment_positions[0] = target_direction.position;
        //its minus 1 as the last position needs to be set to be the origins position
        for (int i = 1; i<length-1; i++)
        {
            //adds a little distance between them
            segment_positions[i] = segment_positions[i-1] + target_direction.right*target_distance;
        }
        //we set the final position equal to the origin
        segment_positions[length-1] = tentacle_movement_data.Origin;

        line_renderer.SetPositions(segment_positions);
    }
}
