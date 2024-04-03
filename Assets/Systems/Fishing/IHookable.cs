using UnityEngine;

public interface IHookable
{
    public bool HasGame(out int slots);
    public void Hook(Transform pole);
    public void Reel(Transform pole);
    public CelestialData Absorb();
}