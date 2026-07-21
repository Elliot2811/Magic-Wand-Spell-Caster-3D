using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
        //return if collision isnt with a projSpell or the projSpell has the same parent as the shield
        if ((!other.CompareTag("projSpell")) || (other.transform.parent == transform.parent))
        {
            return;
        }

        SpellProjHandler otherProjHandler = other.GetComponent<SpellProjHandler>();
        CharacterEntity character = transform.parent.gameObject.GetComponent<CharacterEntity>();
        if (otherProjHandler.projSpell.spellPriority <= spell.spellPriority)
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
