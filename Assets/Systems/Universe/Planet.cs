using System.Collections;
using System.Collections.Generic;
using Kuroneko.UtilityDelivery;
using UnityEngine;

public class Planet : CelestialBody
{
    private readonly List<Moon> _moons = new List<Moon>();

    public void AddMoon(Moon moon)
    {
        _moons.Add(moon);
    }
    
    public void RemoveMoon(Moon moon)
    {
        _moons.Remove(moon);
    }

    public void SetParent(Transform newParent)
    {
        parent = newParent;
    }
    
    protected override void SetParent()
    {
        if(parent == null)
            parent = ServiceLocator.Instance.Get<IUniverseService>().GetCentre();
    }
    private void OnValidate()
    {
        GameObject centre = UniverseHelper.GetCentre();
        if(centre != null)
            transform.position = GetStartPosition(UniverseHelper.GetCentre().transform.position);
    }

    public override void Hook(Transform pole)
    {
        base.Hook(pole);
        for (int i = _moons.Count - 1; i >= 0; --i)
        {
            _moons[i].Hook(pole);
        }
    }

    public override CelestialData Absorb()
    {
        CelestialData data = base.Absorb();
        for (int i = _moons.Count - 1; i >= 0; --i)
        {
            _moons[i].Absorb();
        }

        return data;
    }
}
