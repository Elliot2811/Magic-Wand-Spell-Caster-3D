using Unity.VisualScripting;
using UnityEngine;

public class SpellProjHandler : MonoBehaviour
{
    private bool initialized = false;

    public ScriptableObjectSpell spell;

    private float projectileSpeed;
    private int spellPriority;

    public void Init(ScriptableObjectSpell spell)
    {
        this.spell = spell;
        this.spellPriority = spell.spellPriority;
        projectileSpeed = spell.spellSpeed;

        if (spell == null)
        {
            Debug.LogError("Null Spell");
        }

        initialized = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Destroy(gameObject);
            return;
        }

        if (!initialized)
            return;

        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.transform.parent == other.transform)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            CharacterEntity character = other.GetComponent<CharacterEntity>();
            if (character == null)
            {
                Debug.LogError($"[SpellProjHandler]: No CharacterEntity script on Player tag object");
                return;
            }

            character.TakeDamage(spell.spellDamage);
            Destroy(gameObject);
            character.shieldGameObj = null;
            return;
        }
        else if (other.CompareTag("spell") && (spell.destroyLowerTierSpells == true))
        {
            SpellProjHandler otherProjHandler = other.GetComponent<SpellProjHandler>();
            ShieldSpellProjHandler otherShieldSpellHandler = other.GetComponent<ShieldSpellProjHandler>();
            if (otherShieldSpellHandler != null)
            {
                Debug.Log("Projectile destroyed shield");
            }
            else if (otherProjHandler == null)
            {
                Debug.LogWarning("Warning - Heavy spell unable to retrieve other projectile handler script component");
            }
            else if (otherProjHandler.spellPriority < this.spellPriority)
            {
                Destroy(other.gameObject);
            }
            else if (otherProjHandler.spellPriority == this.spellPriority)
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
    }
}