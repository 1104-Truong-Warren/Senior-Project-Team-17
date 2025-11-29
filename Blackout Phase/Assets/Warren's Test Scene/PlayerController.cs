using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private HealthSystem healthSystem;
    private HealthBar healthBar;
    
    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthBar = GetComponent<HealthBar>();
    }
    
    private void Update()
    {
        // Test damage - press H to take 25 damage
        if (Input.GetKeyDown(KeyCode.H))
        {
            healthSystem.TakeDamage(25);
            Debug.Log($"Player took 25 damage! Health: {healthSystem.currentHealth}/100");
        }
        
        // Test healing - press J to heal 25
        if (Input.GetKeyDown(KeyCode.J))
        {
            healthSystem.Heal(25);
            Debug.Log($"Player healed 25! Health: {healthSystem.currentHealth}/100");
        }
        
        // Test death - press K to take 100 damage
        if (Input.GetKeyDown(KeyCode.K))
        {
            healthSystem.TakeDamage(100);
            Debug.Log($"Player took 100 damage! Should be dead.");
        }
    }
}