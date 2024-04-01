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
    
    protected override void SetParent()
    {
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
        for (int i = 0; i < _moons.Count; ++i)
        {
            _moons[i].Hook(pole);
        }
    }

    public override CelestialData Absorb()
    {
        CelestialData data = base.Absorb();
        for (int i = 0; i < _moons.Count; ++i)
        {
            data.Add(_moons[i].Absorb());
        }

        return data;
    }
}
