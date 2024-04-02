public class CelestialData
{
    public float food;

    public CelestialData(float food)
    {
        this.food = food;
    }

    public void Add(CelestialData data)
    {
        food += data.food;
    }
}