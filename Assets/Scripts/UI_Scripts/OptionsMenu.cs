using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject controllerPanel;

    [Header("Audio Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);

        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        audioPanel.SetActive(false);
        keyboardPanel.SetActive(false);
        controllerPanel.SetActive(false);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    public void OpenAudio()
    {
        audioPanel.SetActive(true);
        keyboardPanel.SetActive(false);
        controllerPanel.SetActive(false);
    }

    public void OpenKeyboard()
    {
        audioPanel.SetActive(false);
        keyboardPanel.SetActive(true);
        controllerPanel.SetActive(false);
    }

    public void OpenController()
    {
        audioPanel.SetActive(false);
        keyboardPanel.SetActive(false);
        controllerPanel.SetActive(true);
    }

    private void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private void SetSFXVolume(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void SetMusicVolume(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }
}