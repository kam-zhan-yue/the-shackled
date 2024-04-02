public class FishingSlot
{
    public float start;
    public float end;
    public float Size => end - start;

    public FishingSlot(float start, float end)
    {
        this.start = start;
        this.end = end;
    }
}