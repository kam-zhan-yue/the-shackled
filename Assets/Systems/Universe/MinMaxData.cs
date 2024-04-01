using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class MinMaxData
{
    [MinMaxSlider(0f, 2f, true)]
    public Vector2 orbitRadius = new Vector2(0f, 2f);
    [MinMaxSlider(0f, 2f, true)]
    public Vector2 bodyRadius = new Vector2(0f, 2f);
    [MinMaxSlider(0f, 2f, true)]
    public Vector2 separation = new Vector2(0f, 2f);
    [MinMaxSlider(0f, 15f, true)]
    public Vector2 orbitalPeriod = new Vector2(5f, 15f);
}