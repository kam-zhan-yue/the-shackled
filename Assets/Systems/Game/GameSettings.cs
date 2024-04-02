using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameSettings")]
public class GameSettings : ScriptableObject
{
    public bool debug = false;
    public bool showIntroCinematic = true;
    public bool showIntroAnimation = true;
    public int initialRings = 10;
}