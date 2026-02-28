// Warren

// The purpose of this script is that it manages all of the audio-related functionality in the game, which includes
// adjusting volume through AudioMixer, having a volume slider, and to make sure that it can be accessed anytime through pausing.

// Resource: https://www.youtube.com/watch?v=Y4bHjOiJBd4 - For adding background music for the title screen.
// Resource: https://www.youtube.com/watch?v=iNRl7b9RQpw - For adding button SFX for the title screen.
// Resource: https://youtu.be/G-JUp8AMEx0?si=jVBMhb5SNJ29ll3V - For creating an audio mixer, allowing the user to adjust the volume of the music.
// Resource: https://docs.unity3d.com/ScriptReference/PlayerPrefs.html - Saves pieces of information between game sessions so that players don't lose their settings when they close and reopen the game.

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string masterVolumeParameter = "MasterVolume";
    [SerializeField] string sfxVolumeParameter = "SFXVolume"; 
    
    [Header("UI References")]
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;    
    
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Find sliders by tag
        if (masterVolumeSlider == null)
        {
            GameObject sliderObj = GameObject.FindGameObjectWithTag("Master Volume slider");
            if (sliderObj != null)
            {
                masterVolumeSlider = sliderObj.GetComponent<Slider>();
            }
        }
        
        if (sfxVolumeSlider == null)
        {
            GameObject sliderObj = GameObject.FindGameObjectWithTag("SFX Volume slider");
            if (sliderObj != null)
            {
                sfxVolumeSlider = sliderObj.GetComponent<Slider>();
            }
        }
        
        // Load saved volumes when game starts
        float savedMasterVolume = GetMasterVolume();
        float savedSFXVolume = GetSFXVolume();
        
        SetMasterVolume(savedMasterVolume);
        SetSFXVolume(savedSFXVolume);
        
        // Update sliders if they're assigned
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = savedMasterVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
            
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = savedSFXVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }
    
    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 0.75f); // Default 75%
    }
    
    public void SetMasterVolume(float volume)
    {
        if (volume <= 0.001f)
        {
            audioMixer.SetFloat(masterVolumeParameter, -80f);
        }
        else
        {
            audioMixer.SetFloat(masterVolumeParameter, Mathf.Log10(volume) * 20);
        }
        
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
    
    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFXVolume", 0.75f); // Default 75%
    }
    
    public void SetSFXVolume(float volume)
    {
        if (volume <= 0.001f)
        {
            audioMixer.SetFloat(sfxVolumeParameter, -80f);
        }
        else
        {
            audioMixer.SetFloat(sfxVolumeParameter, Mathf.Log10(volume) * 20);
        }
        
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }
}