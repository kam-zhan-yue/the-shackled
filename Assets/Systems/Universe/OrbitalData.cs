using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class OrbitalData
{
    [SerializeField] private bool clockwiseOrbit;
    [SerializeField] private float startAngle;
    [SerializeField] private float orbitalPeriod;
    [SerializeField] private float orbitalRadius;
    [SerializeField] private float bodyScale;

    public bool ClockwiseOrbit => clockwiseOrbit;
    public float StartAngle => startAngle;
    public float OrbitalPeriod => orbitalPeriod;
    public float OrbitalRadius => orbitalRadius;
    public float Scale => bodyScale;

    public OrbitalData(float radius, float scale, float angle, float period, bool clockwise)
    {
        orbitalRadius = radius;
        bodyScale = scale;
        startAngle = angle;
        orbitalPeriod = period;
        clockwiseOrbit = clockwise;
    }

    public void SetClockwise(bool clockwise)
    {
        clockwiseOrbit = clockwise;
    }

    public void SetStartingAngle(float angle)
    {
        startAngle = angle;
    }

    public void SetOrbitalRadius(float radius)
    {
        orbitalRadius = radius;
    }

    public void SetOrbitalPeriod(float period)
    {
        orbitalPeriod = period;
    }

    public void Randomise(float maxPeriod, float maxRadius)
    {
        clockwiseOrbit = Random.value > 0.5f;
        startAngle = Random.Range(0f, 360f);
        orbitalPeriod = Random.Range(5f, maxPeriod);
        orbitalRadius = Random.Range(2f, maxRadius);
    }
}