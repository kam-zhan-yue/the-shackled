using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayerHelper : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        _spriteRenderer.sortingOrder = (int)transform.position.y;
    }
}
