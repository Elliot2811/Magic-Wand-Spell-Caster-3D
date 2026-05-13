using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static BasicBulletSpell;
using static UnityEngine.GraphicsBuffer;

public abstract class EntityBase : MonoBehaviour
{
    #region Variables
    public GameObject prefab;
    public GameObject currentObject;
    public BasicBulletSpell projectileScript1;
    public enum playerID
    {
        playerLeft,
        playerRight,
        botLeft,
        botRight,
        none
    }
    public playerID playerIDCurrentSet;

    protected bool playerAlive = true;
    protected float playerHealth = 100.0F;
    protected int playerMana = 100;
    protected float playerDMG = 10.00F;
    protected float playerHaste = 10.00F;
    protected int playerManaRegenRate = 10;
    #endregion

    #region Entity Functions
    public void InitialisePlayerNBots()
    {
        switch (playerIDCurrentSet)
        {
            case playerID.none:
                gameObject.SetActive(false);
                break;
            case playerID.playerLeft:
                gameObject.SetActive(true);
                transform.position = new Vector3(-5.5F, 1, 0);
                transform.Rotate(0, 90F, 0);
                break;
            case playerID.botLeft:
                gameObject.SetActive(true);
                transform.position = new Vector3(-5.5F, 1, 0);
                transform.Rotate(0, 90F, 0);
                break;
            case playerID.playerRight:
                gameObject.SetActive(true);
                transform.position = new Vector3(5.5F, 1, 0);
                transform.Rotate(0, -90F, 0);
                break;
            case playerID.botRight:
                gameObject.SetActive(true);
                transform.position = new Vector3(5.5F, 1, 0);
                transform.Rotate(0, -90F, 0);
                break;

        }
    }
    public void SetPlayerID(playerID side)
    {
        playerIDCurrentSet = side;
    }
    protected virtual IEnumerator CastAndFireSpell(float timeCount)
    {
        while (timeCount > 0)
        {
            Debug.Log(timeCount + "...");
            yield return new WaitForSeconds(1);
            timeCount--;
        }
        FireSummonedSpell();
    }
    protected virtual void FireSummonedSpell()
    {
        GameObject spell = Instantiate(prefab);
        var spellProjectileScript1 = spell.GetComponent<BasicBulletSpell>();

        switch (playerIDCurrentSet)
        {
            case playerID.none:
                Debug.Log("Error - Player ID when firing spell is incorrect and is set to None");
                break;
            case playerID.playerLeft:
                spellProjectileScript1.SetOwner(currentObject);
                break;
            case playerID.botLeft:
                spellProjectileScript1.SetOwner(currentObject);
                break;
            case playerID.playerRight:
                spellProjectileScript1.SetOwner(currentObject);
                break;
            case playerID.botRight:
                spellProjectileScript1.SetOwner(currentObject);
                break;
        }
        Debug.Log($"{playerIDCurrentSet} launched spell");
    }

    protected void TakeDamage(int dmgTaken)
    {
        Debug.Log($"Mage {playerIDCurrentSet} lost {dmgTaken} health");
    }

    //protected void OnTriggerEnter(Collider other)
    //{
    //    var target = other.GetComponent<EntityBase>();
    //    if (target != null)
    //    {
    //        target.TakeDamage(20);
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Projectile"))
    //        return;
    //    //PlayerPVP.bulletHitPlayer1Event?.Invoke();
    //    projectileScript1.DestroyProjectile();
    //}
    #endregion
}