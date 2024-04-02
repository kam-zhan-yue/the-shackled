using Kuroneko.UtilityDelivery;
using UnityEngine;

public interface IUniverseService : IGameService
{
    public Transform GetCentre();
    public void StartSimulation();
    public void PauseSimulation();
    public void ReportRingEaten(int id);
    public void SpawnRing();
    public void EatRing();
    public void DebugClass();
}