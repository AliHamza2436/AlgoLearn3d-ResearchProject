using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource;    
    public AudioSource soundEffectSource;       

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;  
    public AudioClip buttonClickClip;   
    public AudioClip footstepClip;      
    public AudioClip wallFrameClickClip;

    
    private const string SoundPrefKey = "SoundEnabled";
    private const string MusicPrefKey = "MusicEnabled";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ApplySavedPreferences();
    }

    private void ApplySavedPreferences()
    {
        bool soundOn = PlayerPrefs.GetInt(SoundPrefKey, 1) == 1;
        SetSoundEffectState(soundOn);

        bool musicOn = PlayerPrefs.GetInt(MusicPrefKey, 1) == 1;
        SetMusicState(musicOn);
    }

    public void PlayBackgroundMusic()
    {
        if (PlayerPrefs.GetInt(MusicPrefKey, 1) == 1)
        {
            if (musicSource != null && backgroundMusic != null)
            {
                if (!musicSource.isPlaying)
                {
                    musicSource.clip = backgroundMusic;
                    musicSource.loop = true;
                    musicSource.Play();
                }
            }
        }
    }

    public void SetMusicState(bool isPlaying)
    {
        if (musicSource != null)
        {
            if (isPlaying)
            {
                if (!musicSource.isPlaying)
                {
                    if (musicSource.clip == null) musicSource.clip = backgroundMusic;
                    musicSource.loop = true;
                    musicSource.Play();
                }
            }
            else
            {
                if (musicSource.isPlaying) musicSource.Pause();
            }
        }
    }

    public void SetSoundEffectState(bool isEnabled)
    {
        if (soundEffectSource != null)
        {
            soundEffectSource.mute = !isEnabled;
        }
    }

    public void PlayButtonClick()
    {
        PlaySoundEffect(buttonClickClip);
    }

    public void PlayFootstep()
    {
        PlaySoundEffect(footstepClip, 0.5f); 
    }

    public void PlayWallFrameClick()
    {
        PlaySoundEffect(wallFrameClickClip);
    }

    private void PlaySoundEffect(AudioClip clip, float volumeScale = 1.0f)
    {
        if (soundEffectSource != null && clip != null)
        {
            soundEffectSource.PlayOneShot(clip, volumeScale);
        }
    }
}