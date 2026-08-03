/* using UnityEngine;

// BROKEN
public class BotPVP : CharacterEntity
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
        selectRandNum();
    }
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timerInterval)
        {
            timer = 0f;

            FireSpell()
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
*/

using System;
using System.Collections.Generic;
using UnityEngine;

public class BotPVP : CharacterEntity
{

    public ShapesCollectionSO shapesCollection;
    private ShapeInfoSO shapeInfo;

    // Bot Settings
    [SerializeField]
    private float minBotTimer = 4f;
    [SerializeField]
    private float maxBotTimer = 8f;
    [SerializeField]
    private int missChance = 30;

    // Computer Timer Settings
    private float compTimer = 0;

    private void Start()
    {
        compTimer = UnityEngine.Random.Range(minBotTimer, maxBotTimer);
    }

    private void Update()
    {
        if (compTimer <= 0)
        {
            float finalAccuracy = GenerateRandomAccuracy();
            ShapeInfoSO finalShape = (finalAccuracy * 100 > missChance) ? GenerateRandomShape() : null;
            BotRandomShape?.Invoke(finalShape, finalAccuracy);
            Debug.Log("Bot is attempting to cast a spell");
            compTimer = UnityEngine.Random.Range(minBotTimer,maxBotTimer);
        }
        compTimer -= Time.deltaTime;
    }

    public void Init(ShapesCollectionSO shapesCollection)
    {
        this.shapesCollection = shapesCollection;
    }

    private ShapeInfoSO GenerateRandomShape()
    {
        int randomNum = UnityEngine.Random.Range(0, shapesCollection.Count);
        return shapesCollection.GetShapeInfoSO(randomNum);
    }

    private float GenerateRandomAccuracy()
    {
        string accuracyLevel;
        int randomPercentNum = UnityEngine.Random.Range(0, 100);
        float finalAccuracy;
        if (randomPercentNum <= missChance)
        {
            accuracyLevel = "miss";
        }
        else if (randomPercentNum <= 75)
        {
            accuracyLevel = "ok";
        }
        else if (randomPercentNum <= 95)
        {
            accuracyLevel = "great";
        }
        else
        {
            accuracyLevel = "perfect";
        }

        if (accuracyLevel == "miss")
        {
            finalAccuracy = 0;
        }
        else if (accuracyLevel == "ok")
        {
            finalAccuracy = Mathf.Round(UnityEngine.Random.Range(70f, 75f) * 10f) / 10f;
        }
        else if (accuracyLevel == "great")
        {
            finalAccuracy = Mathf.Round(UnityEngine.Random.Range(76f, 89f) * 10f) / 10f;
        }
        else if (accuracyLevel == "perfect")
        {
            finalAccuracy = Mathf.Round(UnityEngine.Random.Range(90f, 100f) * 10f) / 10f;
        }
        else
        {
            finalAccuracy = 0;
            Debug.LogWarning("[BotPVP] - random spell accruacy feature generator has a problem");
        }

        return finalAccuracy /= 100f;
    }

    public event Action<ShapeInfoSO, float> BotRandomShape;

}