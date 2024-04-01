using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/PlanetDatabase")]
public class PlanetDatabase : ScriptableObject
{
    [TableList]
    public PlanetData[] planets = Array.Empty<PlanetData>();
    public Moon moonPrefab;
    public Sun sunPrefab;
    public BlackHole blackHolePrefab;

    public PlanetData GetRandomPlanet()
    {
        int randomIndex = Random.Range(0, planets.Length);
        return planets[randomIndex];
    }

    [Button]
    public OrbitalSystem GeneratePlanet()
    {
        PlanetData planetData = GetRandomPlanet();
        Planet planet = Instantiate(planetData.prefab);
        List<CelestialBody> moons = new List<CelestialBody>();
        if (planetData.moons)
        {
            int numMoons = Random.Range(0, UniverseHelper.MAX_MOONS);
            for (int i = 0; i < numMoons; ++i)
            {
                Moon moon = Instantiate(moonPrefab, planet.transform);
                moons.Add(moon);
            }
        }
        
        OrbitalSystem orbitalSystem = new(planet, moons, Random.Range(1f, 10f));
        orbitalSystem.Arrange(0.1f, 0.5f, 0.5f, 1f);
        return orbitalSystem;
    }

    [Button]
    public OrbitalSystem GenerateSolarSystem()
    {
        CelestialBody centre = GenerateCentre();
        int numPlanets = Random.Range(1, UniverseHelper.MAX_PLANETS);
        List<CelestialBody> solarSystemPlanets = new();
        for (int i = 0; i < numPlanets; ++i)
        {
            PlanetData planetData = GetRandomPlanet();
            Planet planet = Instantiate(planetData.prefab, centre.transform);
            solarSystemPlanets.Add(planet);
        }

        OrbitalSystem orbitalSystem = new(centre, solarSystemPlanets, Random.Range(1f, 10f));
        orbitalSystem.Arrange(1f, 2f, 1f, 3f);
        return orbitalSystem;
    }

    private CelestialBody GenerateCentre()
    {
        return Random.value > UniverseHelper.BLACK_HOLE_SPAWN_RATE ? InstantiateBlackHole() : InstantiateSun();
    }

    private BlackHole InstantiateBlackHole()
    {
        return Instantiate(blackHolePrefab);
    }

    private Sun InstantiateSun()
    {
        return Instantiate(sunPrefab);
    }
}