using System;
using UnityEngine;

public class FishingSlot
{
    public float start;
    public float end;
    public float Size => end - start;
    public bool Resolved { get; private set; }

    public Action OnResolve;

    public FishingSlot(float start, float end)
    {
        this.start = start;
        this.end = end;
        Resolved = false;
    }

    public bool Contains(float value)
    {
        Debug.Log($"Contains: {start} | {value} | {end}");
        return value >= start && value <= end;
    }

    public void Resolve()
    {
        Debug.Log("Resolve!");
        OnResolve?.Invoke();
        Resolved = true;
    }
}