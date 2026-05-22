using System.Collections;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    #region Public Variables
    public GameObject prefab;
    public KeyCode inputKey;
    public bool eventActivated = false;
    #endregion

    #region Protected Variables
    protected GameObject spellSpawnPosAndRot;
    protected bool entityAlive = true;
    protected float entityDmgTaken = 0F;
    protected float entityDMG = 1F;
    protected float entityHaste = 1F;
    #endregion

    #region Entity Functions
    //It does a countdown before it calls another function which fires actually fires the spell
    protected virtual IEnumerator CastAndFireSpell(int timeCount)
    {
        while (timeCount > 0)
        {
            Debug.Log(timeCount + "...");
            yield return new WaitForSeconds(1);
            timeCount--;
        }
        FireSummonedSpell();
    }
    //Instantiates the spell prefab along with the position
    protected virtual void FireSummonedSpell()
    {
        GameObject spell = Instantiate(prefab);
        var spellProjectileScript1 = spell.GetComponent<BasicBulletSpell>();
        spellProjectileScript1.SetProjectilePosAndRot(spellSpawnPosAndRot);
        Debug.Log($"Player launched spell");
    }

    //When projectile hits the player, it calls out this function which damages the players
    public void TakeDamage(int amount)
    {
        entityDmgTaken += amount;
        Debug.Log($"{gameObject} Total Damage Taken: {entityDmgTaken}");
    }
    #endregion
}