using UnityEngine;

public class PlayerPVP : EntityBase
{
    private scriptableObjectEntity playerScriptableObject;
    private CharacterSpriteController spriteController; //connect to sprite control

    void Start()
    {
        // Getting the transform from the empty game object ,which is a child of player, for the spells
        //spellSpawnPosAndRot = transform.GetChild(0).gameObject;
        spellSpawnPosAndRot = transform.Find("SpellPosition").gameObject;
        spriteController = GetComponent<CharacterSpriteController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(inputKey) && (entityAlive) && (eventActivated == false))
        {
            //StartCoroutine(CastAndFireSpell(3));
            spriteController.PlayAttack();
            FireSummonedSpell();
        }
    }
}