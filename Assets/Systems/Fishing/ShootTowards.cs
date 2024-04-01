using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShootTowards : MonoBehaviour
{
    [SerializeField] private bool debug;
    [SerializeField] private float move_speed;
    [SerializeField] private float retract_speed;
    private bool reached_destination = false;
    private Vector3 origin_position;
    public Vector3 Origin => origin_position;
    
    private Tentacle_State current_state;
    public Tentacle_State Current_State => current_state;

    private void Awake() 
    {
        origin_position = transform.position;
    }

    private void Update()
    {
        if (debug && current_state == Tentacle_State.Ready && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 cursor_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursor_position.z = transform.position.z; // Ensure z position is the same as the object's
            Shoot(cursor_position, this.GetCancellationTokenOnDestroy()).Forget();
        }
    }

    public async UniTask Shoot(Vector3 target, float multiplier, CancellationToken token)
    {
        Vector3 position = transform.position;
        Vector3 adjustedTarget = new Vector3(target.x, target.y, position.z);
        Vector3 difference = adjustedTarget - position;
        Vector3 finalPoint = difference * multiplier;
        await Shoot(finalPoint, token);
    }

    private async UniTask Shoot(Vector3 destination, CancellationToken token)
    {
        current_state = Tentacle_State.Shooting;
        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, move_speed * Time.deltaTime);
            await UniTask.NextFrame(token);
        }
        current_state = Tentacle_State.Grabbed;
        await Retract(token);
    }

    private async UniTask Retract(CancellationToken token)
    {
        current_state = Tentacle_State.Retracting;
        while (transform.position != origin_position)
        {
            transform.position = Vector3.MoveTowards(transform.position, origin_position, retract_speed * Time.deltaTime);
            await UniTask.NextFrame(token);
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
