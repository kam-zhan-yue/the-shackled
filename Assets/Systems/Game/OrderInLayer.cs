using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderInLayer : MonoBehaviour
{   
    private SpriteRenderer sprite_renderer;
    [SerializeField] private int offset=0;
    private void Awake() 
    {
        sprite_renderer=GetComponent<SpriteRenderer>();
        
    }
    private void LateUpdate() 
    {
        sprite_renderer.sortingOrder=(Mathf.RoundToInt(-100*transform.position.y)+offset);
        
    }

    public void SetOffset(int _offset)
    {
        offset=_offset;
    }
}
