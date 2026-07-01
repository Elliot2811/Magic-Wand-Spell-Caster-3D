using UnityEngine;

[CreateAssetMenu(fileName = "Basic Spell", menuName = "Spells/Basic/BasicSpell")]
public class ScriptableObjectSpells : ScriptableObject
{
    [Header("Spell settings")]
    public string spellName = "Input Spell Name";
    public string spellType = "Basic";
    public float spellDamage = 10.0f;
    public float spellSpeed = 5f;

    [Header("Audio")]
    public AudioClip castSFX;
    [Range(0f, 1f)] public float castVolume = 1f;
    [Tooltip("Randomize pitch slightly so spell casted doesn't sound same all the time")]
    public bool randomizePitch = true;
    [Range(0f, 0.2f)] public float pitchVariance = 0.05f;

    public GameObject prefab;
}
