using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public AudioSource MusicSource { get; private set; }

    [SerializeField] private GameState initialState;
    private GameState currentState;

    private void Awake()
    {
        //Singleton Pattern
        if (Instance == null)
        {
            //Keep this GameStateManager running
            Instance = this;
            DontDestroyOnLoad(gameObject);

            MusicSource = gameObject.GetComponent<AudioSource>();
            if (MusicSource == null)
            {
                MusicSource = gameObject.AddComponent<AudioSource>();
            }
            MusicSource.loop = true;
            MusicSource.playOnAwake = false;
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
        currentState.EnterState(this);
    }
}
