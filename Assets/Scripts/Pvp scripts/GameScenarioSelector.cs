using System.Collections;
using TMPro;
using UnityEngine;

public class GameScenarioSelector : MonoBehaviour
{
    #region Variables
    [Header("Game Menu Settings")]
    public int gameStartTimer = 5;
    private bool allowInputCheck = true;
    private bool leftCoinSlotInsert = false;
    private bool rightCoinSlotInsert = false;
    private Coroutine gameCountdownCoroutine;
    [SerializeField] private GameState MapSelectionState;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI leftSlotStatusText;
    [SerializeField] private TextMeshProUGUI rightSlotStatusText;
    #endregion

    #region Functions
    #region Start and Update Function
    private void Start()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("ERROR - GameStateManager doesn't exist in current scene.");
        }

        //Initialize the UI text
        UpdateStatusUI();
        if (countdownText != null) countdownText.gameObject.SetActive(false);
    }
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
        if (Input.GetKeyDown(KeyCode.W) && leftCoinSlotInsert == false)
        {
            Debug.Log("Left Coin has been inserted");
            leftCoinSlotInsert = true;
            UpdateStatusUI(); //Update UI text

            if (gameCountdownCoroutine == null)
            {
                gameCountdownCoroutine = StartCoroutine(GameStartCountdown());
            }
        }
        if (Input.GetKeyDown(KeyCode.I) && rightCoinSlotInsert == false)
        {
            Debug.Log("Right Coin has been inserted");
            rightCoinSlotInsert = true;
            UpdateStatusUI(); //Update UI text

            if (gameCountdownCoroutine == null)
            {
                gameCountdownCoroutine = StartCoroutine(GameStartCountdown());
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && (gameCountdownCoroutine != null))
        {
            StopCoroutine(gameCountdownCoroutine);
            gameCountdownCoroutine = null;
            if (countdownText != null) countdownText.gameObject.SetActive(false); //Hide UI
            SelectGameScenario();
        }
    }

    //Countdown before selecting the game scenario
    private IEnumerator GameStartCountdown()
    {
        //Show countdown UI if it exists
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        for (int i = gameStartTimer; i > 0; i--)
        {
            Debug.Log($"{i}!");

            if (countdownText != null)
            {
                countdownText.text = i.ToString();
            }

            yield return new WaitForSeconds(1);
        }

        if (countdownText != null) countdownText.gameObject.SetActive(false);
        SelectGameScenario();
    }
    private void UpdateStatusUI()
    {
        if (leftSlotStatusText != null)
        {
            leftSlotStatusText.text = leftCoinSlotInsert ? "P1 READY" : "INSERT COIN";
            leftSlotStatusText.color = leftCoinSlotInsert ? Color.green : Color.white;
        }

        if (rightSlotStatusText != null)
        {
            rightSlotStatusText.text = rightCoinSlotInsert ? "P2 READY" : "INSERT COIN";
            rightSlotStatusText.color = rightCoinSlotInsert ? Color.green : Color.white;
        }
    }

    //Choose the game scenario based on which side the player plays and the number of players
    private void SelectGameScenario()
    {
        if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == true))
        {
            //gameSceneInitialiser.StartGame(3);
            GameStateManager.Instance.gameScenarioChosen = 3;
            GameStateManager.Instance.TransitionToState(MapSelectionState);
        }
        else if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == false))
        {
            //gameSceneInitialiser.StartGame(1);
            GameStateManager.Instance.gameScenarioChosen = 1;
            GameStateManager.Instance.TransitionToState(MapSelectionState);
        }
        else if ((leftCoinSlotInsert == false) && (rightCoinSlotInsert == true))
        {
            //gameSceneInitialiser.StartGame(2);
            GameStateManager.Instance.gameScenarioChosen = 2;
            GameStateManager.Instance.TransitionToState(MapSelectionState);
        }
        else
        {
            Debug.Log("Error -- Coin Insert if else statement logic is wrong");
        }
    }
    #endregion

    #endregion
}
