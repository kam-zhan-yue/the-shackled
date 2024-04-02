using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private int _id;
    private const float RING_FOOD = 2f;
    [NonSerialized, ShowInInspector, ReadOnly]
    private List<CelestialBody> _celestialBodies = new List<CelestialBody>();
    [NonSerialized, ShowInInspector, ReadOnly]
    private List<CelestialBody> _centres = new List<CelestialBody>();

    public int ID => _id;
    public int Number => _id-3;

    public void Add(OrbitalSystem system)
    {
        List<CelestialBody> systemBodies = system.GetBodies();
        _celestialBodies.AddRange(systemBodies);
        _centres.Add(system.Centre);
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

    public void Lerp(float newOrbitalRadius, float newScaleFactor, float duration)
    {
        for (int i = _centres.Count-1; i>=0; --i)
        {
            _centres[i].Lerp(newOrbitalRadius, newScaleFactor, duration).Forget();
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

    public void Absorb()
    {
        IGodService god = ServiceLocator.Instance.Get<IGodService>();
        Debug.Log($"LOG | absorbing {name}");
        for (int i = _celestialBodies.Count-1; i>=0; --i)
        {
            CelestialData data = _celestialBodies[i].Absorb();
            god.Absorb(data);
        }
    }

    private void OnDestroy()
    {
        for (int i = _celestialBodies.Count-1; i>=0; --i)
        {
            _celestialBodies[i].onAbsorb -= OnAbsorb;
        }
    }
}
