using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private AudioPair mainMenuMusic;

    private void Start()
    {
        //Play MainMenuMusic
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuMusic.clip, mainMenuMusic.volume, randomizePitch: mainMenuMusic.randomizePitch, pitchVariance: mainMenuMusic.pitchVariance);
        }
    }
}
