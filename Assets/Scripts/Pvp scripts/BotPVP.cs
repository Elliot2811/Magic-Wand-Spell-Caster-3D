using System.Collections;
using UnityEngine;

public class BotPVP : EntityBase
{
    public float minTimer = 3F;
    public float maxTimer = 10F;
    public float timer = 0F;

    //DO NOT MANUALLY EDIT
    private float timerInterval;

    void Start()
    {
        //bulletHitPlayer1Event += bulletHitPlayer1Event;
        //Gets a reference to the child of the current player to get its position and rotation for spawning future spells
        Debug.Log("This is a bot script!!!");
        spellSpawnPosAndRot = transform.GetChild(0).gameObject;
        //selectRandNum();
    }
    //void Update()
    //{
    //    timer += Time.deltaTime;

    //    if (timer >= timerInterval)
    //    {
    //        timer = 0f;

    //        StartCoroutine(CastAndFireSpell(3.0F));
    //        selectRandNum();
    //    }
    //}

    //private void selectRandNum()
    //{
    //    timer = Random.Range(minTimer, maxTimer);
    //}
}