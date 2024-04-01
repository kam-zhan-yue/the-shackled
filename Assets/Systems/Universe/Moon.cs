using Unity.VisualScripting;

public class Moon : CelestialBody
{
    public CelestialBody parentBody;
    
    protected override void SetParent()
    {
        parent = transform.parent.transform;
        parentBody = parent.GetComponent<CelestialBody>();
        if (parentBody.GetType() == typeof(Planet))
        {
            Planet planet = (Planet)parentBody;
            planet.AddMoon(this);
        }
    }
    
    private void OnValidate()
    {
        if (transform.parent != null)
            transform.position = GetStartPosition(transform.parent.position);
    }
}