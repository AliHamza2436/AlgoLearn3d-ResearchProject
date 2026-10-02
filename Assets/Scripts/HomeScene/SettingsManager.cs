using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle soundToggle;
    public Toggle musicToggle;
    public Slider sensitivitySlider;

    // PlayerPrefs Keys for persistence
    private const string SoundPrefKey = "SoundEnabled";
    private const string MusicPrefKey = "MusicEnabled";
    private const string SensitivityPrefKey = "TouchSensitivity";

    private void Start()
    {
        LoadSavedSettings();

        if (soundToggle != null)
            soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);

        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);

        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void LoadSavedSettings()
    {
        bool soundOn = PlayerPrefs.GetInt(SoundPrefKey, 1) == 1;
        if (soundToggle != null) soundToggle.isOn = soundOn;
        ApplySoundState(soundOn);

        bool musicOn = PlayerPrefs.GetInt(MusicPrefKey, 1) == 1;
        if (musicToggle != null) musicToggle.isOn = musicOn;
        ApplyMusicState(musicOn);

        float savedSensitivity = PlayerPrefs.GetFloat(SensitivityPrefKey, 0.18f);
        if (sensitivitySlider != null) sensitivitySlider.value = savedSensitivity;
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(SoundPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save();

        ApplySoundState(isOn);
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(MusicPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMusicState(isOn);
    }

    private void ApplySoundState(bool isOn)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSoundEffectState(isOn);
        }
    }

    private void ApplyMusicState(bool isOn)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicState(isOn);
        }
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat(SensitivityPrefKey, value);
        PlayerPrefs.Save();
    }
}