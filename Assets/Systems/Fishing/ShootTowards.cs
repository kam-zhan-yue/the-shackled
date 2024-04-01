using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShootTowards : MonoBehaviour
{
    [SerializeField] private bool debug;
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

    public async UniTask Shoot(Vector3 target, float multiplier, CancellationToken token)
    {
        Vector3 position = transform.position;
        Vector3 adjustedTarget = new Vector3(target.x, target.y, position.z);
        Vector3 difference = adjustedTarget - position;
        Vector3 finalPoint = difference * multiplier;
        await Shoot(finalPoint, token);
    }

    private void Update()
    {
        switch (current_state)
        {
            case Tentacle_State.Ready:
                if (debug && !is_shooting && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Vector3 cursor_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        cursor_position.z = transform.position.z; // Ensure z position is the same as the object's
                        Shoot(cursor_position, this.GetCancellationTokenOnDestroy()).Forget();
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

    private async UniTask Shoot(Vector3 destination, CancellationToken token)
    {
        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, move_speed * Time.deltaTime);
            await UniTask.NextFrame(token);
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
