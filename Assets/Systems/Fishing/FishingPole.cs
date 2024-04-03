using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;

public class FishingPole : MonoBehaviour
{
    private IHookable _hookable;
    public Action<IHookable> OnHook;
    public Action<IHookable> OnReel;
    public Action<IHookable> OnLetGo;
    public Action<IHookable> OnAutoHook;
    private bool _canGame = false;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out IHookable hookable))
        {
            _hookable = hookable;
            if (_canGame && hookable.HasGame(out int slots))
            {
                OnHook?.Invoke(hookable);
                ServiceLocator.Instance.Get<IFishingService>().StartGame(this, slots, this.GetCancellationTokenOnDestroy());
            }
            else
            {
                AutoHook();
            }
        }
    }

    public void SetCanGame(bool canHook)
    {
        _canGame = canHook;
    }

    public void LetGo()
    {
        Debug.Log($"Failure! Let Go {_hookable}");
        OnLetGo?.Invoke(_hookable);
    }

    public void Reel()
    {
        Debug.Log($"Success! Reel {_hookable}");
        _hookable.Hook(transform);
        OnReel?.Invoke(_hookable);
    }

    private void AutoHook()
    {
        Debug.Log("FishingPole AutoHook");
        _hookable.Hook(transform);
        OnAutoHook?.Invoke(_hookable);
    }
}
