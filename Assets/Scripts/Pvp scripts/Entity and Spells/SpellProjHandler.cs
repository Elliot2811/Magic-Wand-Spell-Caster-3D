using Unity.VisualScripting;
using UnityEngine;

public class SpellProjHandler : MonoBehaviour
{
    private bool initialized = false;

    public ScriptableObjectSpell spell;

    private float projectileSpeed;

    public void Init(ScriptableObjectSpell spell)
    {
        this.spell = spell;
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
            return;
        }
        else if (other.CompareTag("spell") && (spell.destroyLowerTierSpells == true))
        {
            SpellProjHandler otherProjHandler = other.GetComponent<SpellProjHandler>();
            if (otherProjHandler.spell.spellPriority < spell.spellPriority)
            {
                Destroy(other.gameObject);
            }
            else if (otherProjHandler.spell.spellPriority == spell.spellPriority)
            {
                Destroy(other.gameObject);
                Destroy(gameObject);
            }
        }
    }
}