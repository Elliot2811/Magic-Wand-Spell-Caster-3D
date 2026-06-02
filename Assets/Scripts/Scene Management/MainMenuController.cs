using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameState MapSelectionState;

    [Header("Music Settings")]
    [SerializeField] private AudioClip mainMenuMusic;

    private void Start()
    {
        //Play MainMenuMusic
        if (mainMenuMusic != null && GameStateManager.Instance.MusicSource != null)
        {
            if (GameStateManager.Instance.MusicSource.clip != mainMenuMusic)
            {
                GameStateManager.Instance.MusicSource.clip = mainMenuMusic;
                GameStateManager.Instance.MusicSource.Play();
            }
        }
    }

    void Update()
    {
        //PLACEHOLDERS for actual controller input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TriggerStartGame();
        }
    }
    public void TriggerStartGame()
    {
        GameStateManager.Instance.TransitionToState(MapSelectionState);
    }
}
