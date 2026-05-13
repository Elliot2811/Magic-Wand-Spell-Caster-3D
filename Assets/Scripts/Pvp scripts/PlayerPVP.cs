using UnityEngine;

public class PlayerPVP : EntityBase
{
    //public static event Action bulletHitPlayer1Event;

    void Start()
    {
        //bulletHitPlayer1Event += bulletHitPlayer1Event;
        currentObject = transform.GetChild(0).gameObject;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && (playerIDCurrentSet == playerID.playerLeft))
        {
            Debug.Log($"{playerIDCurrentSet} pressed the W key!");
            StartCoroutine(CastAndFireSpell(3.0F));
        }
        else if (Input.GetKeyDown(KeyCode.I) && (playerIDCurrentSet == playerID.playerRight))
        {
            Debug.Log($"{playerIDCurrentSet} pressed the I key!");
            StartCoroutine(CastAndFireSpell(3.0F));
        }
    }

    void bulletHitPlayer1()
    {
        TakeDamage(20);
    }
}