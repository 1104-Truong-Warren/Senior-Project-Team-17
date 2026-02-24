// Warren

// The purpose of this script is that it manages all of the audio-related functionality in the game, which includes
// adjusting volume through AudioMixer, having a volume slider, and to make sure that it can be accessed anytime through pausing.

// Resource: https://www.youtube.com/watch?v=Y4bHjOiJBd4 - For adding background music for the title screen.
// Resource: https://youtu.be/G-JUp8AMEx0?si=jVBMhb5SNJ29ll3V - For creating an audio mixer, allowing the user to adjust the volume of the music.
// Resource: https://docs.unity3d.com/ScriptReference/PlayerPrefs.html - Saves pieces of information between game sessions so that players don't lose their settings when they close and reopen the game.

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string volumeParameter = "MasterVolume";
    
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
        // Load saved volume when game starts
        float savedVolume = GetVolume();
        SetVolume(savedVolume);
    }
    
    // The purpose of this function is to retrive the saved volume from the player using PlayerPrefs
    public float GetVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 0.75f); // Default 75%
    }
    
    // The purpose of this function is to apply the volume setting to the AudioMixer and save it
    public void SetVolume(float volume)
    {
        if (volume <= 0.001f)
        {
            audioMixer.SetFloat(volumeParameter, -80f); // Silences the volume completely
        }
        else
        {
            audioMixer.SetFloat(volumeParameter, Mathf.Log10(volume) * 20);
        }
        
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
}