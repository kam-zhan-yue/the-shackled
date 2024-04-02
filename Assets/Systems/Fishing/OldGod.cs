using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class OldGod : MonoBehaviour, IGodService
{
    public UnityEvent OnAbsorb;
    [BoxGroup("Scriptable Objects"), SerializeField] private GameSettings gameSettings;
    [BoxGroup("Components"), SerializeField] private Casting casting;
    [BoxGroup("Components"), SerializeField] private ShootTowards shootTowards;
    [BoxGroup("Components"), SerializeField] private FirePoint firePoint;
    private State _state;
    private Camera _main;
    private PlayerControls _playerControls;
    private float _scaleFactor = 1f;
    private int _threshold = 1;

    private enum State
    {
        Idle = 0,
        Casting = 1,
        Shooting = 2
    }

    private void Awake()
    {
        ServiceLocator.Instance.Register<IGodService>(this);
    }

    private void Start()
    {
        StartAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask StartAsync(CancellationToken token)
    {
        _main = Camera.main;
        if (gameSettings.showIntroAnimation)
        {
            SetCameraSize(gameSettings.zoomOut);
            await UniTask.WaitForSeconds(1f, cancellationToken:token);
            DOTween.To(SetCameraSize, gameSettings.zoomOut, UniverseHelper.START_CAMERA, 1.2f).SetEase(Ease.InOutQuint).OnComplete(Init);
        }
        else
        {
            Init();
        }
    }

    private void Init()
    {
        _playerControls = new PlayerControls();
        _playerControls.Game.Shoot.performed += Shoot;
        _playerControls.Enable();
        ServiceLocator.Instance.Get<IUniverseService>().StartSimulation();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out IHookable hookable))
        {
            CelestialData celestialData = hookable.Absorb();
            Absorb(celestialData);
        }
    }

    public void Absorb(CelestialData data)
    {
        OnAbsorb?.Invoke();
        _scaleFactor += data.food;
        _scaleFactor = Mathf.Clamp(_scaleFactor, 0f, gameSettings.maxScale);
        Vector3 newScale = Vector3.one * _scaleFactor;
        transform.DOScale(newScale, 0.2f).SetEase(Ease.OutQuart);
    }

    public void ResetScale()
    {
        _scaleFactor = gameSettings.endlessScale;
        transform.DOScale(_scaleFactor, 0.2f).SetEase(Ease.OutQuart);
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
        float newSize = UniverseHelper.START_CAMERA + _scaleFactor * UniverseHelper.CAMERA_STEP;
        float clampedSize = Mathf.Clamp(newSize, 0f, gameSettings.maxZoom);
        DOTween.To(SetCameraSize, originalSize, clampedSize, 0.5f).SetEase(Ease.OutQuart);
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
        if (gameSettings.debug)
        {
            float castMultiplier = await casting.GetCastMultiplier(token);
            _state = State.Shooting;
            await firePoint.Shoot(targetPosition, castMultiplier, token);
        }
        else
        {
            _state = State.Shooting;
            await firePoint.Shoot(targetPosition, 1f, token);
        }
        // await shootTowards.Shoot(targetPosition, castMultiplier, token);

        FinishAbsorbing();
        _state = State.Idle;
    }

    private void FinishAbsorbing()
    {
        if (_scaleFactor > UniverseHelper.SCALE_STEP * _threshold)
        {
            ZoomOut();
            _threshold = (int)(_scaleFactor /  UniverseHelper.SCALE_STEP) + 1;
            firePoint.Scale(_scaleFactor);
        }
    }

    private void OnDestroy()
    {
        _playerControls.Dispose();
    }
}
