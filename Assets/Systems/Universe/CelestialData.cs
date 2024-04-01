public class CelestialData
{
    public float scaleFactor = 0.5f;

    public void Add(CelestialData data)
    {
        scaleFactor += data.scaleFactor;
    }
}