using System.Collections;
using System.Collections.Generic;
using Kuroneko.UIDelivery;
using Kuroneko.UtilityDelivery;
using Sirenix.OdinInspector;
using UnityEngine;

public class FishingPopup : Popup
{
    [SerializeField] private RectTransform slotHolder;
    [SerializeField] private FishingSlotPopupItem sampleSlotItem;

    private List<FishingSlotPopupItem> _slots = new();
    private FishingGame _data;
    
    protected override void InitPopup()
    {
        
    }

    [Button]
    private void Test(int slots)
    {
        FishingGame game = new FishingGame(slots);
        game.DebugClass();
        Init(game);
    }

    private void Init(FishingGame game)
    {
        _data = game;
        TryInstantiate();
        for (int i = 0; i < _slots.Count; ++i)
        {
            if (i < _data.SlotNum)
            {
                _slots[i].gameObject.SetActiveFast(true);
                _slots[i].Init(_data.Slots[i]);
            }
            else
            {
                _slots[i].gameObject.SetActiveFast(false);
            }
        }
    }

    private void TryInstantiate()
    {
        int numToSpawn = _data.SlotNum - _slots.Count;
        if (numToSpawn > 0)
        {
            sampleSlotItem.gameObject.SetActiveFast(true);
            for (int i = 0; i < numToSpawn; ++i)
            {
                FishingSlotPopupItem item = Instantiate(sampleSlotItem, slotHolder);
                item.gameObject.SetActiveFast(false);
                _slots.Add(item);
            }
        }
        sampleSlotItem.gameObject.SetActiveFast(false);
    }
}
