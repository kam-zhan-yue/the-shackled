using System.Collections.Generic;
using UnityEngine;

public class OrbitalSystem
{
    private CelestialBody _centre;
    private readonly List<OrbitalSystem> _orbitals;
    private float _radius;

    public CelestialBody Centre => _centre;
    public float Diameter => _radius * 2f;

    private OrbitalSystem(CelestialBody centre)
    {
        _centre = centre;
        _orbitals = new();
    }

    public OrbitalSystem(CelestialBody centre, List<CelestialBody> orbitals, float radius)
    {
        _centre = centre;
        _orbitals = new();
        for (int i = 0; i < orbitals.Count; ++i)
        {
            _orbitals.Add(new OrbitalSystem(centre));
        }
        _radius = radius;
    }

    public OrbitalSystem(CelestialBody centre, List<OrbitalSystem> orbitals, float radius)
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
            _orbitals[i].Centre.SetScale(randomScale);
            _orbitals[i].Centre.SetOrbitalRadius(separation); 
            _orbitals[i].Centre.SetOrbitalPeriod(Random.Range(5f, 15f));
        }

        _radius += separation;
        _centre.SetOrbitalRadius(_radius);
        _centre.SetOrbitalPeriod(Random.Range(5f, 15f));
    }
}