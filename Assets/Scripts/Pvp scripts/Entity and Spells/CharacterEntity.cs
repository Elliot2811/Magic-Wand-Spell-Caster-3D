using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
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

    //public virtual void FireSpell(ScriptableObjectSpells spell)
    //{
    //    if (spell == null)
    //        return;

    //    prefab = spell.prefab;
    //    StartCoroutine(CastAndFireSpell(3));
    //}
    //#endregion

    public float EntityDmgTaken { get; private set; }
    public bool entityShielded = false;
    private bool canCastSpell = true;
    private GameObject shieldGameObj;

    public virtual void FireSpell(ScriptableObjectSpell spell)
    {
        if (spell == null || spell.prefab == null)
            return;
        if (!canCastSpell)
        {
            return;
        }
        if (spell.shieldSpell == true)
        {
            if (entityShielded == true)
            {
                return;
            }
            shieldGameObj = Instantiate(spell.prefab);
            shieldGameObj.transform.SetParent(transform);
            shieldGameObj.transform.localPosition = GameConstants.ProjectileSpawn.shieldRelativePos;
            shieldGameObj.transform.localRotation = GameConstants.ProjectileSpawn.relativeRotation;
            shieldGameObj.transform.localScale = GameConstants.ProjectileSpawn.shieldScale;

            ShieldSpellProjHandler shieldSpellProjHandler = shieldGameObj.GetComponent<ShieldSpellProjHandler>();
            if (shieldSpellProjHandler == null)
            {
                shieldSpellProjHandler = shieldGameObj.AddComponent<ShieldSpellProjHandler>();
            }
            shieldSpellProjHandler.Init(spell);
            entityShielded = true;
        }
        else
        {
            StartCoroutine(FireSpellAfterDelay(spell, spell.spellDelay));
        }
        AudioManager.Instance?.PlaySFX(spell.castSFX, spell.castVolume, spell.randomizePitch, spell.pitchVariance);
    }

    private IEnumerator FireSpellAfterDelay(ScriptableObjectSpell spell, float delay)
    {
        GameObject spellEffectPrefab = null;

        if ((spell.spellChargeEffect != null) && (spell.destroyLowerTierSpells))
        {
            spellEffectPrefab = Instantiate(spell.spellChargeEffect);
            spellEffectPrefab.transform.localScale = this.transform.localScale;
            spellEffectPrefab.transform.localPosition = this.transform.position;
        }
        canCastSpell = false;
        if (entityShielded)
        {
            entityShielded = !entityShielded;
            Destroy(shieldGameObj);

        }
        yield return new WaitForSeconds(delay);
        canCastSpell = true;
        if (spellEffectPrefab != null)
        {
            Destroy(spellEffectPrefab);
            spellEffectPrefab = null;
        }
        GameObject proj = Instantiate(spell.prefab);
        proj.transform.SetParent(transform);
        proj.transform.localPosition = GameConstants.ProjectileSpawn.spellRelativePos;
        proj.transform.localRotation = GameConstants.ProjectileSpawn.relativeRotation;
        proj.transform.localScale = GameConstants.ProjectileSpawn.spellScale;

        SpellProjHandler projHandler = proj.GetComponent<SpellProjHandler>();
        if (projHandler == null)
            projHandler = proj.AddComponent<SpellProjHandler>();
        projHandler.Init(spell);

    }

    public void TakeDamage(float damage)
    {
        EntityDmgTaken += damage;

        damageTakenMessage?.Invoke(damage);

        //Debug.Log($"Sending message of damage taken: {damage}");
    }

    public event Action<float> damageTakenMessage;
}