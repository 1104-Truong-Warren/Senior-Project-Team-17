// Warren

// The purpose of the script is that it manages the victory screen that appears when the player defeats all of the enemies
// within the level. It will then display a panel with "CONTINUE" and "MAIN MENU" buttons, allowing the player
// to progress to the next level or return to the title screen.

// Source: https://youtu.be/Iv7A8TzreY4?si=wDyZHgiQ-Yc8oT3a - For setting up the victory screen panel

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [Header("Victory Screen")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    
    public static VictoryManager Instance { get; private set; }
    
    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Makes sure victory panel is hidden at start
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
    }
    
    // Activates the victory panel GameObject, called from TurnManager.LevelCleared() when all enemies are defeated.
    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }
    
    private void ContinueGame()
    {
        Debug.Log("Continue clicked - Load next level");

        // Clear instance so new one can be created
        Instance = null;
        
        Destroy(gameObject);
        
        SceneManager.LoadScene("TitleScreen"); // Placeholder, will update once Level 2 have been implemented.
    }
    
   private void GoToMainMenu()
    {
        Debug.Log("Main Menu clicked");
        
        Instance = null;
        
        Destroy(gameObject);
        
        // Load main menu
        SceneManager.LoadScene("TitleScreen");
    }

    private void OnDestroy()
    {
        // Clean up button listeners
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(ContinueGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(GoToMainMenu);
        }
    }
}