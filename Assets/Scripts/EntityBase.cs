using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase : MonoBehaviour
{
    #region Variables
    public int entityID;
    public bool playerAlive = true;
    public float playerHealth = 100.0F;
    public float playerDMG = 10.00F;
    public int playerMana = 100;
    public int playerManaRegenRate = 10;
    #endregion

    #region Player Functions
    protected void SummoningSpell()
    {
        Debug.Log("Spell Fired");
    }

    protected void SummonedSpellFired()
    {
        Debug.Log("Player launched spell");
    }

    protected void ReceiveDamage(int entityID, int dmgReceived)
    {
        Debug.Log($"Player {entityID} lost {dmgReceived} health");
    }
    #endregion
}