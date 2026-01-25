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
    
    void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame); // AddListener is a built-in Unity UI event that allows users to drag mouse cursor on the buttons on the screen and triggers a function.
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        Debug.Log("Game Over scene loaded");
    }
    
    // Reloads and resets the main gameplay scene.
    void RestartGame()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.ForceResetToPlayerTurn();
        }
    
        SceneManager.LoadScene("Demo_pxiel_2D_Test_Grid");
    }
    
    // Loads the Title Screen scene.
    void GoToMainMenu()
    {
        Debug.Log("Going to main menu");
        SceneManager.LoadScene("TitleScreen");
    }
    
    void Update()
    {
        // Keyboard keys shortcuts, if mouse cursor does not work on the buttons.
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