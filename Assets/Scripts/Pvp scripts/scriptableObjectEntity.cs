using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "Characters/Entity")]
public class scriptableObjectEntity : ScriptableObject
{
    [Header("Spell settings")]
    public string entityName = "Entity Base";
    public float entityDMG = 1F;
    public float entityHaste = 1F;
}