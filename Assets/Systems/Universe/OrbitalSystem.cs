using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class OrbitalSystem
{
    private enum SpawnPattern
    {
        Sequential = 0,
        Orbital = 1,
    }

    private SpawnPattern _spawnPattern;
    private CelestialBody _centre;
    private readonly List<OrbitalSystem> _orbitals;
    private float _radius;

    public CelestialBody Centre => _centre;

    private OrbitalSystem(CelestialBody centre, float radius)
    {
        Debug.Log($"Create New Orbital System with Centre: {centre} and Radius {radius}");
        _centre = centre;
        _orbitals = new();
        _radius = radius;
    }

    public OrbitalSystem(CelestialBody centre, List<CelestialBody> orbitals, float radius, float angle, float minRadius, float maxRadius)
    {
        _centre = centre;
        _orbitals = new();
        _spawnPattern = SpawnPattern.Orbital;
        for (int i = 0; i < orbitals.Count; ++i)
        {
            OrbitalSystem system = new OrbitalSystem(orbitals[i], Random.Range(minRadius, maxRadius));
            system.ArrangeOrbits(0f, 0f, 0f, 0f);
            _orbitals.Add(system);
        }
        _radius = radius;
        _centre.SetOrbitalRadius(_radius);
        _centre.SetStartingAngle(angle);
        _centre.SetOrbitalPeriod(Random.Range(5f, 15f));
    }

    public OrbitalSystem(CelestialBody centre, List<OrbitalSystem> orbitals, float radius)
    {
        _spawnPattern = SpawnPattern.Sequential;
        _centre = centre;
        _orbitals = orbitals;
        _radius = radius;
    }
    
    public void ArrangeOrbits(float minSize, float maxSize, float minSeparation, float maxSeparation)
    {
        float separation = 0f;
        switch (_spawnPattern)
        {
            case SpawnPattern.Sequential:
                separation = ArrangeSequential(minSize, maxSize, minSeparation, maxSeparation);
                break;
            case SpawnPattern.Orbital:
                separation = ArrangeOrbital(minSize, maxSize, minSeparation, maxSeparation);
                break;
        }
        _radius += separation;
    }

    private float ArrangeOrbital(float minSize, float maxSize, float minSeparation, float maxSeparation)
    {
        float separation = Random.Range(minSeparation, maxSeparation) + Centre.Radius;
        int orbits = _orbitals.Count;
        float angleSeparation = 360f / orbits;
        float startingAngle = Random.Range(0f, 360f);
        for (int i = 0; i < _orbitals.Count; ++i)
        {
            float randomSize = Random.Range(minSize, maxSize);
            Vector3 randomScale = Vector3.one * randomSize;
            _orbitals[i].Centre.SetScale(randomScale);
            _orbitals[i].Centre.SetOrbitalRadius(separation); 
            _orbitals[i].Centre.SetOrbitalPeriod(Random.Range(5f, 15f));
            _orbitals[i].Centre.SetStartingAngle(startingAngle + i * angleSeparation);
        }
        return separation;
    }

    private float ArrangeSequential(float minSize, float maxSize, float minSeparation, float maxSeparation)
    {
        float separation = 0f;
        for (int i = 0; i < _orbitals.Count; ++i)
        {
            float randomSize = Random.Range(minSize, maxSize);
            Vector3 randomScale = Vector3.one * randomSize;
            float randomSeparation = Random.Range(minSeparation, maxSeparation);
            separation += randomSize + randomSeparation + _orbitals[i]._radius;
            _orbitals[i].Centre.SetScale(randomScale);
            _orbitals[i].Centre.SetOrbitalRadius(separation); 
            _orbitals[i].Centre.SetOrbitalPeriod(Random.Range(5f, 15f));
        }

        return separation;
    }

    public void Add(OrbitalSystem system)
    {
        _orbitals.Add(system);
        _radius += system._radius;
    }
}