using System.Collections;
using UnityEngine;

public class BotPVP : EntityBase
{
    #region Variables
    [Header("Bot Settings")]
    public float minTimer = 3F;
    public float maxTimer = 10F;
    public float timer = 0F;
    private float timerInterval;
    #endregion

    #region Functions

    #region Start/Update Functions
    void Start()
    {
        //Gets a reference to the child of the current player to get its position and rotation for spawning future spells
        Debug.Log("This is a bot script!!!");
        //spellSpawnPosAndRot = transform.GetChild(0).gameObject;
        spellSpawnPosAndRot = transform.Find("SpellPosition").gameObject;
        selectRandNum();
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timerInterval)
        {
            timer = 0f;

            StartCoroutine(CastAndFireSpell(3));
            selectRandNum();
        }
    }
    #endregion
    #region Bot Functions
    private void selectRandNum()
    {
        timerInterval = Random.Range(minTimer, maxTimer);
        Debug.Log("Spell will take " + timerInterval);
    }
    #endregion

    #endregion
}