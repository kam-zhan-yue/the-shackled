using System.Collections;
using System.Collections.Generic;
using Kuroneko.UIDelivery;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

public class CastPopup : Popup
{
    public RectTransform fill;
    
    protected override void InitPopup()
    {
        
    }

    public void OnCastMultiplierChanged(FloatPair floatPair)
    {
        Vector3 localScale = fill.localScale;
        localScale = new Vector3(localScale.x, floatPair.Item2, localScale.z);
        fill.localScale = localScale;
    }
}
