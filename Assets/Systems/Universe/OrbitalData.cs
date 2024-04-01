using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class OrbitalData
{
    [SerializeField] private bool clockwiseOrbit = false;
    [SerializeField] private float startAngle = 0f;
    [SerializeField] private float orbitalPeriod = 1f;
    [SerializeField] private float orbitalRadius = 1f;

    public bool ClockwiseOrbit => clockwiseOrbit;
    public float StartAngle => startAngle;
    public float OrbitalPeriod => orbitalPeriod;
    public float OrbitalRadius => orbitalRadius;

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