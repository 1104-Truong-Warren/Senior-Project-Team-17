using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    private HealthSystem healthSystem;
    
    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDeath += OnPlayerDeath;
    }
    
    private void OnPlayerDeath()
    {
        Debug.Log("PLAYER DIED - GAME OVER");
        // Add game over logic here
    }
}