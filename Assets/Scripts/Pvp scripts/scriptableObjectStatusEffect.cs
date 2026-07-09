using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Spells/StatusEffect")]
public class ScriptableObjectStatusEffect : ScriptableObject
{
    [Header("Spell settings")]
    public string statusEffectName = "Input Status Name";
    public float statusTimer = 0f;
    //public enum StatusEffect
    //{
    //    None,
    //    DmgBuff,
    //    HasteBuff,
    //    Heal,
    //    TickBasedDmg,
    //    Protected
    //}
    //public StatusEffect statusEffect;
}
