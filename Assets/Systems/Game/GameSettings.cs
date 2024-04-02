using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/GameSettings")]
public class GameSettings : ScriptableObject
{
    public bool showIntroCinematic = true;
    public bool showIntroAnimation = true;
    public int initialRings = 10;
}