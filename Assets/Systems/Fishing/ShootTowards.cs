using System.Collections;
using UnityEngine;

public class ShootTowards : MonoBehaviour
{
    [SerializeField] private float move_speed;
    [SerializeField] private float retract_speed;
    private bool is_shooting = false;
    private bool reached_destination = false;
    private Vector3 origin_position;
    private Tentacle_State current_state;

    private void Awake() 
    {
        origin_position = transform.position;
    }

    private void Update()
    {
        switch (current_state)
        {
            case Tentacle_State.Ready:
                 if (!is_shooting && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Vector3 cursor_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        cursor_position.z = transform.position.z; // Ensure z position is the same as the object's
                        StartCoroutine(Shoot(cursor_position));
                        current_state = Tentacle_State.Shooting;
                    }
                break;
            case Tentacle_State.Shooting:
                break;
            case Tentacle_State.Grabbed:
                StartCoroutine(Retract());
                current_state = Tentacle_State.Retracting;
                break;


        }
    }

    private IEnumerator Shoot(Vector3 destination)
    {
        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, move_speed * Time.deltaTime);
            yield return null;
        }
        current_state = Tentacle_State.Grabbed;
    }

    private IEnumerator Retract()
    {
        while (transform.position != origin_position)
        {
            transform.position = Vector3.MoveTowards(transform.position, origin_position, retract_speed * Time.deltaTime);
            yield return null;
        }
        current_state = Tentacle_State.Ready;
    }
}

public enum Tentacle_State
{
    Ready,
    Shooting,
    Grabbed,
    Retracting

}
