using Kuroneko.UtilityDelivery;

public interface IGodService : IGameService
{
    public void Absorb(CelestialData data);
    public void ResetScale();
    public void ZoomOut();
}