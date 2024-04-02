using System;
using System.Collections;
using System.Collections.Generic;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using TMPro.EditorUtilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class Universe : MonoBehaviour, IUniverseService
{
    private enum SpawnType
    {
        Planet = 0,
        SolarSystem = 1,
    }

    private enum SimulationState
    {
        Paused = 0,
        Running = 1
    }
    
    [BoxGroup("Scriptable Objects"), SerializeField] private PlanetDatabase planetDatabase;
    [BoxGroup("Scriptable Objects"), SerializeField] private GameSettings gameSettings;
    [NonSerialized, ShowInInspector, ReadOnly] private List<CelestialBody> _bodies = new();
    private GameObject _centre;
    private OrbitalSystem _universeOrbital;
    [SerializeField] private float angleStep = 10f;
    [SerializeField] private int minSpawnPerRing = 1;
    [SerializeField] private int maxSpawnPerRing = 4;
    private int _ringIndex = 4;
    
    [NonSerialized, ShowInInspector, ReadOnly]
    private readonly List<Ring> _rings = new List<Ring>();

    private SimulationState _simulationState = SimulationState.Paused;

    private void Awake()
    {
        ServiceLocator.Instance.Register<IUniverseService>(this);
        _centre = UniverseHelper.GetCentre();
    }
    
    private void Start()
    {
        for (int i = 0; i < gameSettings.initialRings; ++i)
        {
            SpawnRing();
        }
        ServiceLocator.Instance.Get<IUniverseService>().StartSimulation();
    }

    public Transform GetCentre()
    {
        return _centre.transform;
    }

    public void StartSimulation()
    {
        _simulationState = SimulationState.Running;
    }

    public void PauseSimulation()
    {
        _simulationState = SimulationState.Paused;
    }

    public void ReportRingEaten(int id)
    {
        Debug.Log($"Ring {id} Completely Eaten!");
        SpawnRing();
    }

    private void FixedUpdate()
    {
        switch (_simulationState)
        {
            case SimulationState.Paused:
                break;
            case SimulationState.Running:
                SimulateRings();
                break;
        }
    }

    private void SimulateRings()
    {
        for (int i = 0; i < _rings.Count; ++i)
        {
            _rings[i].Simulate();
        }
    }

    [Button]
    public void SpawnRing()
    {
        GameObject ringGameObject = new GameObject($"Ring {_ringIndex - 3}");
        Ring ring = ringGameObject.AddComponent<Ring>();
        _rings.Add(ring);
        
        //Calculate spawns per ring and angles between spawns
        int fibonacci = UniverseHelper.GetFibonacci(_ringIndex);
        float angle = angleStep * fibonacci;
        int spawnPerRing = Random.Range(minSpawnPerRing, maxSpawnPerRing);
        float innerAngleStep = 360f / spawnPerRing;

        //Determine if the ring is clockwise
        bool clockwise = UniverseHelper.ClockwiseRotation();
        //Set the orbital period for all elements in the ring
        float period = UniverseHelper.RandomValue(planetDatabase.planetMinMaxData.orbitalPeriod);
        float orbitalRadius = UniverseHelper.GetRingOrbitalRadius(_ringIndex);
        float scaleFactor = UniverseHelper.GetRingScaleFactor(_ringIndex);
        
        for (int i = 0; i < spawnPerRing; ++i)
        {
            float spawnAngle = angle + i * innerAngleStep;
            SpawnType spawnType = GetSpawnType();
            switch (spawnType)
            {
                case SpawnType.Planet:
                    OrbitalData planetData = new OrbitalData(orbitalRadius, scaleFactor, spawnAngle, period, clockwise);
                    OrbitalSystem planetSystem = planetDatabase.GeneratePlanet(planetData);
                    planetSystem.Centre.transform.parent = ringGameObject.transform;
                    ring.Add(planetSystem);
                    break;
                case SpawnType.SolarSystem:
                    OrbitalData solarSystemData = new OrbitalData(orbitalRadius, scaleFactor, spawnAngle, period, clockwise);
                    OrbitalSystem solarSystem = planetDatabase.GenerateSolarSystem(solarSystemData);
                    solarSystem.Centre.transform.parent = ringGameObject.transform;
                    ring.Add(solarSystem);
                    break;
            }
        }
        ring.Init(_ringIndex);
        _ringIndex++;
    }

    private SpawnType GetSpawnType()
    {
        if (_ringIndex < 5)
        {
            return SpawnType.Planet;
        }

        return Random.value > 0.8f ? SpawnType.SolarSystem : SpawnType.Planet;
    }

    [Button]
    private void DebugSpawnRing(int rings)
    {
        for (int i = 0; i < rings; ++i)
        {
            SpawnRing();
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = _ringIndex; i < 100; ++i)
        {
            int fibonacci = UniverseHelper.GetFibonacci(i);
            float angle = angleStep * fibonacci;
            int spawnPerRing = maxSpawnPerRing;
            float innerAngleStep = 360f / spawnPerRing;

            float orbitalRadius = UniverseHelper.GetRingOrbitalRadius(i);
            float scaleFactor = UniverseHelper.GetRingScaleFactor(i);
            Debug.Log($"Ring {i} Orbital Radius {orbitalRadius}");
            
            for (int j = 0; j < spawnPerRing; ++j)
            {
                float spawnAngle = angle + j * innerAngleStep;
                Vector2 rotation = UniverseHelper.ConvertAngleToRotation(spawnAngle);
                Gizmos.DrawSphere(rotation * orbitalRadius, UniverseHelper.GetScaleModifier(scaleFactor));
            }
            Gizmos.DrawWireSphere(transform.position, orbitalRadius);
        }
    }
}
