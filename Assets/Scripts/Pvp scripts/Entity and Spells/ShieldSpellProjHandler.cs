using UnityEngine;

public class ShieldSpellProjHandler : MonoBehaviour
{
    private ScriptableObjectSpell spell;
    public void Init(ScriptableObjectSpell spell)
    {
        this.spell = spell;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"[Shield] OnTriggerEnter fired with {other.name}, tag={other.tag}");

        //return if collision isnt with a spell or the spell has the same parent as the shield
        if ((!other.CompareTag("spell")) || (other.transform.parent == transform.parent))
        {
            return;
        }

        SpellProjHandler otherProjHandler = other.GetComponent<SpellProjHandler>();
        CharacterEntity character = transform.parent.gameObject.GetComponent<CharacterEntity>();
        character.entityShielded = false;

        if (otherProjHandler.spell.spellPriority <= spell.spellPriority)
        {
            Destroy(other.gameObject);
            Debug.Log("Shield blocked projectile");
        }
        else
        {
            Debug.Log("Shield has been destroyed");
        }

        Destroy(gameObject);
        CombatFeedback.Instance?.PlayShieldBreak(transform.position, otherProjHandler.spell);
    }
}
