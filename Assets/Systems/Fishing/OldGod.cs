using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Schema;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class OldGod : MonoBehaviour
{
    [BoxGroup("Scriptable Objects"), SerializeField] private FloatVariable scale;
    [BoxGroup("Components"), SerializeField] private Casting casting;
    [BoxGroup("Components"), SerializeField] private ShootTowards shootTowards;
    private State _state;
    private Camera _main;
    private PlayerControls _playerControls;
    private float _scaleFactor = 1f;

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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out IHookable hookable))
        {
            CelestialData celestialData = hookable.Absorb();
            Absorb(celestialData);
        }
    }

    private void Absorb(CelestialData data)
    {
        _scaleFactor += data.scaleFactor;
        Vector3 newScale = Vector3.one * _scaleFactor;
        transform.DOScale(newScale, 0.2f).SetEase(Ease.OutQuart);
    }

    private void Shoot(InputAction.CallbackContext callbackContext)
    {
        if (_state == State.Idle)
        {
            Cast().Forget();
        }
    }

    [Button]
    public void ZoomOut()
    {
        float originalSize = _main.orthographicSize;
        float newSize = _scaleFactor * UniverseHelper.CAMERA_STEP;
        DOTween.To(SetCameraSize, originalSize, newSize, 0.5f).SetEase(Ease.OutQuart);
    }

    private void SetCameraSize(float size)
    {
        _main.orthographicSize = size;
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
