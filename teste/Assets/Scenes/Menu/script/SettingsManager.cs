using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Áudio")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;

    [Header("Mouse")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityLabel;

    void Start()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0f);
        sensitivitySlider.minValue = 0.1f;
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 2f);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);

        UpdateSensitivityLabel(sensitivitySlider.value);
    }

    public void SetMasterVolume(float value)
    {
        if (audioMixer != null) audioMixer.SetFloat("Master", value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        if (audioMixer != null) audioMixer.SetFloat("SFX", value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        if (audioMixer != null) audioMixer.SetFloat("Music", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        UpdateSensitivityLabel(value);
    }

    void UpdateSensitivityLabel(float value)
    {
        if (sensitivityLabel != null)
            sensitivityLabel.text = $"Sensibilidade: {value:F1}";
    }
}
