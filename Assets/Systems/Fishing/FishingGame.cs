using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishingGame
{
    private const float START = 0.1f;
    private readonly int _slotNum;
    private readonly FishingSlot[] _slots;

    public int SlotNum => _slotNum;
    public FishingSlot[] Slots => _slots;
    
    public FishingGame(int slotNumNum)
    {
        _slotNum = slotNumNum;
        _slots = new FishingSlot[slotNumNum];
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
}
