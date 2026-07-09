using System;
using UnityEngine;

[Serializable]
public class AudioPair
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.5f;
    public bool randomizePitch = false;
    [Range(0f, 0.2f)]
    public float pitchVariance = 0f;
}

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Game/Audio Library")]
public class AudioLibrarySO : ScriptableObject
{
    public AudioPair coinInsertMusic;
    public AudioPair mapSelectionMusic;
    public AudioPair gameplayMusic;
    public AudioPair victoryMusic;
}