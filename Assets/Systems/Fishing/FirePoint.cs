using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirePoint : MonoBehaviour
{
    [SerializeField] private float destination_tolerance_check;
    [SerializeField] private TentacleMovement point;
    [SerializeField] private List<TentacleMovement> point_list = new List<TentacleMovement>();
    [SerializeField] private Vector3[] point_positions_array;
    [SerializeField] private float speed;
    [SerializeField] private int num_of_points;
    [SerializeField] private float delay_between_firing_points;
    [SerializeField] private LineRenderer line_renderer;
    [Header("Events")]
    [SerializeField] private UnityEvent OnPointsAllFired;
    [SerializeField] private UnityEvent OnReachedDestination;
    private Vector3 cursor_position;
    [Header("Lerp Values")]
    [SerializeField] private float duration;



    private void Awake() 
    {
        point_positions_array = new Vector3[num_of_points];
        Initialize_Array();
    }

    public void Initialize_Array()
    {
        for (int i=0; i<num_of_points; i++)
        {

            Vector3 direction = (cursor_position - transform.position).normalized;
            // Instantiate a TentacleMovement object at the fire point's position
            TentacleMovement _point = Instantiate(point, transform.position, Quaternion.identity);
            // Initialize the TentacleMovement object
            // set its speed to zero and set it to inactive
            _point.transform.parent = gameObject.transform;
            _point.Initialize_Point(direction, 0);
            _point.gameObject.SetActive(false);
            //add to list
            point_list.Add(_point);
        }
    }

    void Update()
    {
        // Check if the left mouse button is pressed down
        if (Input.GetMouseButtonDown(0))
        {
            // Get the cursor position in world space
            cursor_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursor_position.z = 0;
            StartCoroutine(DelayBetweenShots(delay_between_firing_points));
        }
        UpdatePositionsArray();
        //set the number of vertices and then assign the points
        line_renderer.positionCount = num_of_points;
        line_renderer.SetPositions(point_positions_array);
    }

    public void SetSpeeds(float _speed)
    {
        foreach (TentacleMovement _point in point_list)
        {
            _point.SetSpeed(_speed);
        }
    }

    public void LerpTheValues()
    {
        StartCoroutine(LerpFloat(speed,speed*(-1), duration));
    }

    //we will lerp the slow down at the end
    public IEnumerator LerpFloat(float startValue, float endValue, float _duration)
    {
        float currentTime = 0f;

        while (currentTime < _duration)
        {
            // Increment current time using deltaTime from WaitForSeconds
            currentTime += Time.deltaTime;

            // Calculate t value between 0 and 1 based on current time and duration
            float t = currentTime / _duration;

            // Perform the lerp between start and end values
            float lerpedValue = Mathf.Lerp(startValue, endValue, t);

            SetSpeeds(lerpedValue);


            yield return null;
        }

        
    }


    IEnumerator DelayBetweenShots(float delay_between_firing_points)
    {
        for (int i = 0; i < num_of_points; i++)
        {
            //if its true we no longer need to fire points
            if (CheckDestination())
            {
                yield break;
            }

            ShootPoint(speed, i);
            yield return new WaitForSeconds(delay_between_firing_points);
            //line_renderer.SetPositions(point_list);
        }
        OnPointsAllFired?.Invoke();
    }

    public void ShootPoint(float speed, int index)
    {
        
        Vector3 direction = (cursor_position - transform.position).normalized;

        // Initialize the object that is in this point in the index and set it to active
        point_list[index].Initialize_Point(direction, speed);
        point_list[index].gameObject.SetActive(true);
        

    }

    public void UpdatePositionsArray()
    {
        int count = 0;

        foreach (TentacleMovement point in point_list )
        {
            /*
            if (point_positions_array[count] == Vector3.zero)
            {
                Debug.Log("Empty point");
                return;
            */
            point_positions_array[count] = point.transform.position;
            count++;
        }
    }

    public bool CheckDestination()
    {
        float distanceToDestination = Vector3.Distance(point_list[0].transform.position, cursor_position);
        //Debug.Log(distanceToDestination);
        if (distanceToDestination<destination_tolerance_check)
        {
            OnReachedDestination?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
}
