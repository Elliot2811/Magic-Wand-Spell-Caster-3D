using UnityEngine;

public class PlayerPVP : EntityBase
{
    private scriptableObjectEntity playerScriptableObject;

    void Start()
    {
        // Getting the transform from the empty game object ,which is a child of player, for the spells
        spellSpawnPosAndRot = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(inputKey) && (entityAlive) && (eventActivated == false))
        {
            //StartCoroutine(CastAndFireSpell(3));
            FireSummonedSpell();
        }
    }
}