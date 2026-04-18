// The purpose of this script is to manage the game's pause functionality, including toggling the pause menu, freezing and resuming gameplay, and handling scene transitions such as returning to the main menu.
// Warren
using UnityEngine;
using UnityEngine.SceneManagement;  // This built-in library is used to load and manage scenes, allow to switch between various and different scenes.

// Resource: https://www.youtube.com/watch?v=9dYDBomQpBQ&t=37s

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to GameObject of the pause menu UI panel.
    public GameObject optionsCanvas; // Reference to the Options Canvas (VolumeController)
    private bool isPaused = false; // Tracks current pause state.

    // When the game starts, it will hide the pause menu by setting pause menu to inactive.
    void Start()
    {
        pauseMenuUI.SetActive(false);
        
        // Make sure options canvas is hidden at start
        if (optionsCanvas != null)
        {
            optionsCanvas.SetActive(false);
        }
    }

    void Update()
    {
        // Only handle escape if options menu is NOT open
        if (optionsCanvas != null && optionsCanvas.activeSelf)
            return; // Don't close pause menu if options is open
            
        // Toggle pause when player presses the Escape Key.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If the game is paused, then the user can either resume the game or leave it paused.
            if (isPaused) 
            {
                ResumeGame(); // Calls the resume game function.
            }else{
                PauseGame(); // Calls the pause game function.
            }
        }
    }

    // Function that pauses the game, and the pause menu UI will pop up.
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freezes the game.
        isPaused = true;
    }

    // Function that resumes the game once the pause menu UI pop up.
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Unfreezes the game
        isPaused = false;
    }

    // Function that opens the options menu
    public void OpenOptions()
    {
        if (optionsCanvas != null)
        {
            pauseMenuUI.SetActive(false); // Hide pause menu
            optionsCanvas.SetActive(true); // Show options
        }
        else
        {
            Debug.LogError("Options Canvas is not assigned in PauseManager!");
        }
    }
    
    // Function that closes the options menu (called by VolumeController)
    public void CloseOptions()
    {
        if (optionsCanvas != null)
        {
            optionsCanvas.SetActive(false); // Hide options
            pauseMenuUI.SetActive(true); // Show pause menu
        }
    }

    // Function that restarts the level and resets the entire scene, this is not implemented in the game yet, but added here for the future in case.
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Function that loads the main menu, when the user press pause and press the quit button, it will load the main menu/title screen.
    public void LoadMainMenu()
    {
        // Deletes the character game object when the player returns to main menu. Added this to help with save/load problem.
        GameObject player = GameObject.FindGameObjectWithTag("Player1");
        if (player != null)
        {
            Debug.Log("Destroying player before leaving to main menu");
            Destroy(player);
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
    }

    // Function that lets the player save their game anytime. 
    public void OnSaveButtonClick()
    {
        SaveManager saveManager = FindFirstObjectByType<SaveManager>();
        if (saveManager != null)
        {
            saveManager.SaveGame();
        }
        else
        {
            Debug.LogError("SaveManager not found!");
        }
    }
}