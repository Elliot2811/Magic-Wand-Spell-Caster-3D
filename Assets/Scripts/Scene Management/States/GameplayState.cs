using System;
using System.Collections;
using System.Collections.Generic;
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

    public bool redrawFlag = false;
    public ShapesCollectionSO leftSpellCollection;
    public ShapesCollectionSO rightSpellCollection;

    [Header("Mid-Game Events")]
    [Tooltip("How much the winner of the mid-game event shifts displayPercentage in their favor.")]
    public float midGameEventEffectMagnitude = 0.2f;

    [Header("Match End")]
    [Tooltip("Non-finishing damage dealt to the losing player once the match resolves.")]
    [SerializeField] private float matchEndDamage = 8f;

    private IMidGameEvent[] midGameEvents;
    private IMidGameEvent activeMidGameEvent;
    private bool midGameEventTriggered = false;
    private bool activeEventPausedTimer;
    private float initialTimer;

    public override void EnterState(GameStateManager gameManager)
    {
        base.EnterState(gameManager);

        mapData = GameConstants.Instance.mapPresets[stateManager.mapIndex];
        //characterPrefab = GameConstants.Instance.characterPrefab;
        leftCharacter = GameConstants.Instance.player1Prefab;
        rightCharacter = GameConstants.Instance.player2Prefab;

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

        stateManager.renderSpellBookSpell = false; //gameplay's over — stop rendering/casting spells

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

        int index = UnityEngine.Random.Range(0, midGameEvents.Length);
        activeMidGameEvent = midGameEvents[index];
        activeEventPausedTimer = activeMidGameEvent.PausesMainTimer;

        timerRunning = false;

        Debug.Log($"GamePlayState: triggering mid-game event #{index} ({((MonoBehaviour)activeMidGameEvent).name}). " +
                  $"PausesMainTimer={activeEventPausedTimer}. Timer at {timer:F1}.");

        activeMidGameEvent.OnEventCompleted += OnMidGameEventCompleted;

        stateManager.StartCoroutine(RunPreEventSequence());
    }

    //Bonus Event Banner -> instructions -> actual event start, run in sequence. Each step is
    //skipped gracefully (with a warning) if its controller isn't in the scene.
    //</summary>
    private IEnumerator RunPreEventSequence()
    {
        if (MidGameEventBannerController.Instance != null)
            yield return MidGameEventBannerController.Instance.ShowBanner(activeMidGameEvent.EventTitle, () => { });
        else
            Debug.LogWarning("GamePlayState: no MidGameEventBannerController in scene — skipping banner.");

        if (MidGameEventInstructionController.Instance != null)
            yield return MidGameEventInstructionController.Instance.ShowInstructions(activeMidGameEvent.EventTitle, activeMidGameEvent.EventInstructions);
        else
            Debug.LogWarning("GamePlayState: no MidGameEventInstructionController in scene — skipping instructions.");

        StartActiveEventAfterBanner();
    }

    //<summary>Called once the pre-event banner finishes — applies the event's
    //actual timer-pause preference and kicks it off for real.</summary>
    //<summary>
    private void StartActiveEventAfterBanner()
    {
        timerRunning = !activeEventPausedTimer;
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

        Debug.Log($"GamePlayState: mid-game event resolved (winningPlayer={winningPlayer}). Timer at {timer:F1}.");

        if (activeEventPausedTimer)
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

            leftCharacter?.TakeDamage(matchEndDamage); //left lost the match
        }
        else if (sliderPercent > 0.5f)
        {
            stateManager.leftWon = true;
            stateManager.rightWon = false;

            rightCharacter?.TakeDamage(matchEndDamage); //right lost the match
        }
        else
        {
            stateManager.leftWon = false;
            stateManager.rightWon = false;
            // draw — nobody takes match-end damage
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

        leftCharacter = MonoBehaviour.Instantiate(GameConstants.Instance.player1Prefab);
        leftCharacter.transform.position = mapData.leftPos;
        leftCharacter.transform.rotation = Quaternion.Euler(mapData.leftRot);
        leftCharacter.transform.localScale = mapData.leftScale;
        leftCharacter.damageTakenMessage += LeftTakeDamage;

        rightCharacter = MonoBehaviour.Instantiate(GameConstants.Instance.player2Prefab);
        rightCharacter.transform.position = mapData.rightPos;
        rightCharacter.transform.rotation = Quaternion.Euler(mapData.rightRot);
        rightCharacter.transform.localScale = mapData.rightScale;
        rightCharacter.damageTakenMessage += RightTakeDamage;

        bool leftActive = (stateManager.gameScenario & 0b01) != 0;
        bool rightActive = (stateManager.gameScenario & 0b10) != 0;

        if (leftActive) stateManager.wandListenerLeft.ChangeShapesCollection(GameConstants.Instance.allShapes);
        if (rightActive) stateManager.wandListenerRight.ChangeShapesCollection(GameConstants.Instance.allShapes);

        yield return WaitForHowToPlayConfirmation(leftActive, rightActive);

        if (HowToPlayPanelController.Instance != null)
            yield return HowToPlayPanelController.Instance.ShowRecapThenHide();

        leftSpellBook = new GameObject("Left Spell Book").AddComponent<SpellBook>();
        leftSpellBook.Init(stateManager.wandListenerLeft, leftCharacter, GameConstants.Instance.lookUpTable, GameConstants.Instance.allShapes, 0);

        rightSpellBook = new GameObject("Right Spell Book").AddComponent<SpellBook>();
        rightSpellBook.Init(stateManager.wandListenerRight, rightCharacter, GameConstants.Instance.lookUpTable, GameConstants.Instance.allShapes, 1);

        DiscoverMidGameEvents();

        //3-2-1 Countdown before game starts
        if (CountdownController.Instance != null)
            yield return CountdownController.Instance.ShowCountdown();
        else
            Debug.LogWarning("GamePlayState: no CountdownController in scene — skipping countdown.");

        timerRunning = true;
        timer = 100;
        initialTimer = timer;

        stateManager.renderSpellBookSpell = true; //gameplay is live — safe for SpellBook to render/cast now
    }
    //private IEnumerator WaitForHowToPlayConfirmation(bool leftActive, bool rightActive)
    //{
    //    bool leftReady = !leftActive;   //inactive side auto-satisfied
    //    bool rightReady = !rightActive;

    //    Action<ShapeInfoSO> onLeftMatch = shape => { if (shape != null) leftReady = true; };
    //    Action<ShapeInfoSO> onRightMatch = shape => { if (shape != null) rightReady = true; };

    //    if (leftActive) stateManager.wandListenerLeft.MatchedShape += onLeftMatch;
    //    if (rightActive) stateManager.wandListenerRight.MatchedShape += onRightMatch;

    //    yield return new WaitUntil(() => leftReady && rightReady);

    //    if (leftActive) stateManager.wandListenerLeft.MatchedShape -= onLeftMatch;
    //    if (rightActive) stateManager.wandListenerRight.MatchedShape -= onRightMatch;
    //}

    //If tutorial requires players to draw all four spells once
    private IEnumerator WaitForHowToPlayConfirmation(bool leftActive, bool rightActive)
    {
        var allShapes = GameConstants.Instance.allShapes.GetAllShapes();
        int requiredCount = allShapes.Length;

        var leftDrawn = new HashSet<ShapeInfoSO>();
        var rightDrawn = new HashSet<ShapeInfoSO>();

        Action<ShapeInfoSO> onLeftMatch = shape =>
        {
            if (shape != null)
            {
                leftDrawn.Add(shape);
                HowToPlayPanelController.Instance?.MarkLeftDrawn(shape);
            }
        };
        Action<ShapeInfoSO> onRightMatch = shape =>
        {
            if (shape != null)
            {
                rightDrawn.Add(shape);
                HowToPlayPanelController.Instance?.MarkRightDrawn(shape);
            }
        };

        if (leftActive) stateManager.wandListenerLeft.MatchedShape += onLeftMatch;
        if (rightActive) stateManager.wandListenerRight.MatchedShape += onRightMatch;

        bool LeftDone() => !leftActive || leftDrawn.Count >= requiredCount;
        bool RightDone() => !rightActive || rightDrawn.Count >= requiredCount;

        yield return new WaitUntil(() => LeftDone() && RightDone());

        if (leftActive) stateManager.wandListenerLeft.MatchedShape -= onLeftMatch;
        if (rightActive) stateManager.wandListenerRight.MatchedShape -= onRightMatch;
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