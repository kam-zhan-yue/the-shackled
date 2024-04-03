using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingGame
{
    private const float START = 0.15f;
    private readonly int _slotNum;
    private readonly FishingSlot[] _slots;

    public int SlotNum => _slotNum;
    public FishingSlot[] Slots => _slots;

    public Action<float> OnSetValue;
    
    public FishingGame(int slotNum)
    {
        _slotNum = slotNum;
        _slots = new FishingSlot[slotNum];
        Generate();
    }

    private void Generate()
    {
        float slotSize = GetSlotSize();
        float segmentSize = (1f - START) / _slotNum;
        for (int i = 0; i < _slotNum; ++i)
        {
            float start = START + segmentSize * i;
            float end = start + slotSize;
            _slots[i] = new FishingSlot(start, end);
        }
    }

    public void SetValue(float value)
    {
        OnSetValue?.Invoke(value);
    }

    public void DebugClass()
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            Debug.Log($"Slot {i+1} | Start: {_slots[i].start} End: {_slots[i].end}");
        }
    }

    private float GetSlotSize()
    {
        switch (_slotNum)
        {
            case 1:
                return 0.3f;
                break;
            case 2:
                return 0.2f;
                break;
            case 3:
                return 0.1f;
                break;
            case 4:
                return 0.1f;
                break;
            default:
                return 0.05f;
        }
    }

    public bool TryResolve(float value)
    {
        bool contains = false;
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i].Contains(value))
            {
                _slots[i].Resolve();
                contains = true;
            }
        }
        return contains;
    }

    public bool Success()
    {
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (!_slots[i].Resolved)
                return false;
        }
        return true;
    }
}
