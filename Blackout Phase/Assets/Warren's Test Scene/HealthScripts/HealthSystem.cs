using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth; // Changed from private to public
    
    [Header("Events")]
    public System.Action<int> OnHealthChanged;
    public System.Action OnDeath;
    
    public bool IsDead => currentHealth <= 0;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        OnHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
    }
    
    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        OnDeath?.Invoke();
        
        Destroy(gameObject);
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}