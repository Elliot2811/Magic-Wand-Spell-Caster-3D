using System;
using System.Collections;
using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    //#region Public Variables
    //public GameObject prefab;
    //public KeyCode inputKey;
    //public bool eventActivated = false;
    //#endregion

    //#region Protected Variables
    //protected GameObject spellSpawnPosAndRot;
    //protected bool entityAlive = true;
    //public float entityDmgTaken { get; private set; } = 0F;
    //protected float entityDMG = 1F;
    //protected float entityHaste = 1F;
    //#endregion

    //#region Entity Functions
    ////It does a countdown before it calls another function which fires actually fires the spell
    //protected virtual IEnumerator CastAndFireSpell(int timeCount)
    //{
    //    while (timeCount > 0)
    //    {
    //        Debug.Log(timeCount + "...");
    //        yield return new WaitForSeconds(1);
    //        timeCount--;
    //    }
    //    FireSummonedSpell();
    //}
    ////Instantiates the spell prefab along with the position
    //protected virtual void FireSummonedSpell()
    //{
    //    GameObject spell = Instantiate(prefab);
    //    var spellProjectileScript1 = spell.GetComponent<BasicBulletSpell>();
    //    spellProjectileScript1.SetProjectilePosAndRot(spellSpawnPosAndRot);
    //    Debug.Log($"Player launched spell");
    //}

    ////When projectile hits the player, it calls out this function which damages the players
    //public void TakeDamage(int amount)
    //{
    //    entityDmgTaken += amount;
    //    Debug.Log($"{gameObject} Total Damage Taken: {entityDmgTaken}");
    //}

    //public virtual void FireSpell(ScriptableObjectSpell spell)
    //{
    //    if (spell == null)
    //        return;

    //    prefab = spell.prefab;
    //    StartCoroutine(CastAndFireSpell(3));
    //}
    //#endregion

    public float EntityDmgTaken { get; private set; }
    public virtual void FireSpell(ScriptableObjectSpell spell, float delay = 0f, float damageMultiplier = 1f)
    {
        StartCoroutine(WaitFireSpell(spell, delay, damageMultiplier));
    }

    //original FireSpell(spell) body, with a multiplier applied to the projectile before launch
    public virtual void FireSpellWithMultiplier(ScriptableObjectSpell spell, float damageMultiplier)
    {
        if (spell == null || spell.prefab == null)
            return;
        GameObject proj = Instantiate(spell.prefab);
        proj.transform.localScale = GameConstants.ProjectileSpawn.scale;
        proj.transform.SetParent(transform);
        proj.transform.localPosition = GameConstants.ProjectileSpawn.relativePos;
        proj.transform.localRotation = GameConstants.ProjectileSpawn.relativeRotation;
        SpellProjHandler projHandler = proj.GetComponent<SpellProjHandler>();
        if (projHandler == null)
            projHandler = proj.AddComponent<SpellProjHandler>();
        projHandler.damageMultiplier = damageMultiplier;
        projHandler.Init(spell);
        AudioManager.Instance?.PlaySFX(spell.castSFX, spell.castVolume, spell.randomizePitch, spell.pitchVariance);
    }
    private IEnumerator WaitFireSpell(ScriptableObjectSpell spell, float delay, float damageMultiplier)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"[CharacterEntity] Firing '{spell.name}' � base damage {spell.spellDamage}, multiplier x{damageMultiplier}, expected damage {spell.spellDamage * damageMultiplier}");
        FireSpellWithMultiplier(spell, damageMultiplier);
    }

    public void TakeDamage(float damage)
    {
        EntityDmgTaken += damage;
        damageTakenMessage?.Invoke(damage);
        //Debug.Log($"Sending message of damage taken: {damage}");
    }
    public event Action<float> damageTakenMessage;
}