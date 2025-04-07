using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button endlessButton;
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private AudioMixer sfxAudioMixer;

    private void Awake()
    {
        if (!GameManager.isAnglerCatched)
        {
            //endlessButton.interactable = false;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void PlayEndless()
    {
        SceneManager.LoadScene(2);
    }
    
    public void SetMainVolume(float value)
    {
        mainAudioMixer.SetFloat("Volume", value);     
    }
    
    public void SetSFXVolume(float value)
    {
        sfxAudioMixer.SetFloat("Volume", value);     
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
