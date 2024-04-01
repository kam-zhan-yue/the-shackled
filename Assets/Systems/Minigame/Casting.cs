using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class Casting : MonoBehaviour
{
    [SerializeField] private FloatVariable castMultiplier;
    [SerializeField] private float chargeTime = 1f;
    private float _chargeTimer = 0f;

    private enum State
    {
        Ready = 0,
        Charging = 1,
        Casted = 2,
    }

    private State _state;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (_state)
            {
                case State.Ready:
                    Charge().Forget();
                    break;
                case State.Charging:
                    Cast();
                    break;
                case State.Casted:
                    ResetCast();
                    break;
            }
        }
    }

    private async UniTask Charge()
    {
        float value = await ChargeAsync(this.GetCancellationTokenOnDestroy());
    }

    private async UniTask<float> ChargeAsync(CancellationToken token)
    {
        castMultiplier.Value = 0f;
        _state = State.Charging;
        bool up = true;
        while (_state == State.Charging)
        {
            _chargeTimer = chargeTime;
            while (_chargeTimer > 0f && _state == State.Charging)
            {
                _chargeTimer -= Time.deltaTime;
                if (up)
                {
                    castMultiplier.Value += 1 / chargeTime * Time.deltaTime;
                }
                else
                {
                    castMultiplier.Value -= 1 / chargeTime * Time.deltaTime;
                }

                await UniTask.NextFrame(cancellationToken:token);
            }
            up = !up;
        }

        return castMultiplier.Value;
    }
    
    private void Cast()
    {
        _state = State.Casted;
    }

    private void ResetCast()
    {
        _state = State.Ready;
    }
}
