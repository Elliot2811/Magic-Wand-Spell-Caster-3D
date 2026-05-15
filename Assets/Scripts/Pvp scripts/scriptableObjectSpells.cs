using UnityEngine;

[CreateAssetMenu(fileName = "Basic Spell", menuName = "Spells/BasicSpell")]
public class scriptableObjectSpells : ScriptableObject
{
    [Header("Spell settings")]
    public string spellName = "Input Spell Name";
    public string spellType = "Basic";
    public float spellDamage = 10.0F;
}