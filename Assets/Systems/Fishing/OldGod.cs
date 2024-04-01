using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class OldGod : MonoBehaviour
{
    public Casting casting;
    public ShootTowards shootTowards;
    private State _state;
    private Camera _main;
    private PlayerControls _playerControls;

    private enum State
    {
        Idle = 0,
        Casting = 1,
        Shooting = 2
    }

    private void Start()
    {
        _main = Camera.main;
        _playerControls = new PlayerControls();
        _playerControls.Game.Shoot.performed += Shoot;
        _playerControls.Enable();
    }

    private void Shoot(InputAction.CallbackContext callbackContext)
    {
        switch (_state)
        {
            case State.Idle:
                Cast().Forget();
                break;
            case State.Casting:
                break;
            case State.Shooting:
                break;
        }
    }
    
    private async UniTask Cast()
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        _state = State.Casting;
        Vector3 targetPosition = _main.ScreenToWorldPoint(Input.mousePosition);
        float castMultiplier = await casting.GetCastMultiplier(token);
        
        _state = State.Shooting;
        await shootTowards.Shoot(targetPosition, castMultiplier, token);

        _state = State.Idle;
    }

    private void OnDestroy()
    {
        _playerControls.Dispose();
    }
}
