using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button endlessButton;
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private AudioMixer sfxAudioMixer;
    [SerializeField] private Slider mainAudioSlider;
    [SerializeField] private Slider sfxAudioSlider;

    private void Awake()
    {
        mainAudioSlider.value = -20;
        sfxAudioSlider.value = -20;
        if (PlayerPrefs.GetInt("isAnglerCatched") == 0)
        {
             endlessButton.interactable = false;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayEndless()
    {
        SceneManager.LoadScene(2);
        GameManager.isEndlessMode = true;
    }
    
    public void SetMainVolume(float value)
    {
        mainAudioMixer.SetFloat("Volume", value);     
    }
    
    public void SetSFXVolume(float value)
    {
        sfxAudioMixer.SetFloat("SFXVolume", value);     
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
