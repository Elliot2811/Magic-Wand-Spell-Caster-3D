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

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayState : GameState
{
    private MapData mapData;
    private CharacterEntity characterPrefab;

    public CharacterEntity leftCharacter { get; private set; }
    public CharacterEntity rightCharacter { get; private set; }

    public bool timerRunning = false;
    public float timer = 100;
    public float displayPercentage = 0.5f;

    private SpellBook leftSpellBook;
    private SpellBook rightSpellBook;

    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        if (SceneManager.GetActiveScene().name != "Gameplay")
            SceneManager.LoadScene("Gameplay");

        mapData = GameConstants.Instance.mapPresets[stateManager.mapIndex];
        characterPrefab = GameConstants.Instance.characterPrefab;

        AudioManager.Instance.PlayMusic(mapData.mapMusic, mapData.musicVolume);

        stateManager.StartCoroutine(LoadGameplayObjects());
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (timerRunning)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0 || displayPercentage <= 0 || displayPercentage >= 1)
        {
            Debug.Log($"Timer: {timer}\nDisplay Percentage: {displayPercentage}");

            onWin(displayPercentage);
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        if (leftCharacter != null)
            leftCharacter.damageTakenMessage -= LeftTakeDamage;

        if (rightCharacter != null)
            rightCharacter.damageTakenMessage -= RightTakeDamage;
    }

    public void onWin(float sliderPercent)
    {
        Debug.Log($"On win called, sliderPercent: {sliderPercent}");

        if (sliderPercent < 0.5)
        {
            stateManager.leftWon = false;
            stateManager.rightWon = true;
        }
        else if (sliderPercent > 0.5f)
        {
            stateManager.leftWon = true;
            stateManager.rightWon = false;
        }
        else
        {
            stateManager.leftWon = false;
            stateManager.rightWon = false;
        }

        stateManager.TransitionToState(GameStateManager.StateEnum.Winner);
    }

    private void LeftTakeDamage(float damage)
    {
        displayPercentage -= damage;
        displayPercentage = Mathf.Clamp01(displayPercentage);
    }

    private void RightTakeDamage(float damage)
    {
        displayPercentage += damage;
        displayPercentage = Mathf.Clamp01(displayPercentage);
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

        leftCharacter.damageTakenMessage += LeftTakeDamage;

        leftSpellBook = new GameObject("Left Spell Book").AddComponent<SpellBook>();
        leftSpellBook.Init(stateManager.wandListenerLeft, leftCharacter, GameConstants.Instance.lookUpTable);

        rightCharacter = MonoBehaviour.Instantiate(characterPrefab);
        rightCharacter.transform.position = mapData.rightPos;
        rightCharacter.transform.rotation = Quaternion.Euler(mapData.rightRot);
        rightCharacter.transform.localScale = mapData.rightScale;

        rightCharacter.damageTakenMessage += RightTakeDamage;

        rightSpellBook = new GameObject("Right Spell Book").AddComponent<SpellBook>();
        rightSpellBook.Init(stateManager.wandListenerRight, rightCharacter, GameConstants.Instance.lookUpTable);

        timerRunning = true;
        timer = 100;
    }
}