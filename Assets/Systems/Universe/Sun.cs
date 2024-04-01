using System.Collections;
using System.Collections.Generic;
using Kuroneko.UtilityDelivery;
using UnityEngine;

public class Sun : CelestialBody
{
    protected override void SetParent()
    {
        parent = ServiceLocator.Instance.Get<IUniverseService>().GetCentre();
    }
}
