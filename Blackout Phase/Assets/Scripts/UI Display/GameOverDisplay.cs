// Warren

// The purpose of this script is that it manages the "GAME OVER" display text on the screen when the player dies.
// It continusously monitors the game state through the TurnManager script and shows/hides the TextMeshPro UI element.
// If the game is NOT over, then the GAME OVER screen is hidden.

// using UnityEngine;
// using TMPro;

// public class GameOverDisplay : MonoBehaviour
// {
//     [SerializeField] private TextMeshProUGUI gameOverText;
    
//     void Start()
//     {
//         // Finds TextMeshPro component if is is not assigned in the inspector.
//         if (gameOverText == null)
//         {
//             gameOverText = GetComponent<TextMeshProUGUI>();
//         }
        
//         // Initial hiding of the GAME OVER text
//         if (gameOverText != null)
//         {
//             gameOverText.gameObject.SetActive(false);
//         }
//     }
    
//     void Update()
//     {
//         // Checks if game is over, references the turn mananger conditions from TurnManager script Line 98.
//         if (TurnManager.Instance != null && TurnManager.Instance.State == TurnState.GameOver)
//         {
//             ShowGameOver(); // Shows that the game is over.
//         }
//         else
//         {
//             HideGameOver(); // Hides the text if the game is not over.
//         }
//     }
    
//     // This method displays the game over text.
//     void ShowGameOver()
//     {
//         if (gameOverText != null && !gameOverText.gameObject.activeSelf)
//         {
//             gameOverText.gameObject.SetActive(true);
//             Debug.Log("Game Over Display: Showing Game Over text");
//         }
//     }
    
//     // This method hides the game over text.
//     void HideGameOver()
//     {
//         // Checks if the thext exists and is currently active.
//         if (gameOverText != null && gameOverText.gameObject.activeSelf)
//         {
//             gameOverText.gameObject.SetActive(false);
//         }
//     }
// }

// Warren
// FIXED - Resets player on scene load

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuScene = "TitleScreen";
    
    void Start()
    {
        Debug.Log("GameOverDisplay: Script started - Scene: " + SceneManager.GetActiveScene().name);
        
        // Hide UI on start
        HideAllUI();
        
        // Reset player state when scene loads
        StartCoroutine(ResetPlayerOnLoad());
        
        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }
        else
        {
            Debug.LogError("Restart Button is NULL!");
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        else
        {
            Debug.LogError("Main Menu Button is NULL!");
        }
    }
    
    IEnumerator ResetPlayerOnLoad()
    {
        // Wait for everything to initialize
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log("Checking player state after scene load...");
        
        // Check if player exists
        if (CharacterInfo1.Instance != null)
        {
            Debug.Log($"Player found. HP: {CharacterInfo1.Instance.hp}");
            
            // If player is dead, try to fix it
            if (CharacterInfo1.Instance.hp <= 0)
            {
                Debug.LogWarning("Player is dead on scene load! Attempting to fix...");
                
                // Reset TurnManager state
                if (TurnManager.Instance != null)
                {
                    Debug.Log($"Current TurnState: {TurnManager.Instance.State}");
                    
                    // Only reset if it's GameOver
                    if (TurnManager.Instance.State == TurnState.GameOver)
                    {
                        TurnManager.Instance.SetTurnState(TurnState.PlayerAction);
                        Debug.Log("Reset TurnState to PlayerAction");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("CharacterInfo1.Instance is null - player not created yet");
        }
    }
    
    void Update()
    {
        // Checks if the game is over
        if (TurnManager.Instance != null && TurnManager.Instance.State == TurnState.GameOver)
        {
            Debug.Log("GAME OVER detected in Update");
            ShowGameOver();
        }
        
        CheckKeyboardShortcuts();
    }
    
    void ShowGameOver()
    {
        // Show Game Over text
        if (gameOverText != null && !gameOverText.gameObject.activeSelf)
        {
            gameOverText.gameObject.SetActive(true);
            Debug.Log("Game Over text SHOWN");
        }
        
        // Show buttons
        if (restartButton != null && !restartButton.gameObject.activeSelf)
        {
            restartButton.gameObject.SetActive(true);
            Debug.Log("Restart button SHOWN");
        }
        
        if (mainMenuButton != null && !mainMenuButton.gameObject.activeSelf)
        {
            mainMenuButton.gameObject.SetActive(true);
            Debug.Log("Main menu button SHOWN");
        }
    }
    
    void HideAllUI()
    {
        // Hide text
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        
        // Hide buttons
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.gameObject.SetActive(false);
        }
    }
    
    void RestartLevel()
    {
        Debug.Log("Restarting Level");
        Time.timeScale = 1f;
        
        // Destroy UI elements to prevent it from showing back up after scene resets
        DestroyUI();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    void GoToMainMenu()
    {
        Debug.Log("Go to main menu");
        Time.timeScale = 1f;
        
        // DESTROY UI ELEMENTS
        DestroyUI();
        
        // Load main menu
        SceneManager.LoadScene(mainMenuScene);
    }
    
    void DestroyUI()
    {
        // Destroy Game Over Text
        if (gameOverText != null)
        {
            Destroy(gameOverText.gameObject);
            Debug.Log("Destroyed Game Over Text");
        }
        
        // Destroy Restart Button
        if (restartButton != null)
        {
            Destroy(restartButton.gameObject);
            Debug.Log("Destroyed Restart Button");
        }
        
        // Destroy Main Menu Button
        if (mainMenuButton != null)
        {
            Destroy(mainMenuButton.gameObject);
            Debug.Log("Destroyed Main Menu Button");
        }
    }
    
    void CheckKeyboardShortcuts()
    {
        if (gameOverText != null && gameOverText.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
            
            if (Input.GetKeyDown(KeyCode.M))
            {
                GoToMainMenu();
            }
            
            // Test key, force hiding
            if (Input.GetKeyDown(KeyCode.H))
            {
                HideAllUI();
                Debug.Log("Force hidden UI with H key");
            }
        }
    }
}