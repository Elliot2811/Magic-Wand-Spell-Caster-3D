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
    private bool chargingHeavySpell = false;
    private GameObject spellProj = null;
    public GameObject shieldGameObj = null;
    private GameObject spellEffectPrefab = null;
    private Coroutine spellCoroutine = null;

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
            spellCoroutine = StartCoroutine(FireProjAfterDelay(spell, spell.spellDelay));
        }
        AudioManager.Instance?.PlaySFX(spell.castSFX, spell.castVolume, spell.randomizePitch, spell.pitchVariance);
    }

    private IEnumerator FireProjAfterDelay(ScriptableObjectSpell spell, float delay)
    {
        spellEffectPrefab = null;

        if ((spell.spellChargeEffect != null) && (spell.destroyLowerTierSpells == true))
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
        if (delay > 0)
        {
            chargingHeavySpell = true;
        }
        yield return new WaitForSeconds(delay);
        if (spellEffectPrefab != null)
        {
            Destroy(spellEffectPrefab);
            spellEffectPrefab = null;
        }

        //Firing of spell (works for all spell tiers 1,2 and 3)
        spellProj = Instantiate(spell.prefab);
        spellProj.transform.SetParent(transform);
        spellProj.transform.localPosition = GameConstants.ProjectileSpawn.spellRelativePos;
        spellProj.transform.localRotation = GameConstants.ProjectileSpawn.relativeRotation;
        spellProj.transform.localScale = GameConstants.ProjectileSpawn.spellScale;

        SpellProjHandler projHandler = spellProj.GetComponent<SpellProjHandler>();
        if (projHandler == null)
            projHandler = spellProj.AddComponent<SpellProjHandler>();
        projHandler.Init(spell);

        canCastSpell = true;
        chargingHeavySpell = false;
        spellCoroutine = null;
        spellProj = null;
    }

    public void TakeDamage(float damage)
    {
        EntityDmgTaken += damage;
        damageTakenMessage?.Invoke(damage);
        if (chargingHeavySpell)
        {
            if (spellEffectPrefab != null)
            {
                Destroy(spellEffectPrefab);
                spellEffectPrefab = null;
            }
            if (spellCoroutine != null)
            {
                StopCoroutine(spellCoroutine);
                spellCoroutine = null;

                //Reset code run by coroutine (*Rare case* precaution against case where some lines of code are run)
                canCastSpell = true;
                chargingHeavySpell = false;
                if (spellProj != null)
                {
                    Destroy(spellProj);
                    spellProj = null;
                }
            }
            Debug.Log("Charge intercepted!");
        }
        //Debug.Log($"Sending message of damage taken: {damage}");
    }

    public event Action<float> damageTakenMessage;
}