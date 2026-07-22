using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterEntity : MonoBehaviour
{
    public float EntityDmgTaken { get; private set; }
    public bool entityShielded = false;
    private bool canCastSpell = true;
    private bool chargingHeavySpell = false;
    
    private GameObject spellProj = null;
    public GameObject shieldGameObj = null;
    private GameObject spellEffectPrefab = null;
    private Coroutine spellCoroutine = null;
    
    private CharacterSpriteController spriteController;
    
    private void Awake()
    {
        spriteController = GetComponent<CharacterSpriteController>();

        if (spriteController == null)
            spriteController = GetComponentInChildren<CharacterSpriteController>();
    }
    public virtual void CastSpell(ScriptableObjectSpell spell, float damageMultiplier = 1f)  
    {
        if (spell == null || spell.prefab == null)
        {
            Debug.LogError("Spell or prefab is null!");
            return;
        }
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
            Vector3 direction = transform.position.x < 0
                ? Vector3.right
                : Vector3.left;
            spriteController?.PlayAttack();
            shieldGameObj = Instantiate(spell.prefab);
            shieldGameObj.transform.SetParent(transform);
            if (transform.position.x < 0)
            {
                shieldGameObj.transform.localPosition = GameConstants.ProjectileSpawn.shieldRelativePos;
            }
            else if (transform.position.x > 0)
            {
                shieldGameObj.transform.localPosition = new Vector3(-GameConstants.ProjectileSpawn.shieldRelativePos.x, GameConstants.ProjectileSpawn.shieldRelativePos.y, GameConstants.ProjectileSpawn.shieldRelativePos.z);
            }
            else
            {
                Debug.LogError("Warning - position of player = 0 so direcion cannot be determined");
            }
            shieldGameObj.transform.localScale = GameConstants.ProjectileSpawn.shieldScale * spell.spellScale;

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
            spellCoroutine = StartCoroutine(FireProjAfterDelay(spell, spell.spellDelay, damageMultiplier));
        }
        AudioManager.Instance?.PlaySFX(spell.castSFX, spell.castVolume, spell.randomizePitch, spell.pitchVariance);
    }

    private IEnumerator FireProjAfterDelay(ScriptableObjectSpell spell, float delay, float damageMultiplier = 1f)
    {
        spellEffectPrefab = null;

        if ((spell.spellChargeEffect != null) && (spell.destroyLowerTierSpells == true))
        {
            spellEffectPrefab = Instantiate(spell.spellChargeEffect);
            spellEffectPrefab.transform.localScale = GameConstants.ProjectileSpawn.spellChargeEffectScale;
            spellEffectPrefab.transform.localPosition = new Vector3(this.transform.position.x, 2f, this.transform.position.z);
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
        spriteController?.PlayAttack();
        //Firing of spell (works for all spell tiers 1,2 and 3)
        spellProj = Instantiate(spell.prefab);
        spellProj.transform.SetParent(transform);
        //If player left, cast spells from x = pos, right then -x = pos
        if (transform.position.x < 0)
        {
            spellProj.transform.localPosition = GameConstants.ProjectileSpawn.spellRelativePos;
        }
        else if (transform.position.x > 0)
        {
            spellProj.transform.localPosition = new Vector3(-GameConstants.ProjectileSpawn.spellRelativePos.x, GameConstants.ProjectileSpawn.spellRelativePos.y, GameConstants.ProjectileSpawn.spellRelativePos.z);
        }
        else
        {
            Debug.LogError("Warning - position of player = 0 so direcion cannot be determined");
        }
        spellProj.transform.localScale = GameConstants.ProjectileSpawn.spellScale * spell.spellScale;

        SpellProjHandler projHandler = spellProj.GetComponent<SpellProjHandler>();
        if (projHandler == null)
            projHandler = spellProj.AddComponent<SpellProjHandler>();
        projHandler.Init(spell, damageMultiplier);
        
        Debug.Log($"[CharacterEntity] Firing '{spell.name}' � base damage {spell.spellDamage}, multiplier x{damageMultiplier}, expected damage {spell.spellDamage * damageMultiplier}");
        
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

                //Reset code run by coroutine (*Rare case* precaution against case where some lines of coroutine's code are run)
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