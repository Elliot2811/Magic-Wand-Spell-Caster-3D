using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Basic Spell", menuName = "Spells/Basic")]
public class ScriptableObjectSpell : ScriptableObject
{
    [Header("Spell settings")]
    public string spellName = "Input Spell Name";
    public int spellPriority = 0;
    public float spellDamage = 10.0f;
    public float spellSpeed = 5f;
    public float spellDelay = 0f;
    public bool shieldSpell = false;
    public bool destroyLowerTierSpells = false;
    //public string spellElement = "Input projSpell Element";
    //public ScriptableObjectStatusEffect spellChargeEffect = null;
    //public int spellEffectChance = 0;
    //public enum SpellType
    //{
    //    Small,
    //    Medium,
    //    Large
    //}
    //public SpellType spellType;

    [Header("Audio")]
    public AudioClip castSFX;
    [Range(0f, 1f)] public float castVolume = 0.5f;
    [Tooltip("Randomize pitch slightly so projSpell casted doesn't sound same all the time")]
    public bool randomizePitch = true;
    [Range(0f, 0.2f)] public float pitchVariance = 0.05f;

    public GameObject prefab;
    public GameObject spellChargeEffect;
}
