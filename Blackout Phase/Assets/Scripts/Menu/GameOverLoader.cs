// Warren 
// The purpose of this script is to monitor the game state and automatically load the Game Over scene when the player dies.

// Resource: https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadScene.html - For loading into a different scene
// Resource: https://docs.unity3d.com/Manual/class-ScriptableObject.html - For TurnManager singleton object pattern

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverLoader : MonoBehaviour
{
    [SerializeField] private string gameOverScene = "GameOver";
    
    void Update()
    {
        // When player dies in main game, load game over scene
        if (TurnManager.Instance != null && TurnManager.Instance.State == TurnState.GameOver)
        {
            Debug.Log("Player died, loading game over scene");
            SceneManager.LoadScene(gameOverScene);
        }
    }
}