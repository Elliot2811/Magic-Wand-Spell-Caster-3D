using UnityEngine;

<<<<<<<< HEAD:Assets/Scripts/Pvp scripts/ScriptableObjectSpells.cs
[CreateAssetMenu(fileName = "Basic Spell", menuName = "Spells/Basic/BasicSpell")]
public class ScriptableObjectSpells : ScriptableObject
========
[CreateAssetMenu(fileName = "Basic Spell", menuName = "Spells/BasicSpell")]
public class scriptableObjectSpells : ScriptableObject
>>>>>>>> projectile-script:Assets/Scripts/Pvp scripts/scriptableObjectSpells.cs
{
    [Header("Spell settings")]
    public string spellName = "Input Spell Name";
    public string spellType = "Basic";
    public float spellDamage = 10.0F;
}