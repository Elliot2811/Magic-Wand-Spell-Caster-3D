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

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayState : GameState
{
    private MapData mapData;
    private CharacterEntity characterPrefab;

    private CharacterEntity leftCharacter;
    private CharacterEntity rightCharacter;

    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        if (SceneManager.GetActiveScene().name != "Gameplay")
            SceneManager.LoadScene("Gameplay");

        mapData = GameConstants.Instance.mapPresets[stateManager.mapIndex];
        characterPrefab = GameConstants.Instance.characterPrefab;


        stateManager.StartCoroutine(LoadGameplayObjects());
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }


    private IEnumerator LoadGameplayObjects()
    {
        yield return SceneManager.LoadSceneAsync("Gameplay");

        Debug.Log("Spawning Objects");

        MonoBehaviour.Instantiate(mapData.mapPrefab);

        leftCharacter = MonoBehaviour.Instantiate(characterPrefab);
        leftCharacter.transform.position = mapData.leftPos;
        leftCharacter.transform.rotation = Quaternion.Euler(mapData.leftRot);
        leftCharacter.transform.localScale = mapData.leftScale;

        rightCharacter = MonoBehaviour.Instantiate(characterPrefab);
        rightCharacter.transform.position = mapData.rightPos;
        rightCharacter.transform.rotation = Quaternion.Euler(mapData.rightRot);
        rightCharacter.transform.localScale = mapData.rightScale;
    }
}