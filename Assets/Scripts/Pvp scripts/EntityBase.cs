using System.Collections;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    #region Public Variables
    public GameObject prefab;
    public GameObject spellSpawnPosAndRot;
    public enum playerID
    {
        playerLeft,
        playerRight,
        botLeft,
        botRight,
        none
    }
    public playerID playerIDCurrentSet;
    #endregion

    #region Protected Variables
    protected bool playerAlive = true;
    protected float playerDmgTaken = 0F;
    protected float playerDMG = 1F;
    protected float playerHaste = 1F;
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
                spellProjectileScript1.SetProjectilePosAndRot(spellSpawnPosAndRot);
                break;
            case playerID.botLeft:
                spellProjectileScript1.SetProjectilePosAndRot(spellSpawnPosAndRot);
                break;
            case playerID.playerRight:
                spellProjectileScript1.SetProjectilePosAndRot(spellSpawnPosAndRot);
                break;
            case playerID.botRight:
                spellProjectileScript1.SetProjectilePosAndRot(spellSpawnPosAndRot);
                break;
        }
        Debug.Log($"{playerIDCurrentSet} launched spell");
    }

    public void TakeDamage(int amount)
    {
        playerDmgTaken += amount;
        Debug.Log($"{gameObject} Total Damage Taken: {playerDmgTaken}");
    }
    #endregion
}