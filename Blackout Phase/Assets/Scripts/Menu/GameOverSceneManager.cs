// Warren
// The purpose of this script is that it will load in a GAME OVER scene when the player dies. 
// A GAME OVER screen will pop up, and there are two buttons the player can press.
// They could either restart the level or return to the main menu.

// Resource: https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadScene.html - For loading different scenes from main gameplay loop
// Resource: https://docs.unity3d.com/ScriptReference/Events.UnityEvent.AddListener.html - For allow mouse click on UI buttons
// Resource: https://www.youtube.com/watch?v=VbZ9_C4-Qbo&t=250s around the 7:30 mark - For setting up GAME OVER conditions, along with the buttons

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    // Stores the name of the level the player was just on
    private string lastLevelName;
    
    void Start()
    {
        // Retrieve the level name that was saved before loading the Game Over scene
        // If no level was saved, default to "Level1" as a test
        lastLevelName = PlayerPrefs.GetString("LastLevel", "Level1");
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        Debug.Log("Game Over scene loaded - Last level was: " + lastLevelName);
    }
    
    // Reloads and resets the current gameplay scene, and it works for any level, instead of one predefined level.
    void RestartGame()
    {
        Debug.Log("Restarting game on level: " + lastLevelName);
        
        // Resets player stats before restarting.
        if (CharacterInfo1.Instance != null)
        {
            CharacterInfo1.Instance.ResetForLevelRestart();
            Debug.Log("Reset player stats for restart");
        }

        // Reset TurnManager state before loading scene
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.ForceResetToPlayerTurn();
        }
        
        SceneManager.LoadScene(lastLevelName);
    }
    
    // Loads the Title Screen scene.
    void GoToMainMenu()
    {
        Debug.Log("Going to main menu");
        
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.ForceResetToPlayerTurn();
        }
        
        SceneManager.LoadScene("TitleScreen");
    }
    
    void Update()
    {
        // Keyboard shortcuts, if mouse cursor does not work on the buttons.
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            GoToMainMenu();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu();
        }
    }
}