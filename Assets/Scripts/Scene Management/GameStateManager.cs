using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
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

    private Dictionary<StateEnum, GameState> dict = new Dictionary<StateEnum, GameState>();

    [HideInInspector]
    public int gameScenario = 0;

    private void Awake()
    {
        //Singleton Pattern
        if (Instance == null)
        {
            //Keep this GameStateManager running
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dict.Add(StateEnum.Init, new InitializationState());
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

    public void TransitionToState(StateEnum newState)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState();
        }

        dict.TryGetValue(newState, out GameState gameState);

        CurrentState = gameState;
        Debug.Log("Changing to " +  CurrentState);
        CurrentState.EnterState(this);
    }

    public void AddState(StateEnum state, GameState gameState)
    {
        if (dict.ContainsKey(state))
            Debug.LogError("[GameStateManager]: Contains a duplicate of the same enum game state");

        dict.Add(state, gameState);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
