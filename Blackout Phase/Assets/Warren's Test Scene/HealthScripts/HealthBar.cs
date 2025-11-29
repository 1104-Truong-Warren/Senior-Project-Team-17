using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Transform healthBarFill;
    [SerializeField] private Color fullHealthColor = new Color(0f, 1f, 0f); // Bright green
    [SerializeField] private Color lowHealthColor = new Color(1f, 0f, 0f);  // Bright red
    
    private Renderer healthBarRenderer;
    
    private void Start()
    {
        healthBarRenderer = healthBarFill.GetComponent<Renderer>();
        
        // Create a new unlit material
        Material newMat = new Material(Shader.Find("Unlit/Color"));
        healthBarRenderer.material = newMat;
        
        healthSystem.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(healthSystem.currentHealth);
    }
    
    private void UpdateHealthBar(int currentHealth)
    {
        float healthPercent = (float)currentHealth / healthSystem.maxHealth;
        healthBarFill.localScale = new Vector3(healthPercent, 0.1f, 0.1f);
        
        if (healthBarRenderer != null)
        {
            healthBarRenderer.material.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        }
    }
}