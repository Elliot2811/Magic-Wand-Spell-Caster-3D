using UnityEngine;

public class SpellProjHandler : MonoBehaviour
{
    private bool initialized = false;

    private ScriptableObjectSpell spell;

    private float projectileSpeed;

    public float damageMultiplier = 1f;

    private Vector3 direction;

    private CharacterEntity owner;

    public void Init(ScriptableObjectSpell spell, Vector3 direction, CharacterEntity owner)
    {
        this.spell = spell;
        projectileSpeed = spell.spellSpeed;
        this.direction = direction.normalized;
        this.owner = owner;

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
        transform.position += direction * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER: " + other.name + " Tag: " + other.tag);

        if (!other.CompareTag("Player"))
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

            // Don't hit the player who fired this projectile
            if (character == owner)
                return;

            float damage = spell.spellDamage * damageMultiplier;
            Debug.Log($"[SpellProjHandler] '{spell.name}' hit {other.name} for {damage} (base {spell.spellDamage} x{damageMultiplier})");
            character.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Border"))
        {
            Debug.Log("Projectile missed!");
            Destroy(gameObject);

            return;
        }
    }
}