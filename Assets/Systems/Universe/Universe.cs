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
    [BoxGroup("Scriptable Objects"), SerializeField] private PlanetDatabase planetDatabase;
    [BoxGroup("Scriptable Objects"), SerializeField] private GameSettings gameSettings;
    [NonSerialized, ShowInInspector, ReadOnly] private List<CelestialBody> _bodies = new();
    private GameObject _centre;
    private OrbitalSystem _universeOrbital;
    [SerializeField] private float angleStep = 10f;
    [SerializeField] private int minSpawnPerRing = 1;
    [SerializeField] private int maxSpawnPerRing = 4;
    private int _ring = 3;
    private readonly Dictionary<int, List<OrbitalSystem>> _ringDictionary = new();

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
    }

    public Transform GetCentre()
    {
        return _centre.transform;
    }

    [Button]
    public void SpawnRing()
    {
        int fibonacci = UniverseHelper.GetFibonacci(_ring);
        float angle = angleStep * fibonacci;
        float scale = UniverseHelper.GetScaleModifier(fibonacci);
        int spawnPerRing = Random.Range(minSpawnPerRing, maxSpawnPerRing);
        float innerAngleStep = 360f / spawnPerRing;

        List<OrbitalSystem> systems = new();
        for (int j = 0; j < spawnPerRing; ++j)
        {
            float spawnAngle = angle + j * innerAngleStep;
            SpawnType spawnType = GetSpawnType();
            switch (spawnType)
            {
                case SpawnType.Planet:
                    systems.Add(planetDatabase.GeneratePlanet(fibonacci, scale, spawnAngle));
                    break;
                case SpawnType.SolarSystem:
                    systems.Add(planetDatabase.GenerateSolarSystem(fibonacci, scale, angle));
                    break;
            }
        }

        _ringDictionary.Add(_ring, systems);
        _ring++;
    }

    private SpawnType GetSpawnType()
    {
        if (_ring < 5)
        {
            return SpawnType.Planet;
        }

        return Random.value > 0.8f ? SpawnType.Planet : SpawnType.SolarSystem;
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
        for (int i = _ring; i < 100; ++i)
        {
            int fibonacci = UniverseHelper.GetFibonacci(i);
            float angle = angleStep * fibonacci;
            int spawnPerRing = maxSpawnPerRing;
            float innerAngleStep = 360f / spawnPerRing;
            for (int j = 0; j < spawnPerRing; ++j)
            {
                float spawnAngle = angle + j * innerAngleStep;
                Vector2 rotation = UniverseHelper.ConvertAngleToRotation(spawnAngle);
                Gizmos.DrawSphere(rotation * fibonacci, UniverseHelper.GetScaleModifier(fibonacci));
            }
            Gizmos.DrawWireSphere(transform.position, fibonacci);
        }
    }
}
