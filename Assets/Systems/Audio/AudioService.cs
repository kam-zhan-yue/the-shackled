using System;
using System.Collections;
using System.Collections.Generic;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioService : MonoBehaviour, IAudioService
{
    public SoundDatabase soundDatabase;
    private readonly Dictionary<string, AudioSource> _sources = new();

    private void Awake()
    {
        ServiceLocator.Instance.Register<IAudioService>(this);
        for (int i = 0; i < soundDatabase.sounds.Length; ++i)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = soundDatabase.sounds[i].clip;
            audioSource.pitch = soundDatabase.sounds[i].pitch;
            audioSource.loop = soundDatabase.sounds[i].loop;
            _sources.Add(soundDatabase.sounds[i].id, audioSource);
        }
    }

    [Button]
    public void Play(string clipName)
    {
        if (_sources.TryGetValue(clipName, out AudioSource source))
        {
            source.Play();
        }
    }

    [Button]
    public void Stop(string clipName)
    {
        if (_sources.TryGetValue(clipName, out AudioSource source))
        {
            source.Stop();
        }
    }
}