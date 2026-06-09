using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private GameState initialState;
    private GameState currentState;

    [HideInInspector] public int gameScenarioChosen = 0;

    private void Awake()
    {
        //Singleton Pattern
        if (Instance == null)
        {
            //Keep this GameStateManager running
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        TransitionToState(initialState);
    }
    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }
    public void TransitionToState(GameState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        Debug.Log("Changing to " + currentState);
        currentState.EnterState(this);
    }
}
