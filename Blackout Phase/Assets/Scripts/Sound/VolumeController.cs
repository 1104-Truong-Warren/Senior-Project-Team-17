// Warren

// The purpose of this script is it allows the player to adjust the volume of the music and SFX
// in the main game scene instead of the main menu. Has same functionalies as the main menu version,
// but this is specifically integrated with the pause menu instead.

using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Settings UI")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button backButton;

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
        
        backButton.onClick.AddListener(CloseOptions);
        
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        
        AudioSource music = GameObject.Find("BackgroundMusic")?.GetComponent<AudioSource>();
        if (music != null) music.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        
        // Find all audio sources and set their volume
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in allAudio)
        {
            if (audio.gameObject.name != "BackgroundMusic")
            {
                audio.volume = volume;
            }
        }
    }
    
    public void CloseOptions()
    {
        gameObject.SetActive(false);
        FindFirstObjectByType<PauseManager>()?.CloseOptions();
    }
}