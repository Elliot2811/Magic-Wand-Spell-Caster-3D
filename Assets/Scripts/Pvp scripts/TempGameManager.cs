using System.Collections;
using UnityEngine;

public class TempGameManager : MonoBehaviour
{
    [Header("Game Object references")]
    public GameObject entityPrefab1;
    public GameObject entityPrefab2;
    public GameObject leftPlayer;
    public GameObject rightPlayer;
    public int gameScenario = 0; //1 means player at left side, 2 means player at right side, 3 means 2 players
    public int gameStartTimer = 5;

    [Header("Starting Game Position and Rotation")]
    public Vector3 leftPlayerPosition = new Vector3(-5.5F, 1, 0);
    public Quaternion leftPlayerRotation = Quaternion.Euler(0, 90, 0);
    public Vector3 rightPlayerPosition = new Vector3(5.5F, 1, 0);
    public Quaternion rightPlayerRotation = Quaternion.Euler(0,-90,0);

    private bool leftCoinSlotInsert = false;
    private bool rightCoinSlotInsert = false;
    private bool gameStarted = false;
    private Coroutine gameCountdownCoroutine;

    private void Update()
    {
        if (!gameStarted)
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
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopCoroutine(gameCountdownCoroutine);
            gameCountdownCoroutine = null;
            CheckAndPlayScenario();
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    scenarioChecked = true;
        //    checkScenario();
        //    //Setting each script to their roles (enum values in the script) which defines whether they are playing and their position
        //    //leftPlayerScript = leftPlayer.GetComponent<PlayerPVP>();
        //    //rightPlayerScript = rightPlayer.GetComponent<PlayerPVP>();
        //    //player3Script = player3.GetComponent<PlayerPVP>();
        //    //player4Script = player4.GetComponent<PlayerPVP>();

        //    //SetPlayerScriptPosition();
        //}
    }

    //private void SetPlayerScriptPosition()
    //{
    //    //Set player position in world and code
    //    leftPlayerScript.playerIDCurrentSet = EntityBase.playerID.playerLeft;
    //    rightPlayerScript.playerIDCurrentSet = EntityBase.playerID.playerRight;
    //    player3Script.playerIDCurrentSet = EntityBase.playerID.none;
    //    player4Script.playerIDCurrentSet = EntityBase.playerID.none;
    //    leftPlayerScript.InitialisePlayerNBots();
    //    rightPlayerScript.InitialisePlayerNBots();
    //    player3Script.InitialisePlayerNBots();
    //    player4Script.InitialisePlayerNBots();
    //    Debug.Log("Finished setting the players");
    //}

    private IEnumerator GameStartCountdown()
    {
        for (int i = gameStartTimer; i > 0; i--)
        {
            Debug.Log($"{i}!");
            yield return new WaitForSeconds(1);
        }
        CheckAndPlayScenario();
    }

    private void CheckAndPlayScenario()
    {
        if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == true))
        {
            gameScenario = 3;
        }
        else if ((leftCoinSlotInsert == true) && (rightCoinSlotInsert == false))
        {
            gameScenario = 1;
        }
        else if ((leftCoinSlotInsert == false) && (rightCoinSlotInsert == true))
        {
            gameScenario = 2;
        }
        else
        {
            Debug.Log("Error -- Coin Insert if else statement logic is wrong");
        }
        gameStarted = true;
        switch (gameScenario)
            {
                case 0:
                    Debug.Log("Error -- Game Scenario variable has not been given a scenario");
                    break;

                case 1:
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
                    leftPlayer.AddComponent<PlayerPVP>();
                    rightPlayer.AddComponent<BotPVP>();
                    break;
                case 2:
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
                    leftPlayer.AddComponent<BotPVP>();
                    rightPlayer.AddComponent<PlayerPVP>();
                    break;
                case 3:
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
                    leftPlayer.AddComponent<PlayerPVP>();
                    rightPlayer.AddComponent<PlayerPVP>();
                    break;
            }
    }
}
