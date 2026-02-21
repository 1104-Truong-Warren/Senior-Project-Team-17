// Warren
// The purpose of this script is to observe player health changes and display damage notifications in the UI.
// When the player takes damage, it shows how much damage was dealt and attempts to shows that the enemy attacked.
// The notification displays for 2 seconds, and then automatically disappears.

// Source: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html - For  health monitoring
// Source: https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html - For showing/hiding UI elements
// Source: https://docs.unity3d.com/ScriptReference/Object.FindObjectsOfType.html - For finding attacking enemies
// Source: https://docs.unity3d.com/ScriptReference/Vector3.Distance.html - For calculating closest enemy distance
// Source: https://docs.unity3d.com/ScriptReference/Camera.WorldToScreenPoint.html - Converting world position to screen position
// Source: https://docs.unity3d.com/ScriptReference/RectTransform-position.html - Setting UI element position

using UnityEngine;
using TMPro;
using System.Collections; // For IEnumerator

public class DamageObserver : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI damageText; 
    [SerializeField] private TextMeshProUGUI attackerText; 
    
    [Header("Animation")]
    [SerializeField] private Animation damageAnimation; 
    [SerializeField] private Animation attackerAnimation; 
    
    [Header("Settings")]
    [SerializeField] private float displayTime = 2f; // How long damage notification stays visible, in this case, 2 seconds.
    [SerializeField] private float appearDelay = 0.1f; // Small delay before text appears (adjustable in Inspector)
    
    // Added option to position text above player
    [Header("2D Position Offset")]
    [SerializeField] private Vector2 damageOffset = new Vector2(0, 50); 
    [SerializeField] private Vector2 attackerOffset = new Vector2(0, 80); 
    [SerializeField] private Vector2 playerDamageOffset = new Vector2(0, 30); // Separate offset for player attacks on enemies
    [SerializeField] private Vector2 dodgeOffset = new Vector2(0, 40); // Separate offset for dodge text above player

    [Header("Player Damage Settings")]
    [SerializeField] private bool showPlayerDamage = true;
    [SerializeField] private TextMeshProUGUI playerDamageText;
    
    // Prefab for spawning player damage text
    [Header("Player Damage Prefab")]
    [SerializeField] private GameObject playerDamagePrefab; // Assign the duplicated Damage Text prefab here
    [SerializeField] private Transform canvasTransform; // Reference to the Canvas for spawning
    
    // Singleton for easy access from other scripts
    public static DamageObserver Instance { get; private set; }
    
    private int lastPlayerHP; // Player's HP from previous frame
    private float hideTime; // Amount of time the UI will be hidden       
    private bool isShowing = false;
    
    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        // Get initial HP, provides access to player's health from CharacterInfo1
        if (CharacterInfo1.Instance != null)
        {
            lastPlayerHP = CharacterInfo1.Instance.CurrentHP;
            Debug.Log($"DamageObserver: Started. Player HP = {lastPlayerHP}");
        }
        
        // Hide text initially, GameObjects are inactive until damage occurs
        if (damageText != null) damageText.gameObject.SetActive(false);
        if (attackerText != null) attackerText.gameObject.SetActive(false);
        
        // If canvasTransform not set, try to find it automatically
        if (canvasTransform == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
                canvasTransform = canvas.transform;
        }
    }
    
    void Update()
    {
        // Makes sure that CharacterInfo1 exists before accessing it
        if (CharacterInfo1.Instance == null)
        {
            Debug.LogWarning("DamageObserver: CharacterInfo1.Instance is null");
            return;
        }
        
        int currentHP = CharacterInfo1.Instance.CurrentHP;
        
        // Check if player took damage by comparing with previous frame's HP
        if (currentHP < lastPlayerHP)
        {
            int damage = lastPlayerHP - currentHP; // Calculate damage amount
            StartCoroutine(ShowDamageWithDelay(damage)); // CHANGED: Use coroutine with delay
            lastPlayerHP = currentHP; // Update stored HP
            
            Debug.Log($"DamageObserver: Player took {damage} damage. HP now {currentHP}");
        }
        else if (currentHP > lastPlayerHP)
        {
            lastPlayerHP = currentHP; // HP increased, update stored HP without showing notification
        }
        
        // Update text positions to follow player in 2D
        if (isShowing)
        {
            Update2DPositions();
        }
        
        // Hide text after display time 
        // Time.time gives current game time in seconds
        if (isShowing && Time.time > hideTime)
        {
            HideDamage();
        }
    }
    
    // Coroutine to add delay before showing damage
    IEnumerator ShowDamageWithDelay(int damage)
    {
        // Wait for the specified delay (adjustable in Inspector)
        yield return new WaitForSeconds(appearDelay);
        
        // Now show the damage
        ShowDamage(damage);
    }
    
    // Displays damage notification with amount and enemy name
    void ShowDamage(int damage)
    {
        // Finds which enemy attacked by checking proximity to player
        string attackerName = FindAttackingEnemy();
        
        if (damageText != null)
        {
            damageText.text = $"-{damage} HP"; // Format: "-10 HP"
            damageText.gameObject.SetActive(true);
            
            // Play damage text animation
            if (damageAnimation != null)
            {
                damageAnimation.Play();
                Debug.Log("Playing damage text animation");
            }
        }
        
        if (attackerText != null)
        {
            attackerText.text = $"{attackerName} attacks!";
            attackerText.gameObject.SetActive(true);
            
            // Play attacker text animation
            if (attackerAnimation != null)
            {
                attackerAnimation.Play();
                Debug.Log("Playing attacker text animation");
            }
        }
        
        // This function allows the text to follow the player in the scene, and have the text appear above the player's head.
        Update2DPositions();
        
        // Set timer, such as the hide time
        hideTime = Time.time + displayTime;
        isShowing = true;
        
        Debug.Log($"DamageObserver: {attackerName} hit for {damage} damage");
      
    }
    
    // This function allows the text to follow the player in the scene, and have the text appear above the player's head.
    void Update2DPositions()
    {
        if (CharacterInfo1.Instance == null || Camera.main == null) return;
        
        // Get player's position in 2D screen space
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(CharacterInfo1.Instance.transform.position);
        
        if (damageText != null && damageText.gameObject.activeSelf)
        {
            // 2D offset
            Vector3 damagePos = new Vector3(
                playerScreenPos.x + damageOffset.x,
                playerScreenPos.y + damageOffset.y,
                playerScreenPos.z
            );
            damageText.rectTransform.position = damagePos;
        }
        
        if (attackerText != null && attackerText.gameObject.activeSelf)
        {
            // 2D offset
            Vector3 attackerPos = new Vector3(
                playerScreenPos.x + attackerOffset.x,
                playerScreenPos.y + attackerOffset.y,
                playerScreenPos.z
            );
            attackerText.rectTransform.position = attackerPos;
        }
    }
    
    // Hides the damage notification UI elements
    void HideDamage()
    {
        if (damageText != null) damageText.gameObject.SetActive(false);
        if (attackerText != null) attackerText.gameObject.SetActive(false);
        isShowing = false;
    }
    
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

    // Add this public method that PlayerCombatCheck can call
    public void ShowPlayerDamage(int damage, Vector3 enemyPosition)
    {
        if (!showPlayerDamage) return;
        
        // MODIFIED: Use prefab spawning instead of single text instance
        // This allows multiple damage numbers to appear simultaneously
        
        // Make sure we have a prefab and canvas
        if (playerDamagePrefab == null || canvasTransform == null)
        {
            Debug.LogError("PlayerDamagePrefab or CanvasTransform not assigned in DamageObserver!");
            return;
        }
        
        // Convert enemy world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(enemyPosition + Vector3.up * 2f);
        
        // Spawn a new damage text instance
        GameObject damageInstance = Instantiate(playerDamagePrefab, canvasTransform);
        TextMeshProUGUI damageTMP = damageInstance.GetComponent<TextMeshProUGUI>();
        
        if (damageTMP != null)
        {
            // Position the text with the SEPARATE player damage offset
            damageTMP.rectTransform.position = screenPos + new Vector3(playerDamageOffset.x, playerDamageOffset.y, 0);
            
            // Set the text
            damageTMP.text = $"-{damage} HP";
        }
        
        // Play the animation if it exists (using the same animation name as enemy damage)
        Animation anim = damageInstance.GetComponent<Animation>();
        if (anim != null)
        {
            anim.Play("DamageTextBounce");
        }
        
        // Destroy after delay to clean up
        StartCoroutine(HidePlayerDamage(damageInstance));
    }

    // Show "Miss!" text when player misses an attack
    public void ShowMissText(Vector3 enemyPosition)
    {
        Debug.Log($"ShowMissText called at position: {enemyPosition}"); 
        
        if (!showPlayerDamage) 
        {
            Debug.Log("ShowMissText: showPlayerDamage is false");
            return;
        }
        
        // Make sure we have a prefab and canvas
        if (playerDamagePrefab == null || canvasTransform == null)
        {
            Debug.LogError($"PlayerDamagePrefab or CanvasTransform not assigned! Prefab: {playerDamagePrefab}, Canvas: {canvasTransform}");
            return;
        }
        
        // Convert enemy world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(enemyPosition + Vector3.up * 2f);
        Debug.Log($"Screen position: {screenPos}"); 
        
        // Spawn a new text instance
        GameObject missInstance = Instantiate(playerDamagePrefab, canvasTransform);
        TextMeshProUGUI missTMP = missInstance.GetComponent<TextMeshProUGUI>();
        
        if (missTMP != null)
        {
            // Position the text with the same offset as damage numbers
            missTMP.rectTransform.position = screenPos + new Vector3(playerDamageOffset.x, playerDamageOffset.y, 0);
            
            // Set the text to "Miss!"
            missTMP.text = "Miss!";
            
            // Optional: Change color for miss (gray/white)
            missTMP.color = Color.gray;
            
            Debug.Log($"Miss text set and positioned at: {missTMP.rectTransform.position}");
        }
        else
        {
            Debug.LogError("Miss text: TextMeshProUGUI component not found on prefab!");
        }
        
        // Play the animation
        Animation anim = missInstance.GetComponent<Animation>();
        if (anim != null)
        {
            anim.Play("DamageTextBounce");
            Debug.Log("Animation playing"); 
        }
        else
        {
            Debug.LogWarning("Miss text: No Animation component found"); 
        }
        
        // Destroy after delay
        StartCoroutine(HidePlayerDamage(missInstance));
    }

    // Show "Dodged!" text when player dodges an enemy attack (appears above player)
    public void ShowDodgedText(Vector3 playerPosition)
    {
        // Make sure we have a prefab and canvas
        if (playerDamagePrefab == null || canvasTransform == null)
        {
            Debug.LogError("PlayerDamagePrefab or CanvasTransform not assigned in DamageObserver!");
            return;
        }
        
        // Convert player world position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerPosition + Vector3.up * 2f);
        
        // Spawn a new text instance
        GameObject dodgeInstance = Instantiate(playerDamagePrefab, canvasTransform);
        TextMeshProUGUI dodgeTMP = dodgeInstance.GetComponent<TextMeshProUGUI>();
        
        if (dodgeTMP != null)
        {
            // Position the text with the SEPARATE dodge offset
            dodgeTMP.rectTransform.position = screenPos + new Vector3(dodgeOffset.x, dodgeOffset.y, 0);
            
            // Set the text to "Dodged!"
            dodgeTMP.text = "Dodged!";
            
            // Optional: Change color for dodge (green/blue)
            dodgeTMP.color = Color.cyan;
        }
        
        // Play the animation
        Animation anim = dodgeInstance.GetComponent<Animation>();
        if (anim != null)
        {
            anim.Play("DamageTextBounce");
        }
        
        // Destroy after delay
        StartCoroutine(HidePlayerDamage(dodgeInstance));
    }

    IEnumerator HidePlayerDamage(GameObject damageInstance)
    {
        yield return new WaitForSeconds(displayTime);
        if (damageInstance != null)
            Destroy(damageInstance);
    }
}