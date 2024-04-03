using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlaySound : MonoBehaviour
{

    public void PlaySoundEffect(string id)
    {
        ServiceLocator.Instance.Get<IAudioService>().Play(id);
    }
    
}
