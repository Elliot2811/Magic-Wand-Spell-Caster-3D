using System;
using UnityEngine;

public class GameSceneInitialiser : MonoBehaviour
{
    #region Variables
    [Header("Game Object references")]
    //Prefabs
    public GameObject entityPrefab1;
    public GameObject entityPrefab2;
    public GameObject projectilePrefab;
    private GameObject leftPlayer;
    private GameObject rightPlayer;

    //Script References
    public GameScenarioSelector gameScenarioSelector;

    //[Header("Starting Game Position and Rotation")]
    //public Vector3 leftPlayerPosition = new Vector3(-5.5F, 1, 0);
    //public Quaternion leftPlayerRotation = Quaternion.Euler(0, 90, 0);
    //public Vector3 rightPlayerPosition = new Vector3(5.5F, 1, 0);
    //public Quaternion rightPlayerRotation = Quaternion.Euler(0,-90,0);

    //Overall Game flow variables
    public static event Action<GameRunStatus> StartMainGame;
    public static event Action StartMainMenu;
    private int gameScenario = 0; //1 means player at left side, 2 means player at right side, 3 means 2 players
    public enum GameRunStatus
    {
        MainMenu,
        GameStarted,
        GameEnded
    }
    #endregion

    #region Functions 

    #region Start Function
    private void Start()
    {
        StartMainMenu?.Invoke();
    }
    #endregion
    #region Scene Initialise Function
    public void StartGame(int gameScenarioSelected)
    {
        gameScenario = gameScenarioSelected;
        gameScenarioSelector.InputCheckSetFalse();
        StartMainGame?.Invoke(GameRunStatus.GameStarted);

        leftPlayer = Instantiate(
            entityPrefab1,
            GameConstants.LakeWorldLeftPos,
            Quaternion.Euler(GameConstants.LakeWorldLeftRot)
        );
        leftPlayer.transform.localScale = GameConstants.LakeWorldLeftScale;

        rightPlayer = Instantiate(
            entityPrefab2,
            GameConstants.LakeWorldRightPos,
            Quaternion.Euler(GameConstants.LakeWorldRightRot)
        );
        rightPlayer.transform.localScale = GameConstants.LakeWorldRightScale;

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
    #endregion

    #endregion
}