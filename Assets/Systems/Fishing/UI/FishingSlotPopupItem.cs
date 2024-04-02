using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishingSlotPopupItem : MonoBehaviour
{
    [SerializeField] private Image fill;
    
    private FishingSlot _data;

    public void Init(FishingSlot slot)
    {
        _data = slot;
        fill.fillAmount = _data.Size;
        float zRotation = 360f * (_data.start) * -1f;
        transform.eulerAngles = new Vector3(0f, 0f, zRotation);
        Debug.Log($"Rotation: {zRotation}");
    }
}
