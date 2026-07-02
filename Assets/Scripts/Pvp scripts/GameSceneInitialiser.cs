//using System;
//using UnityEngine;

//public class GameSceneInitialiser : MonoBehaviour
//{
//    #region Variables
//    [Header("Game Object references")]
//    //Prefabs
//    public GameObject entityPrefab1;
//    public GameObject entityPrefab2;
//    public GameObject projectilePrefab;
//    private GameObject leftPlayer;
//    private GameObject rightPlayer;

//    [Header("UI References")]
//    public HealthDisplay healthDisplayUI;

//    public MapData currentMapData;
//    private bool hasGameStarted = false;

//    //Script References
//    //public GameScenarioSelector gameScenarioSelector;

//    //[Header("Starting Game Position and Rotation")]
//    //public Vector3 leftPlayerPosition = new Vector3(-5.5F, 1, 0);
//    //public Quaternion leftPlayerRotation = Quaternion.Euler(0, 90, 0);
//    //public Vector3 rightPlayerPosition = new Vector3(5.5F, 1, 0);
//    //public Quaternion rightPlayerRotation = Quaternion.Euler(0,-90,0);

//    //Overall Game flow variables
//    public static event Action<GameRunStatus> StartMainGame;
//    public static event Action StartMainMenu;
//    //private int gameScenario = 0; //1 means player at left side, 2 means player at right side, 3 means 2 players
//    public enum GameRunStatus
//    {
//        MainMenu,
//        GameStarted,
//        GameEnded
//    }
//    #endregion

//    #region Functions 

//    #region Start Function
//    private void Start()
//    {
//        if (GameStateManager.Instance == null)
//        {
//            Debug.LogError("ERROR - GameStateManager doesn't exist in current scene");
//        }
//        //StartMainMenu?.Invoke();
//        StartGame(GameStateManager.Instance.gameScenario);

//        //assigning which entity is which player is which for Healthbar UI to track health
//        CharacterEntity leftEntity = leftPlayer.GetComponent<CharacterEntity>();
//        CharacterEntity rightEntity = rightPlayer.GetComponent<CharacterEntity>();

//        if (healthDisplayUI != null && leftEntity != null && rightEntity != null)
//        {
//            //pass entities over to the UI!
//            healthDisplayUI.InitializePlayers(leftEntity, rightEntity);
//        }
//        else
//        {
//            Debug.LogError("Failed to link players to HealthDisplay UI!");
//        }
//    }
//    #endregion
//    #region Scene Initialise Function
//    public void StartGame(int gameScenarioSelected)
//    {
//        if (hasGameStarted) return;
//        hasGameStarted = true;
//        //gameScenarioSelector.InputCheckSetFalse();
//        Debug.Log("Starting game...");
//        StartMainGame?.Invoke(GameRunStatus.GameStarted);

//        if (currentMapData == null)
//        {
//            Debug.LogError("Current Map Data asset is missing from GameSceneInitialiser");
//            return;
//        }

//        if (currentMapData.mapPrefab != null)
//        {
//            Instantiate(currentMapData.mapPrefab, currentMapData.mapPrefab.transform.position, currentMapData.mapPrefab.transform.rotation);
//        }
//        else
//        {
//            Debug.LogWarning($"No mapPrefab has been assigned inside the {currentMapData.mapName} data asset!");
//        }

//        leftPlayer = Instantiate(
//            entityPrefab1,
//            currentMapData.leftPos,
//            Quaternion.Euler(currentMapData.leftRot)
//        );
//        leftPlayer.transform.localScale = currentMapData.leftScale;

//        rightPlayer = Instantiate(
//            entityPrefab2,
//            currentMapData.rightPos,
//            Quaternion.Euler(currentMapData.rightRot)
//        );
//        rightPlayer.transform.localScale = currentMapData.rightScale;

//        switch (gameScenarioSelected)
//        {
//            //case 0:
//            //    Debug.Log("Error -- Game Scenario variable has not been given a scenario");
//            //    break;

//            //case 1:
//            //    PlayerPVP playerScript1Left = leftPlayer.AddComponent<PlayerPVP>();
//            //    BotPVP botScript1Right = rightPlayer.AddComponent<BotPVP>();
//            //    playerScript1Left.prefab = projectilePrefab;
//            //    botScript1Right.prefab = projectilePrefab;
//            //    playerScript1Left.inputKey = KeyCode.W;
//            //    break;
//            //case 2:
//            //    BotPVP botScript2Left = leftPlayer.AddComponent<BotPVP>();
//            //    PlayerPVP playerScript2Right = rightPlayer.AddComponent<PlayerPVP>();
//            //    botScript2Left.prefab = projectilePrefab;
//            //    playerScript2Right.prefab = projectilePrefab;
//            //    playerScript2Right.inputKey = KeyCode.I;
//            //    break;
//            //case 3:
//            //    PlayerPVP playerScript3Left = leftPlayer.AddComponent<PlayerPVP>();
//            //    PlayerPVP playerScript3Right = rightPlayer.AddComponent<PlayerPVP>();
//            //    playerScript3Left.prefab = projectilePrefab;
//            //    playerScript3Right.prefab = projectilePrefab;
//            //    playerScript3Left.inputKey = KeyCode.W;
//            //    playerScript3Right.inputKey = KeyCode.I;
//            //    break;
//        }
//    }
//    #endregion

//    #endregion
//}