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

    public void PlayMusic(AudioClip clip, float musicClipVolume = 1f, float pitch = 1f, bool randomizePitch = false, float pitchVariance = 0f)
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
            musicSource.pitch = randomizePitch ? 1f + Random.Range(-pitchVariance, pitchVariance) : pitch;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip, float clipVolume = 1f, float pitch = 1f, bool randomizePitch = false, float pitchVariance = 0f)
    {
        if (clip == null) return;

        sfxSource.pitch = randomizePitch ? 1f + Random.Range(-pitchVariance, pitchVariance) : pitch;
        sfxSource.PlayOneShot(clip, clipVolume * GameConstants.globalSfxVolume);
    }
}