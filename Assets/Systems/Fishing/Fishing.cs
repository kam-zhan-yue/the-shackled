using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fishing : MonoBehaviour, IFishingService
{
    [SerializeField] private float fishTime = 1f;
    private PlayerControls _playerControls;
    private FishingGame _fishingGame = new FishingGame(0);

    private float _fishTimer = 0f;
    private bool _fishing = false;
    private bool gameEnded = false;

    private Action<FishingGame> OnGameStart;
    private Action<FishingGame> OnGameEnd;

    private void Awake()
    {
        ServiceLocator.Instance.Register<IFishingService>(this);
        _playerControls = new();
        _playerControls.Game.Shoot.performed += Shoot;
    }
    
    public void RegisterGameStart(Action<FishingGame> gameStart)
    {
        OnGameStart += gameStart;
    }

    public void RegisterGameEnd(Action<FishingGame> gameEnd)
    {
        OnGameEnd += gameEnd;
    }

    public void UnregisterGameStart(Action<FishingGame> gameStart)
    {
        OnGameStart -= gameStart;
    }

    public void UnregisterGameEnd(Action<FishingGame> gameEnd)
    {
        OnGameEnd -= gameEnd;
    }

    private void Shoot(InputAction.CallbackContext callbackContext)
    {
        float value = _fishTimer / fishTime;
        
        //If missed, then fishing game is over
        if (!_fishingGame.TryResolve(value))
        {
            _fishing = false;
        }

        // if (_fishingGame.Success())
        // {
        //     OnGameEnd?.Invoke(_fishingGame);
        // }
    }

    public void StartGame(FishingPole pole, int slots, CancellationToken token)
    {
        StartGameAsync(pole, slots, token).Forget();
    }

    private async UniTask StartGameAsync(FishingPole pole, int slots, CancellationToken token)
    {
        // Pause the Universe and create a new fishing game
        ServiceLocator.Instance.Get<IUniverseService>().PauseSimulation();
        _fishingGame = new FishingGame(slots);
        _playerControls.Enable();
        OnGameStart?.Invoke(_fishingGame);
        // Wait for fishing game...
        await FishingLoopAsync(token);
        // Report the result of the game
        ReportFishingResult(pole, _fishingGame.Success());
        ServiceLocator.Instance.Get<IUniverseService>().StartSimulation();
        _playerControls.Disable();
        OnGameEnd?.Invoke(_fishingGame);
    }

    private async UniTask FishingLoopAsync(CancellationToken token)
    {
        // Init all values
        _fishTimer = 0f;
        _fishing = true;
        // Update values within while loop
        while (_fishing)
        {
            _fishTimer += Time.deltaTime;
            _fishingGame.SetValue(_fishTimer/fishTime);
            if (_fishTimer >= fishTime)
                _fishing = false;
            await UniTask.NextFrame(token);
        }
    }

    private void ReportFishingResult(FishingPole pole, bool success)
    {
        if (pole)
        {
            if (_fishingGame.Success())
            {
                pole.Reel();
            }
            else
            {
                ServiceLocator.Instance.Get<IAudioService>().Play("SLOT");
                CinemachineShake.Instance.ShakeCamera(2f, 0.2f);
                pole.LetGo();
            }
        }
    }

    [Button]
    private void TestGame(int slots)
    {
        StartGameAsync(null, slots, this.GetCancellationTokenOnDestroy()).Forget();
    }
}
