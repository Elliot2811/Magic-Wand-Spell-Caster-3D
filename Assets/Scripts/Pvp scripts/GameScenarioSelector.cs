using System.Collections;
using UnityEngine;

public class GameScenarioSelector : MonoBehaviour
{
    #region Variables
    [Header("Game Object References")]
    public GameSceneInitialiser mainGameManager;

    [Header("Game Menu Settings")]
    public int gameStartTimer = 5;
    private bool allowInputCheck = false;
    private bool leftCoinSlotInsert = false;
    private bool rightCoinSlotInsert = false;
    private Coroutine gameCountdownCoroutine;
    #endregion

    #region Functions

    #region Event Functions
    private void OnEnable()
    {
        GameSceneInitialiser.StartMainMenu += InputCheckSetTrue;
    }

    private void OnDisable()
    {
        GameSceneInitialiser.StartMainMenu -= InputCheckSetTrue;
    }
    #endregion
    #region Update Function
    private void Update()
    {
        if (allowInputCheck)
        {
            CoinInsertInput();
        }
    }
    #endregion
    #region Coin Insert Logic to Initialise Game
    //These two functions are to control when to check for coin inputs "W" and "I"
    public void InputCheckSetTrue()
    {
        allowInputCheck = true;
    }
    public void InputCheckSetFalse()
    {
        allowInputCheck = false;
    }

    //Starts Countdown upon a coin input or press space to directly select game scenario
    private void CoinInsertInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            leftCoinSlotInsert = true;
            if (gameCountdownCoroutine == null)
            {
                gameCountdownCoroutine = StartCoroutine(GameStartCountdown());
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            rightCoinSlotInsert = true;
            if (gameCountdownCoroutine == null)
            {
                gameCountdownCoroutine = StartCoroutine(GameStartCountdown());
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && (gameCountdownCoroutine != null))
        {
            StopCoroutine(gameCountdownCoroutine);
            gameCountdownCoroutine = null;
            SelectGameScenario();
        }
    }

    //Countdown before selecting the game scenario
    private IEnumerator GameStartCountdown()
    {
        for (int i = gameStartTimer; i > 0; i--)
        {
            Debug.Log($"{i}!");
            yield return new WaitForSeconds(1);
        }
        SelectGameScenario();
    }

    //Choose the game scenario based on which side the player plays and the number of players
    private void SelectGameScenario()
    {
        if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == true))
        {
            mainGameManager.StartGame(3);
        }
        else if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == false))
        {
            mainGameManager.StartGame(1);
        }
        else if ((leftCoinSlotInsert == false) && (rightCoinSlotInsert == true))
        {
            mainGameManager.StartGame(2);
        }
        else
        {
            Debug.Log("Error -- Coin Insert if else statement logic is wrong");
        }
    }
    #endregion

    #endregion
}
