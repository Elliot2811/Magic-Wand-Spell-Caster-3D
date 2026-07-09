using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayMusic(AudioClip clip, float musicClipVolume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.pitch = pitch;
            musicSource.volume = musicClipVolume * GameConstants.globalMusicVolume;
            musicSource.Play();
        }
    }
    public void PlayMusic(AudioClip clip, float musicClipVolume = 1f, bool randomizePitch = false, float pitchVariance = 0f)
    {
        if (clip == null) 
        {
            StopMusic();
            return;
        }

        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.volume = musicClipVolume * GameConstants.globalMusicVolume;

            if (randomizePitch)
            {
                float randomPitch = Random.Range(-pitchVariance, pitchVariance);
                musicSource.pitch = 1f + randomPitch;
            }
            else
            {
                musicSource.pitch = 1f;
            }
            musicSource.Play();
        }
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void PlaySFX(AudioClip clip, float clipVolume = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            musicSource.Stop();
            return;
        }

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, clipVolume * GameConstants.globalSfxVolume);
    }

    public void PlaySFX(AudioClip clip, float clipVolume = 1f, bool randomizePitch = false, float pitchVariance = 0f)
    {
        if (clip == null) return;
        if (randomizePitch)
        {
            float randomPitch = Random.Range(-pitchVariance, pitchVariance);
            sfxSource.pitch = (1f + randomPitch);
        }
        else
        {
            sfxSource.pitch = 1f;
        }
        sfxSource.PlayOneShot(clip, clipVolume * GameConstants.globalSfxVolume);
    }   
}
