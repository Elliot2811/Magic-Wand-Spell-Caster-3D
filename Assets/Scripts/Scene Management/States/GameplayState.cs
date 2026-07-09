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
using System.Linq;
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

    [Header("Mid-Game Events")]
    [Tooltip("How much the winner of the mid-game event shifts displayPercentage in their favor.")]
    public float midGameEventEffectMagnitude = 0.2f;

    private IMidGameEvent[] midGameEvents;
    private IMidGameEvent activeMidGameEvent;
    private bool midGameEventTriggered = false;
    private float initialTimer;

    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);
        if (SceneManager.GetActiveScene().name != "Gameplay")
            SceneManager.LoadScene("Gameplay");
        mapData = GameConstants.Instance.mapPresets[stateManager.mapIndex];
        characterPrefab = GameConstants.Instance.characterPrefab;

        AudioManager.Instance.PlayMusic(mapData.mapMusic.clip, mapData.mapMusic.volume, mapData.mapMusic.randomizePitch);

        activeMidGameEvent = null;
        midGameEventTriggered = false;

        stateManager.StartCoroutine(LoadGameplayObjects());
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (timerRunning)
        {
            timer -= Time.deltaTime;

            if (!midGameEventTriggered && timer <= initialTimer * 0.5f)
            {
                TriggerMidGameEvent();
            }
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

        if (activeMidGameEvent != null)
        {
            activeMidGameEvent.OnEventCompleted -= OnMidGameEventCompleted;
            activeMidGameEvent = null;
        }
    }

    //<summary>
    //Picks one mid-game event at random, pauses the main timer, and starts it.
    //The main timer resumes automatically when the event's OnEventCompleted fires.
    //</summary>
    private void TriggerMidGameEvent()
    {
        midGameEventTriggered = true;

        if (midGameEvents == null || midGameEvents.Length == 0)
        {
            Debug.LogWarning("GamePlayState: halfway mark reached but no mid-game events are assigned.");
            return;
        }

        timerRunning = false;

        int index = UnityEngine.Random.Range(0, midGameEvents.Length);
        activeMidGameEvent = midGameEvents[index];

        Debug.Log($"GamePlayState: triggering mid-game event #{index} ({((MonoBehaviour)activeMidGameEvent).name}). Timer paused at {timer:F1}.");

        activeMidGameEvent.OnEventCompleted += OnMidGameEventCompleted;
        activeMidGameEvent.StartEvent();
    }

    //<summary>winningPlayer: 0 = draw/timeout, 1 = left won, 2 = right won.</summary>
    private void OnMidGameEventCompleted(int winningPlayer)
    {
        if (activeMidGameEvent != null)
        {
            activeMidGameEvent.OnEventCompleted -= OnMidGameEventCompleted;
            activeMidGameEvent = null;
        }

        ApplyMidGameEventEffect(winningPlayer);

        Debug.Log($"GamePlayState: mid-game event resolved (winningPlayer={winningPlayer}). Resuming timer at {timer:F1}.");
        timerRunning = true;
    }

    private void ApplyMidGameEventEffect(int winningPlayer)
    {
        if (winningPlayer == 1)
            displayPercentage = Mathf.Clamp01(displayPercentage + midGameEventEffectMagnitude);
        else if (winningPlayer == 2)
            displayPercentage = Mathf.Clamp01(displayPercentage - midGameEventEffectMagnitude);
        // winningPlayer == 0 (draw/timeout): no shift
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

        DiscoverMidGameEvents();

        timerRunning = true;
        timer = 100;
        initialTimer = timer;
    }

    //<summary>
    //Finds every IMidGameEvent component present in the Gameplay scene (active or not)
    //so they don't need to be manually wired up anywhere � just drop the GameObject
    //for a new event into the Gameplay scene and it's picked up automatically.
    //</summary>
    private void DiscoverMidGameEvents()
    {
        var scene = SceneManager.GetSceneByName("Gameplay");
        var found = new System.Collections.Generic.List<IMidGameEvent>();

        foreach (var root in scene.GetRootGameObjects())
        {
            found.AddRange(root.GetComponentsInChildren<MonoBehaviour>(true).OfType<IMidGameEvent>());
        }

        midGameEvents = found.ToArray();
        Debug.Log($"GamePlayState: discovered {midGameEvents.Length} mid-game event(s) in the Gameplay scene.");
    }
}