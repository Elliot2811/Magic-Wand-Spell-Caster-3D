//using UnityEngine;
//using UnityEngine.SceneManagement;

//[CreateAssetMenu(menuName = "States/GameplayState")]
//public class GameplayState : GameState
//{
//    [SerializeField] private VictoryState victoryState;
//    private MapData activeMapData;

//    public void SetMap(MapData chosenMap)
//    {
//        activeMapData = chosenMap;
//    }

//    public override void EnterState(GameStateManager gameManager)
//    {
//        if (activeMapData == null)
//        {
//            Debug.LogError("No Map Data set before entering GameplayState!");
//            return;
//        }

//        // Double-check to catch typos early
//        if (string.IsNullOrEmpty(activeMapData.sceneToLoad))
//        {
//            Debug.LogError($"[MapData Error] 'sceneToLoad' field is completely empty inside asset: {activeMapData.name}!");
//            return;
//        }

//        SceneManager.sceneLoaded += OnGameplaySceneLoaded;
//        SceneManager.LoadScene(activeMapData.sceneToLoad);

//        // Play Music
//        if (AudioManager.Instance != null)
//        {
//            AudioManager.Instance.PlayMusic(activeMapData.mapMusic);
//        }
//    }
//    private void OnGameplaySceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//        GameSceneInitialiser initialiser = FindFirstObjectByType<GameSceneInitialiser>();

//        if (initialiser != null)
//        {
//            initialiser.currentMapData = activeMapData; // Pass data
//            initialiser.StartGame(GameStateManager.Instance.gameScenario); // Start game!
//        }
//        SceneManager.sceneLoaded -= OnGameplaySceneLoaded;
//    }
//    public override void UpdateState(GameStateManager gameManager)
//    {
//        //PLACEHOLDER to decide which player wins
//        if (Input.GetKeyDown(KeyCode.Alpha1))
//        {
//            EndMatch(1,gameManager);
//        }
//        else if (Input.GetKeyDown(KeyCode.Alpha2))
//        {
//            EndMatch(2, gameManager);
//        }
//    }
//    public override void ExitState(GameStateManager gameManager)
//    {
//        //Stop battleMusic if exit Gameplay
//        if (AudioManager.Instance != null)
//        {
//            AudioManager.Instance.StopMusic();
//        }
//    }
//    public void EndMatch(int winningPlayerNumber, GameStateManager gameManager)
//    {
//        victoryState.SetWinner(winningPlayerNumber);
//        gameManager.TransitionToState(victoryState);
//    }
//}
