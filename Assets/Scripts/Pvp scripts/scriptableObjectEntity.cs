using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Characters/Entity")]
public class scriptableObjectEntity : ScriptableObject
{
    [Header("Spell settings")]
    public string spellName = "Entity Base";
    public float playerDMG = 1F;
    public float playerHaste = 1F;
}