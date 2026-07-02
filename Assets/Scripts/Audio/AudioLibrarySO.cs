using System;
using UnityEngine;

[Serializable]
public class AudioPair
{
    [Range(0f, 1f)]
    public float volume = 1f;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Game/Audio Library")]
public class AudioLibrarySO : ScriptableObject
{
    public AudioPair coinInsertMusic;
    public AudioPair mapSelectionMusic;
    public AudioPair gameplayMusic;
    public AudioPair victoryMusic;
}