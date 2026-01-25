// Warren
// The purpose of this script is to observe player health changes and display damage notifications in the UI.
// When the player takes damage, it shows how much damage was dealt and attempts to shows that the enemy attacked.
// The notification displays for 2 seconds, and then automatically disappears.

// Resource: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html - For  health monitoring
// Resource: https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html - For showing/hiding UI elements
// Resource: https://docs.unity3d.com/ScriptReference/Object.FindObjectsOfType.html - For finding attacking enemies
// Resource: https://docs.unity3d.com/ScriptReference/Vector3.Distance.html - For calculating closest enemy distance

using UnityEngine;
using TMPro;

public class DamageObserver : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI damageText; 
    [SerializeField] private TextMeshProUGUI attackerText; 
    [Header("Settings")]
    [SerializeField] private float displayTime = 2f; // How long damage notification stays visible, in this case, 2 seconds.
    private int lastPlayerHP; // Player's HP from previous frame
    private float hideTime; // Amount of time the UI will be hidden       
    private bool isShowing = false;
    
    void Start()
    {
        // Get initial HP, provides access to player's health from CharacterInfo1
        if (CharacterInfo1.Instance != null)
        {
            lastPlayerHP = CharacterInfo1.Instance.hp;
            Debug.Log($"DamageObserver: Started. Player HP = {lastPlayerHP}");
        }
        
        // Hide text initially, GameObjects are inactive until damage occurs
        if (damageText != null) damageText.gameObject.SetActive(false);
        if (attackerText != null) attackerText.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Makes sure that CharacterInfo1 exists before accessing it
        if (CharacterInfo1.Instance == null)
        {
            Debug.LogWarning("DamageObserver: CharacterInfo1.Instance is null");
            return;
        }
        
        int currentHP = CharacterInfo1.Instance.hp;
        
        // Check if player took damage by comparing with previous frame's HP
        if (currentHP < lastPlayerHP)
        {
            int damage = lastPlayerHP - currentHP; // Calculate damage amount
            ShowDamage(damage); // Display damage notification
            lastPlayerHP = currentHP; // Update stored HP
            
            Debug.Log($"DamageObserver: Player took {damage} damage. HP now {currentHP}");
        }
        else if (currentHP > lastPlayerHP)
        {
            lastPlayerHP = currentHP; // HP increased, update stored HP without showing notification
        }
        
        // Hide text after display time 
        // Time.time gives current game time in seconds
        if (isShowing && Time.time > hideTime)
        {
            HideDamage();
        }
    }
    
    // Displays damage notification with amount and enemy name
    void ShowDamage(int damage)
    {
        // Finds which enemy attacked by checking proximity to player
        string attackerName = FindAttackingEnemy();
        
        // Update UI, set text content and activate GameObject
        if (damageText != null)
        {
            damageText.text = $"-{damage} HP"; // Format: "-10 HP"
            damageText.gameObject.SetActive(true);
        }
        
        if (attackerText != null)
        {
            attackerText.text = $"{attackerName} attacks!";
            attackerText.gameObject.SetActive(true);
        }
        
        // Set timer, such as the hide time
        hideTime = Time.time + displayTime;
        isShowing =
        
        Debug.Log($"DamageObserver: {attackerName} hit for {damage} damage");
    }
    
    // Hides the damage notification UI elements
    void HideDamage()
    {
        if (damageText != null) damageText.gameObject.SetActive(false);
        if (attackerText != null) attackerText.gameObject.SetActive(false);
        isShowing = false;
    
        // Determines which enemy likely attacked the player
        string FindAttackingEnemy()
        {
            // Find all enemies in the scene from EnemyController1 script
            EnemyController1[] enemies = FindObjectsOfType<EnemyController1>();
            
            if (enemies.Length == 0)
            {
                return "Enemy"; // Default name if no enemies found
            } 
            
            // Find closest enemy to player using distance calculation
            Transform player = CharacterInfo1.Instance.transform; // Get player's position
            EnemyController1 closestEnemy = null;
            float closestDistance = float.MaxValue; // Start with very large number
            
            // foreach function goes through every item in an array or list
            // In this case, it is checkint which enemy attacked the player
            foreach (var enemy in enemies)
            {
                // Calculate distance between enemy and player
                float distance = Vector3.Distance(enemy.transform.position, player.position);
                
                // Check if this enemy is closer than previous closest and within attack range (5 units)
                if (distance < closestDistance && distance < 5f)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
            
            if (closestEnemy != null)
            {
                // Clean up name, remove Unity's clone suffix from instantiated objects
                return closestEnemy.name.Replace("(Clone)", "").Trim(); 
            }
            
            return "Enemy"; // Default name 
        }
    }
}