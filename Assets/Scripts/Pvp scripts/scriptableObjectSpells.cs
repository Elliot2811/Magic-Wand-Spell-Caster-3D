using UnityEngine;

[CreateAssetMenu(fileName = "Basic Spell", menuName = "Spells/Basic/BasicSpell")]
public class ScriptableObjectSpells : ScriptableObject
{
    [Header("Spell settings")]
    public string spellName = "Input Spell Name";
    public string spellType = "Basic";
    public float spellDamage = 10.0F;

    public GameObject prefab;
}
