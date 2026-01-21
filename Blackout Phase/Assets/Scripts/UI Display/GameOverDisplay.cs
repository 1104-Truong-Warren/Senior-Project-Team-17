// Warren

// The purpose of this script is that it manages the "GAME OVER" display text on the screen when the player dies.
// It continusously monitors the game state through the TurnManager script and shows/hides the TextMeshPro UI element.
// If the game is NOT over, then the GAME OVER screen is hidden.

using UnityEngine;
using TMPro;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    
    void Start()
    {
        // Finds TextMeshPro component if is is not assigned in the inspector.
        if (gameOverText == null)
        {
            gameOverText = GetComponent<TextMeshProUGUI>();
        }
        
        // Initial hiding of the GAME OVER text
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        // Checks if game is over, references the turn mananger conditions from TurnManager script Line 98.
        if (TurnManager.Instance != null && TurnManager.Instance.State == TurnState.GameOver)
        {
            ShowGameOver(); // Shows that the game is over.
        }
        else
        {
            HideGameOver(); // Hides the text if the game is not over.
        }
    }
    
    // This method displays the game over text.
    void ShowGameOver()
    {
        if (gameOverText != null && !gameOverText.gameObject.activeSelf)
        {
            gameOverText.gameObject.SetActive(true);
            Debug.Log("Game Over Display: Showing Game Over text");
        }
    }
    
    // This method hides the game over text.
    void HideGameOver()
    {
        // Checks if the thext exists and is currently active.
        if (gameOverText != null && gameOverText.gameObject.activeSelf)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }
}