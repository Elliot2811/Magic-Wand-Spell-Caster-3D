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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuMusic);
        }
    }
}
