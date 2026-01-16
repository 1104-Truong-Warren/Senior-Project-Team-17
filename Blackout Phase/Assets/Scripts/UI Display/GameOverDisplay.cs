// Warren

using UnityEngine;
using TMPro;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverText;
    
    void Start()
    {
        if (gameOverText == null)
        {
            gameOverText = GetComponent<TextMeshProUGUI>();
        }
        
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        // Check if game is over
        if (TurnManager.Instance != null && TurnManager.Instance.State == TurnState.GameOver)
        {
            ShowGameOver();
        }
        else
        {
            HideGameOver();
        }
    }
    
    void ShowGameOver()
    {
        if (gameOverText != null && !gameOverText.gameObject.activeSelf)
        {
            gameOverText.gameObject.SetActive(true);
            Debug.Log("Game Over Display: Showing Game Over text");
        }
    }
    
    void HideGameOver()
    {
        if (gameOverText != null && gameOverText.gameObject.activeSelf)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }
}