// Warren

using UnityEngine;
using TMPro;

public class DamageObserver : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI attackerText;
    
    [Header("Settings")]
    [SerializeField] private float displayTime = 2f;
    
    private int lastPlayerHP;
    private float hideTime;
    private bool isShowing = false;
    
    void Start()
    {
        // Get initial HP
        if (CharacterInfo1.Instance != null)
        {
            lastPlayerHP = CharacterInfo1.Instance.hp;
            Debug.Log($"DamageObserver: Started. Player HP = {lastPlayerHP}");
        }
        
        // Hide text initially
        if (damageText != null) damageText.gameObject.SetActive(false);
        if (attackerText != null) attackerText.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (CharacterInfo1.Instance == null)
        {
            Debug.LogWarning("DamageObserver: CharacterInfo1.Instance is null");
            return;
        }
        
        int currentHP = CharacterInfo1.Instance.hp;
        
        // Check if player took damage
        if (currentHP < lastPlayerHP)
        {
            int damage = lastPlayerHP - currentHP;
            ShowDamage(damage);
            lastPlayerHP = currentHP;
            
            Debug.Log($"DamageObserver: Player took {damage} damage. HP now {currentHP}");
        }
        else if (currentHP > lastPlayerHP)
        {
            // HP increased (healed)
            lastPlayerHP = currentHP;
        }
        
        // Hide text after display time
        if (isShowing && Time.time > hideTime)
        {
            HideDamage();
        }
    }
    
    void ShowDamage(int damage)
    {
        // Try to find which enemy attacked
        string attackerName = FindAttackingEnemy();
        
        // Update UI
        if (damageText != null)
        {
            damageText.text = $"-{damage} HP";
            damageText.gameObject.SetActive(true);
        }
        
        if (attackerText != null)
        {
            attackerText.text = $"{attackerName} attacks!";
            attackerText.gameObject.SetActive(true);
        }
        
        // Set timer
        hideTime = Time.time + displayTime;
        isShowing = true;
        
        Debug.Log($"DamageObserver: {attackerName} hit for {damage} damage");
    }
    
    void HideDamage()
    {
        if (damageText != null) damageText.gameObject.SetActive(false);
        if (attackerText != null) attackerText.gameObject.SetActive(false);
        isShowing = false;
    }
    
    string FindAttackingEnemy()
    {
        // Find all enemies
        EnemyController1[] enemies = FindObjectsOfType<EnemyController1>();
        
        if (enemies.Length == 0) return "Enemy";
        
        // Find closest enemy to player
        Transform player = CharacterInfo1.Instance.transform;
        EnemyController1 closestEnemy = null;
        float closestDistance = float.MaxValue;
        
        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, player.position);
            
            if (distance < closestDistance && distance < 5f) // Within 5 units
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        
        if (closestEnemy != null)
        {
            // Clean up name (remove "(Clone)" if present)
            return closestEnemy.name.Replace("(Clone)", "").Trim();
        }
        
        return "Enemy";
    }
}