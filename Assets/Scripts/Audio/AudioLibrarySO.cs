using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Game/Audio Library")]
public class AudioLibrarySO : ScriptableObject
{
    public AudioClip coinInsertMusic;
    public AudioClip mapSelectionMusic;
    public AudioClip gameplayMusic;
    public AudioClip victoryMusic;
}