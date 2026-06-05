using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
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
}
