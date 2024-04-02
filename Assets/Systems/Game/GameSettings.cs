using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameSettings")]
public class GameSettings : ScriptableObject
{
    public bool debug = false;
    public bool showIntroCinematic = true;
    public bool showIntroAnimation = true;
    public int initialRings = 10;
    public float zoomOut = 100f;
    
    [Header("Old God Constants")]
    public float maxZoom = 100;
    public float maxScale = 15;
    public float endlessScale = 10;
}