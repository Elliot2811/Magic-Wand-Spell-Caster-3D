using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ShieldSpellProjHandler : MonoBehaviour
{
    private Vector3 normalizedDirection;
    private ScriptableObjectSpell spell;
    public void Init(ScriptableObjectSpell spell)
    {
        this.spell = spell;
        // Determine direction of shield
        Vector3 direction = transform.position.x < 0
            ? Vector3.right
            : Vector3.left;
        normalizedDirection = direction.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        //return if collision isnt with a spell or the spell has the same parent as the shield
        if ((!other.CompareTag("spell")) || (other.transform.parent == transform.parent))
        {
            return;
        }

        SpellProjHandler otherProjHandler = other.GetComponent<SpellProjHandler>();
        CharacterEntity character = transform.parent.gameObject.GetComponent<CharacterEntity>();
        if (otherProjHandler.spell.spellPriority <= spell.spellPriority)
        {
            character.entityShielded = false;
            Destroy(other.gameObject);
            Destroy(gameObject);
            Debug.Log("Shield blocked projectile");
            return;
        }
        else
        {
            character.entityShielded = false;
            Destroy(gameObject);
            Debug.Log("Shield has been destroyed");
            return;
        }
    }
}
