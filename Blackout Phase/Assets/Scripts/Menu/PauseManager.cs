using UnityEngine;
using UnityEngine.SceneManagement;

// Resource: https://www.youtube.com/watch?v=9dYDBomQpBQ&t=37s

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to GameObject of the pause menu UI panel.
    private bool isPaused = false; // Tracks current pause state.

    // When the game starts, it will hide the pause menu by setting pause menu to inactive.
    void Start()
    {
        pauseMenuUI.SetActive(false); 
    }

    void Update()
    {
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

    // Function that restarts the level and resets the entire scene, this is not implemented in the game yet, but added here for the future in case.
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Function that loads the main menu, when the user press pause and press the quit button, it will load the main menu/title screen.
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
    }
}