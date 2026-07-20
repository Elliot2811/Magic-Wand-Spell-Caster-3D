using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public AudioLibrarySO audioLibrary;

    public enum StateEnum
    {
        Init,
        PlayersSelect,
        MapSelect,
        Fight,
        Winner
    }

    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public JoyConTracker joyConTracker;

    public Wand wandPrefab;
    [HideInInspector]
    public Wand wandLeft;
    [HideInInspector]
    public Wand wandRight;

    public WandListener wandListenerPrefab;
    [HideInInspector]
    public WandListener wandListenerLeft;
    [HideInInspector]
    public WandListener wandListenerRight;

    [HideInInspector]
    public bool leftWon = false;
    [HideInInspector]
    public bool rightWon = false;

    public ShapesCollectionSO allShapesCollectionSO;

    private Dictionary<StateEnum, Func<GameState>> dict = new Dictionary<StateEnum, Func<GameState>>();

    [HideInInspector]
    public int gameScenario = 0;
    [HideInInspector]
    public int mapIndex = 0;

    private void Awake()
    {
        //Singleton Pattern
        if (Instance == null)
        {
            //Keep this GameStateManager running
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dict.Add(StateEnum.Init, () => new InitializationState());
            dict.Add(StateEnum.PlayersSelect, () => new CoinInsertState());
            dict.Add(StateEnum.Fight, () => new GamePlayState());
            dict.Add(StateEnum.Winner, () => new VictoryState());
        }
        else
        {
            //Prevent GameStateManager from duplicating
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        TransitionToState(StateEnum.Init);

    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.UpdateState();
        }
    }

    public void TransitionToState(StateEnum newState, float time)
    {
        StartCoroutine(TimeTransitionToState(newState, time));
    }

    public IEnumerator TimeTransitionToState(StateEnum newState, float time)
    {
        yield return new WaitForSeconds(time);

        TransitionToState(newState);
    }

    public void TransitionToState(StateEnum newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState();
        }

        if (!dict.ContainsKey(newState))
        {
            Debug.LogError($"[GameStateManager]: Contatins no enum game state of {newState.ToString()}");
        }

        CurrentState = dict[newState]();

        Debug.Log("Changing to " +  CurrentState);
        CurrentState.EnterState(this);
    }

    public void AddState(StateEnum state, Func<GameState> gameState)
    {
        if (dict.ContainsKey(state))
            Debug.LogError("[GameStateManager]: Contains a duplicate of the same enum game state");

        dict.Add(state, gameState);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearAllDrawings();
    }

    public void ResetGameAndWand()
    {
        gameScenario = 0;

        if (wandLeft != null)
        {
            Destroy(wandLeft.gameObject);
            wandLeft = null;
            wandListenerLeft = null;
        }
        if (wandRight != null)
        {
            Destroy(wandRight.gameObject);
            wandRight = null;
            wandListenerRight = null;
        }
    }
    private void ClearAllDrawings()
    {
        if (wandLeft != null)
            wandLeft.ClearDrawnLine();
        if (wandRight != null)
            wandRight.ClearDrawnLine();

        if (wandListenerLeft != null)
            wandListenerLeft.ClearShape();
        if (wandListenerRight != null)
            wandListenerRight.ClearShape();
    }

    //public void ResetWands()
    //{
    //    gameScenario = 0;

    //    if (wandLeft != null)
    //    {
    //        if (wandLeft.lineRenderer != null)
    //        {
    //            wandLeft.setNotReady();
    //            GameObject lr = wandLeft.lineRenderer.gameObject;
    //            wandLeft.lineRenderer = null;
    //            GameObject.DestroyImmediate(lr);
    //        }

    //        GameObject wandObj = wandLeft.gameObject;
    //        wandLeft = null;
    //        GameObject.DestroyImmediate(wandObj);
    //    }

    //    if (wandRight != null)
    //    {
    //        if (wandRight.lineRenderer != null)
    //        {
    //            wandRight.setNotReady();
    //            GameObject lr = wandRight.lineRenderer.gameObject;
    //            wandRight.lineRenderer = null;
    //            GameObject.DestroyImmediate(lr);
    //        }

    //        GameObject wandObj = wandRight.gameObject;
    //        wandRight = null;
    //        GameObject.DestroyImmediate(wandObj);
    //    }
    //}
}
