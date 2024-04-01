using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class Casting : MonoBehaviour
{
    [SerializeField] private BoolVariable casting;
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
    private PlayerControls _playerControls;

    private void Start()
    {
        _playerControls = new PlayerControls();
        _playerControls.Game.Shoot.performed += Shoot;
    }
    
    private void Shoot(InputAction.CallbackContext callbackContext)
    {
        switch (_state)
        {
            case State.Charging:
                _state = State.Casted;
                break;
        }
    }

    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         switch (_state)
    //         {
    //             case State.Ready:
    //                 Charge().Forget();
    //                 break;
    //             case State.Charging:
    //                 Cast();
    //                 break;
    //             case State.Casted:
    //                 ResetCast();
    //                 break;
    //         }
    //     }
    // }

    public async UniTask<float> GetCastMultiplier(CancellationToken token)
    {
        return await ChargeAsync(token);
    }

    private void EnableInput()
    {
        _playerControls.Enable();
    }

    private void DisableInput()
    {
        _playerControls.Disable();
    }

    private async UniTask Charge()
    {
        float value = await ChargeAsync(this.GetCancellationTokenOnDestroy());
    }

    private async UniTask<float> ChargeAsync(CancellationToken token)
    {
        _state = State.Charging;
        EnableInput();
        casting.Value = true;
        castMultiplier.Value = 0f;
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

                await UniTask.NextFrame(token);
            }
            up = !up;
        }

        DisableInput();
        casting.Value = false;
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
