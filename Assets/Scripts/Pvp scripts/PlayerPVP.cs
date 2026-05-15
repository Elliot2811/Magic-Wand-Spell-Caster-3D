using UnityEngine;

public class PlayerPVP : EntityBase
{
    //public static event Action bulletHitPlayer1Event;

    void Start()
    {
        //bulletHitPlayer1Event += bulletHitPlayer1Event;
        //Gets a reference to the child of the current player to get its position and rotation for spawning future spells
        spellSpawnPosAndRot = transform.GetChild(0).gameObject;
    }
    void Update()
    {
        //Developer controls where pressing "W" and "I" button summons a spell for the left and right player respectively
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
}