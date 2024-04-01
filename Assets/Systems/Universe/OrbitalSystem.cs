using System.Collections.Generic;
using UnityEngine;

public class OrbitalSystem
{
    private CelestialBody _centre;
    private readonly List<CelestialBody> _orbitals;
    private float _radius;

    public OrbitalSystem(CelestialBody centre, List<CelestialBody> orbitals, float radius)
    {
        _centre = centre;
        _orbitals = orbitals;
        _radius = radius;
    }

    public void Arrange(float minSize, float maxSize, float minSeparation, float maxSeparation)
    {
        float separation = 0f;
        for (int i = 0; i < _orbitals.Count; ++i)
        {
            float randomSize = Random.Range(minSize, maxSize);
            Vector3 randomScale = Vector3.one * randomSize;
            float randomSeparation = Random.Range(minSeparation, maxSeparation);
            separation += randomSize + randomSeparation;
            _orbitals[i].SetScale(randomScale);
            _orbitals[i].SetOrbitalRadius(separation);
            _orbitals[i].SetOrbitalPeriod(Random.Range(5f, 15f));
        }

        _radius += separation;
        _centre.SetOrbitalRadius(_radius);
        _centre.SetOrbitalPeriod(Random.Range(5f, 15f));
    }
}