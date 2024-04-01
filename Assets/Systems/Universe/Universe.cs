using System;
using System.Collections;
using System.Collections.Generic;
using Kuroneko.UtilityDelivery;
using TMPro.EditorUtilities;
using UnityEngine;

public class Universe : MonoBehaviour, IUniverseService
{
    private List<CelestialBody> _bodies = new();
    private GameObject _centre;
    
    private void Awake()
    {
        ServiceLocator.Instance.Register<IUniverseService>(this);
        _centre = UniverseHelper.GetCentre();
    }

    private void Start()
    {
        CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
        _bodies = new List<CelestialBody>(bodies);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _bodies.Count; ++i)
        {
            // _bodies[i].UpdateVelocity(UniverseHelper.PHYSICS_TIME_STEP);
        }
    }

    public Transform GetCentre()
    {
        return _centre.transform;
    }
}
