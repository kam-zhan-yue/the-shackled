using System;
using System.Collections;
using System.Collections.Generic;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private int _id;
    private const float RING_FOOD = 1f;
    [NonSerialized, ShowInInspector, ReadOnly]
    private List<CelestialBody> _celestialBodies = new List<CelestialBody>();

    public void Add(OrbitalSystem system)
    {
        List<CelestialBody> systemBodies = system.GetBodies();
        _celestialBodies.AddRange(systemBodies);
    }

    public void Init(int id)
    {
        _id = id;
        float foodPerBody = RING_FOOD / _celestialBodies.Count;
        for (int i = _celestialBodies.Count-1; i>=0; --i)
        {
            CelestialData data = new CelestialData(foodPerBody);
            _celestialBodies[i].SetData(data);
            _celestialBodies[i].onAbsorb += OnAbsorb;
        }
    }

    public void Simulate()
    {
        for (int i = _celestialBodies.Count-1; i>=0; --i)
        {
            _celestialBodies[i].Simulate();
        }
    }

    private void OnDestroy()
    {
        for (int i = _celestialBodies.Count-1; i>=0; --i)
        {
            _celestialBodies[i].onAbsorb -= OnAbsorb;
        }
    }

    private void OnAbsorb(CelestialBody body)
    {
        _celestialBodies.Remove(body);
        if (_celestialBodies.Count <= 0)
        {
            ServiceLocator.Instance.Get<IUniverseService>().ReportRingEaten(_id);
        }
    }
}
