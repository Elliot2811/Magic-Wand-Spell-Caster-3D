using System;
using System.Collections;
using UnityEngine;

public class TempGameManager : MonoBehaviour
{
    #region Variables
    [Header("Game Object references")]
    public GameObject entityPrefab1;
    public GameObject entityPrefab2;
    public GameObject leftPlayer;
    public GameObject rightPlayer;
    public GameObject projectilePrefab;
    public GameScenarioSelector gameScenarioSelector;

    [Header("Starting Game Position and Rotation")]
    public Vector3 leftPlayerPosition = new Vector3(-5.5F, 1, 0);
    public Quaternion leftPlayerRotation = Quaternion.Euler(0, 90, 0);
    public Vector3 rightPlayerPosition = new Vector3(5.5F, 1, 0);
    public Quaternion rightPlayerRotation = Quaternion.Euler(0,-90,0);

    //Game Menu logic
    //[Header("Game Menu Settings")]
    //public int gameStartTimer = 5;
    //private bool leftCoinSlotInsert = false;
    //private bool rightCoinSlotInsert = false;
    //private bool gameStarted = false;
    //private Coroutine gameCountdownCoroutine;
    #endregion

    //Overall Game flow variables
    public static event Action<GameRunStatus> StartMainGame;
    public static event Action StartMainMenu;
    public enum GameRunStatus
    {
        MainMenu,
        GameStarted,
        GameEnded
    }

    private int gameScenario = 0; //1 means player at left side, 2 means player at right side, 3 means 2 players

    private void Start()
    {
        StartMainMenu?.Invoke();
    }

    private void Update()
    {
        //if (!gameStarted)
        //{
        //    if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        leftCoinSlotInsert = true;
        //        if (gameCountdownCoroutine == null)
        //        {
        //            gameCountdownCoroutine = StartCoroutine(GameStartCountdown());
        //        }
        //    }
        //    if (Input.GetKeyDown(KeyCode.I))
        //    {
        //        rightCoinSlotInsert = true;
        //        if (gameCountdownCoroutine == null)
        //        {
        //            gameCountdownCoroutine = StartCoroutine(GameStartCountdown());
        //        }
        //    }
        //    if (Input.GetKeyDown(KeyCode.Space) && (gameCountdownCoroutine != null))
        //    {
        //        StopCoroutine(gameCountdownCoroutine);
        //        gameCountdownCoroutine = null;
        //        StartGame();
        //    }
        //}
    }

    //private IEnumerator GameStartCountdown()
    //{
    //    for (int i = gameStartTimer; i > 0; i--)
    //    {
    //        Debug.Log($"{i}!");
    //        yield return new WaitForSeconds(1);
    //    }
    //    StartGame();
    //}

    //private void StartGame()
    //{
    //    if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == true))
    //    {
    //        gameScenario = 3;
    //    }
    //    else if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == false))
    //    {
    //        gameScenario = 1;
    //    }
    //    else if ((leftCoinSlotInsert == false) && (rightCoinSlotInsert == true))
    //    {
    //        gameScenario = 2;
    //    }
    //    else
    //    {
    //        Debug.Log("Error -- Coin Insert if else statement logic is wrong");
    //    }
    //    gameStarted = true;

    //    leftPlayer = Instantiate(
    //        entityPrefab1,
    //        leftPlayerPosition,
    //        leftPlayerRotation
    //    );
    //    rightPlayer = Instantiate(
    //        entityPrefab2,
    //        rightPlayerPosition,
    //        rightPlayerRotation
    //    );

    //    switch (gameScenario)
    //    {
    //        case 0:
    //            Debug.Log("Error -- Game Scenario variable has not been given a scenario");
    //            break;

    //        case 1:
    //            PlayerPVP playerScript1Left = leftPlayer.AddComponent<PlayerPVP>();
    //            BotPVP botScript1Right = rightPlayer.AddComponent<BotPVP>();
    //            playerScript1Left.prefab = projectilePrefab;
    //            botScript1Right.prefab = projectilePrefab;
    //            playerScript1Left.inputKey = KeyCode.W;
    //            break;
    //        case 2:
    //            BotPVP botScript2Left = leftPlayer.AddComponent<BotPVP>();
    //            PlayerPVP playerScript2Right = rightPlayer.AddComponent<PlayerPVP>();
    //            botScript2Left.prefab = projectilePrefab;
    //            playerScript2Right.prefab = projectilePrefab;
    //            playerScript2Right.inputKey = KeyCode.I;
    //            break;
    //        case 3:
    //            PlayerPVP playerScript3Left = leftPlayer.AddComponent<PlayerPVP>();
    //            PlayerPVP playerScript3Right = rightPlayer.AddComponent<PlayerPVP>();
    //            playerScript3Left.prefab = projectilePrefab;
    //            playerScript3Right.prefab = projectilePrefab;
    //            playerScript3Left.inputKey = KeyCode.W;
    //            playerScript3Right.inputKey = KeyCode.I;
    //            break;
    //    }
    //    StartMainGame?.Invoke(GameRunStatus.GameStarted);
    //}

    public void StartGame(int gameScenarioSelected)
    {
        gameScenario = gameScenarioSelected;
        gameScenarioSelector.InputCheckSetFalse();
        StartMainGame?.Invoke(GameRunStatus.GameStarted);

        leftPlayer = Instantiate(
            entityPrefab1,
            leftPlayerPosition,
            leftPlayerRotation
        );
        rightPlayer = Instantiate(
            entityPrefab2,
            rightPlayerPosition,
            rightPlayerRotation
        );

        switch (gameScenario)
        {
            case 0:
                Debug.Log("Error -- Game Scenario variable has not been given a scenario");
                break;

            case 1:
                PlayerPVP playerScript1Left = leftPlayer.AddComponent<PlayerPVP>();
                BotPVP botScript1Right = rightPlayer.AddComponent<BotPVP>();
                playerScript1Left.prefab = projectilePrefab;
                botScript1Right.prefab = projectilePrefab;
                playerScript1Left.inputKey = KeyCode.W;
                break;
            case 2:
                BotPVP botScript2Left = leftPlayer.AddComponent<BotPVP>();
                PlayerPVP playerScript2Right = rightPlayer.AddComponent<PlayerPVP>();
                botScript2Left.prefab = projectilePrefab;
                playerScript2Right.prefab = projectilePrefab;
                playerScript2Right.inputKey = KeyCode.I;
                break;
            case 3:
                PlayerPVP playerScript3Left = leftPlayer.AddComponent<PlayerPVP>();
                PlayerPVP playerScript3Right = rightPlayer.AddComponent<PlayerPVP>();
                playerScript3Left.prefab = projectilePrefab;
                playerScript3Right.prefab = projectilePrefab;
                playerScript3Left.inputKey = KeyCode.W;
                playerScript3Right.inputKey = KeyCode.I;
                break;
        }
    }
}
