using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kuroneko.UIDelivery;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FishingSlotPopupItem : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private PresetController presetController;
    private FishingSlot _data;

    public void Init(FishingSlot slot)
    {
        _data = slot;
        fill.fillAmount = _data.Size;
        float zRotation = 360f * (_data.start) * -1f;
        transform.eulerAngles = new Vector3(0f, 0f, zRotation);
        presetController.SetPresetById("reset");
        _data.OnResolve = null;
        _data.OnResolve += OnResolve;
    }

    private void OnResolve()
    {
        presetController.SetPresetById("resolve");
    }
}
