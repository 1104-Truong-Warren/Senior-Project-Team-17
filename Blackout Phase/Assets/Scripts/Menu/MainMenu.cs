// Warren
// The purpose of this script is to add and control the main menu functionality, including starting the game by loading the main gameplay scene and existing the application when the user selects the quit button.

// Resource: https://www.youtube.com/watch?v=-GWjA6dixV4 - For setting up main menu and the implementation of start game button and quitting to desktop
// Resource: https://www.youtube.com/watch?v=zc8ac_qUXQY&t=1s - For setting up the volume adjustment button and visuals

using UnityEngine;
using UnityEngine.SceneManagement; // This built-in library is used to load and manage scenes, allow to switch between various and different scenes.
using UnityEngine.UI; // Added for UI components
using TMPro; // Add this if using TextMeshPro

public class MainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] GameObject settingsPanel; // Reference to settings panel
    
    [Header("Settings UI")]
    [SerializeField] Slider volumeSlider; // Reference to volume slider
    [SerializeField] Button settingsButton; // Reference to settings button
    [SerializeField] Button backButton; // Reference to back button
    
    AudioManager audioManager; // Reference to audio manager
    
    void Start()
    {
        // Find or get audio manager reference
        audioManager = FindObjectOfType<AudioManager>();
        
        // Setup button listeners
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (backButton != null)
            backButton.onClick.AddListener(CloseSettings);
        
        // Setup volume slider
        if (volumeSlider != null && audioManager != null)
        {
            volumeSlider.value = audioManager.GetVolume();
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        
        // Ensure settings panel is closed at start
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    
    public void PlayGame() // Calls in Unity's built-in SceneMananger function, it will load the scene depending on the name.
    {
        SceneManager.LoadScene("TestIntro");
    }

    
    // Settings menu functions
    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    public void SetVolume(float volume)
    {
        if (audioManager != null)
        {
            audioManager.SetVolume(volume);
        }
    }

    public void QuitGame() // Calls in Unity's built-in Application and Quit function, when you press quit in the main menu, it will close the application completely, stopping all processes. 
    {
        Application.Quit();
        #if UNITY_EDITOR // Preprocessor directive, the code will only execute when the Unity Editor is running.
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the Unity Editor
        #endif // Ends the conditional compilation block.
    }
}