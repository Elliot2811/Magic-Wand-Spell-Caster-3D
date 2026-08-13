using Unity.VisualScripting;
using UnityEngine;

public class SpellProjHandler : MonoBehaviour
{
    private bool initialized = false;
    private float projectileSpeed = 0;
    private int spellPriority;
    public float damageMultiplier = 1f;
    private Vector3 normalizedDirection;
    public ScriptableObjectSpell spell;

    public void Init(ScriptableObjectSpell spell, float damageMultiplier = 1f)
    {
        this.spell = spell;
        projectileSpeed = spell.spellSpeed;
        this.spellPriority = spell.spellPriority;
        this.damageMultiplier = damageMultiplier;

        // Determine direction of projectile
        Vector3 direction = transform.position.x < 0
            ? Vector3.right
            : Vector3.left;
        normalizedDirection = direction.normalized;

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

        //transform.position += transform.forward * projectileSpeed * Time.deltaTime;
        //Vector3 direction = transform.position.x < 0 ? Vector3.right : Vector3.left;
        transform.position += normalizedDirection * projectileSpeed * Time.deltaTime;
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

            float damage = spell.spellDamage * damageMultiplier;
            Debug.Log($"[SpellProjHandler] '{spell.name}' hit {other.name} for {damage} (base {spell.spellDamage} x{damageMultiplier})");
            character.TakeDamage(damage);
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