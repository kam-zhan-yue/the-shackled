using UnityEngine;

public class Moon : CelestialBody
{
    public CelestialBody parentBody;
    private Planet _planet;

    protected override void SetParent()
    {
        parent = transform.parent.transform;
        parentBody = parent.GetComponent<CelestialBody>();
        if (parentBody.GetType() == typeof(Planet))
        {
            _planet = (Planet)parentBody;
            _planet.AddMoon(this);
        }
    }
    
    public override CelestialData Absorb()
    {
        CelestialData data = base.Absorb();
        if(_planet != null)
            _planet.RemoveMoon(this);
        return data;
    }

    private void OnValidate()
    {
        if (transform.parent != null)
            transform.position = GetStartPosition(transform.parent.position);
    }
}