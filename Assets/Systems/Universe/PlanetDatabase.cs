using System;
using System.Collections.Generic;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "ScriptableObject/PlanetDatabase")]
public class PlanetDatabase : ScriptableObject
{
    [FoldoutGroup("Moon RNG"), HideLabel]
    public MinMaxData moonMinMaxData = new MinMaxData();
    [FoldoutGroup("Planet RNG"), HideLabel]
    public MinMaxData planetMinMaxData = new MinMaxData();
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

    [FoldoutGroup("Debug Functions"), Button]
    private void SpawnRing()
    {
        ServiceLocator.Instance.Get<IUniverseService>().SpawnRing();
    }

    [FoldoutGroup("Debug Functions"), Button]
    private void EatRing()
    {
        ServiceLocator.Instance.Get<IUniverseService>().EatRing();
    }
    
    [FoldoutGroup("Debug Functions"), Button]
    private void Debug()
    {
        ServiceLocator.Instance.Get<IUniverseService>().DebugClass();
    }

    [Button]
    public OrbitalSystem GeneratePlanet(OrbitalData orbitalData)
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
        
        OrbitalSystem orbitalSystem = new(planet, moons, orbitalData,
            moonMinMaxData.orbitRadius, moonMinMaxData.orbitalPeriod);
        orbitalSystem.ArrangeOrbits(moonMinMaxData.bodyRadius.x, moonMinMaxData.bodyRadius.y, 
            moonMinMaxData.separation.x, moonMinMaxData.separation.y);
        return orbitalSystem;
    }

    [Button]
    public OrbitalSystem GenerateSolarSystem(OrbitalData orbitalData)
    {
        CelestialBody centre = GenerateCentre();
        int numPlanets = Random.Range(1, UniverseHelper.MAX_PLANETS);
        List<OrbitalSystem> solarSystemOrbitals = new();

        //Each individual planet has their own orbital data within the Solar System's orbital data
        bool clockwise = UniverseHelper.ClockwiseRotation();
        float scale = orbitalData.Scale;
        float period = UniverseHelper.RandomValue(planetMinMaxData.orbitalPeriod);
        
        for (int i = 0; i < numPlanets; ++i)
        {
            float startAngle = Random.Range(0f, 360f);
            float planetOrbitalRadius = UniverseHelper.RandomValue(planetMinMaxData.orbitRadius);
            OrbitalData planetData = new OrbitalData(planetOrbitalRadius, scale, startAngle, period, clockwise);
            OrbitalSystem planetOrbitalSystem = GeneratePlanet(planetData);
            Planet planet = (Planet)planetOrbitalSystem.Centre;
            planet.SetParent(centre.transform);
            planetOrbitalSystem.Centre.transform.parent = centre.transform;
            solarSystemOrbitals.Add(planetOrbitalSystem);
        }

        OrbitalSystem orbitalSystem = new(centre, solarSystemOrbitals, orbitalData, planetMinMaxData.orbitalPeriod);
        orbitalSystem.ArrangeOrbits(
            planetMinMaxData.bodyRadius.x, planetMinMaxData.bodyRadius.y, 
            planetMinMaxData.separation.x, planetMinMaxData.separation.y);
        return orbitalSystem;
    }

    private CelestialBody GenerateCentre()
    {
        return Random.value <= UniverseHelper.BLACK_HOLE_SPAWN_RATE ? InstantiateBlackHole() : InstantiateSun();
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