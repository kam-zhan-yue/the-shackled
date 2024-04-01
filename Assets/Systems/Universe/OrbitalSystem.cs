using System;
using System.Collections.Generic;
using TMPro;
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
    private float _orbitalRadius;
    private OrbitalData _orbitalData;
    private Vector2 _outerOrbitalPeriod;

    public CelestialBody Centre => _centre;

    private OrbitalSystem(CelestialBody centre, float orbitalRadius, OrbitalData data)
    {
        _centre = centre;
        _orbitals = new();
        _orbitalRadius = orbitalRadius;
        _orbitalData = data;
        InitCentre();
    }

    public OrbitalSystem(CelestialBody centre, List<CelestialBody> orbitals, OrbitalData orbitalData, Vector2 bodySize, Vector2 orbitalPeriod)
    {
        _centre = centre;
        _orbitals = new();
        _spawnPattern = SpawnPattern.Orbital;
        for (int i = 0; i < orbitals.Count; ++i)
        {
            OrbitalSystem system = new OrbitalSystem(orbitals[i], UniverseHelper.RandomValue(bodySize), orbitalData);
            system.ArrangeOrbits(0f, 0f, 0f, 0f);
            _orbitals.Add(system);
        }

        _orbitalData = orbitalData;
        _orbitalRadius = _orbitalData.OrbitalRadius;
        _outerOrbitalPeriod = orbitalPeriod;
        InitCentre();
    }

    public OrbitalSystem(CelestialBody centre, List<OrbitalSystem> orbitals, OrbitalData orbitalData, Vector2 orbitalPeriod)
    {
        _orbitalData = orbitalData;
        _spawnPattern = SpawnPattern.Sequential;
        _centre = centre;
        _orbitals = orbitals;
        _orbitalRadius = _orbitalData.OrbitalRadius;
        _outerOrbitalPeriod = orbitalPeriod;
        InitCentre();
    }

    private void InitCentre()
    {
        _centre.SetScale(Vector3.one * _orbitalData.Scale);
        _centre.SetOrbitalRadius(_orbitalRadius);
        _centre.SetStartingAngle( _orbitalData.StartAngle);
        _centre.SetOrbitalPeriod(_orbitalData.OrbitalPeriod);
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
        _orbitalRadius += separation;
        Debug.Log($"Arranging Orbitals for {_centre.gameObject.GetInstanceID()} {_centre.name} {_orbitalRadius} Separation: {separation}");
    }

    private float ArrangeOrbital(float minSize, float maxSize, float minSeparation, float maxSeparation)
    {
        float separation = Random.Range(minSeparation, maxSeparation) + Centre.Radius * 0.5f;
        int orbits = _orbitals.Count;
        float angleSeparation = 360f / orbits;
        bool clockwise = UniverseHelper.ClockwiseRotation();
        float startingAngle = Random.Range(0f, 360f);
        float orbitalPeriod = UniverseHelper.RandomValue(_outerOrbitalPeriod);
        for (int i = 0; i < _orbitals.Count; ++i)
        {
            float randomSize = Random.Range(minSize, maxSize);
            Vector3 randomScale = Vector3.one * randomSize;
            _orbitals[i].Centre.SetClockwise(clockwise);
            _orbitals[i].Centre.SetScale(randomScale);
            _orbitals[i].Centre.SetOrbitalRadius(separation); 
            _orbitals[i].Centre.SetOrbitalPeriod(orbitalPeriod);
            _orbitals[i].Centre.SetStartingAngle(startingAngle + i * angleSeparation);
        }
        return separation;
    }

    private float ArrangeSequential(float minSize, float maxSize, float minSeparation, float maxSeparation)
    {
        float separation = 0f;
        Debug.Log($"Arrange Sequential for {_centre.name}");
        for (int i = 0; i < _orbitals.Count; ++i)
        {
            float randomSize = Random.Range(minSize, maxSize);
            Vector3 randomScale = Vector3.one * randomSize;
            float randomSeparation = Random.Range(minSeparation, maxSeparation);
            separation += randomSize + randomSeparation + _orbitals[i]._orbitalRadius;
            bool clockwise = UniverseHelper.ClockwiseRotation();
            float orbitalPeriod = UniverseHelper.RandomValue(_outerOrbitalPeriod);
            _orbitals[i].Centre.SetClockwise(clockwise);
            _orbitals[i].Centre.SetScale(randomScale);
            _orbitals[i].Centre.SetOrbitalRadius(separation);
            _orbitals[i].Centre.SetOrbitalPeriod(orbitalPeriod);
            Debug.Log($"Size: {randomSize}, Separation: {randomSeparation} Radius: {_orbitals[i]._orbitalRadius}");
            Debug.Log($"Set Orbital Radius of {_orbitals[i].Centre.name} to {separation}");
        }

        return separation;
    }

    public void Add(OrbitalSystem system)
    {
        _orbitals.Add(system);
        _orbitalRadius += system._orbitalRadius;
    }
}