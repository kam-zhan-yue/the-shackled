using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
    public Sound[] sounds = Array.Empty<Sound>();
}
