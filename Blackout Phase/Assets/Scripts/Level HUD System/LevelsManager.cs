// Warren

// The main purpose of this script is it creates a a complete XP and Leveling System that automatically tracks enemy kills, XP gained, and displays the Level HUD visually.
// When the player levels up, a big text will fade in briefly, and then fades away.

// Source: https://www.youtube.com/watch?v=Hd1xWdt3cP8 - Replicated from the YouTuber "Can With Code". Such as the IncreaseXP, UpdateHUD, and CheckForLevelUp method.
// Source: https://docs.unity3d.com/Manual/execution-order.html - Singleton pattern for managing different classes if leveling system is needed.
// Source: https://docs.unity3d.com/Manual/Coroutines.html - Animation timing and coroutines of the text display pop up.

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI currentLevelText;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] Image xpBar;
    
    [Header("Level Up Effect")]
    [SerializeField] private TextMeshProUGUI levelUpText; 
    [SerializeField] private float levelUpDisplayTime = 2f; 
    [SerializeField] private AnimationCurve levelUpAnimationCurve;
    
    [Space(10)]
    [Header("XP Settings")]
    [SerializeField] int targetXP = 100;
    [SerializeField] int targetXPIncrease = 50;
    [SerializeField] int xpPerEnemy = 25;

    int currentLevel;
    int currentXP;
    
    // For tracking the Enemies
    private List<GameObject> previousEnemies = new List<GameObject>();
    private float checkInterval = 0.5f;
    private float checkTimer = 0f;
    
    // Level Up effect
    private Coroutine levelUpCoroutine;

    // Singleton pattern implementation, so that any script can give XP without complication (if needed).
    public static LevelsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        currentLevel = 1;
        
        // Hides the level up text
        if (levelUpText != null)
        {
            levelUpText.gameObject.SetActive(false);
        }
        
        UpdateHUD();
        ScanForEnemies();
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer >= checkInterval)
        {
            checkTimer = 0f;
            CheckForEnemyDeaths(); 
        }
    }

    // This method tracks all of the enemies that are still alive in the scene.
    private void ScanForEnemies()
    {
        previousEnemies.Clear();
        EnemyInfo[] allEnemies = FindObjectsOfType<EnemyInfo>(); // Searches the entire game scene and finds all of the GameObject that has EnemyInfo component attached to it.
        
        // Goes through each of the found enemies one by one.
        foreach (EnemyInfo enemy in allEnemies)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                previousEnemies.Add(enemy.gameObject); // Empties the old list, and then stores the refernce to each enemy GameObject.
            }
        }
    }

    // This method detechs when the enemies die
    private void CheckForEnemyDeaths()
    {
        List<GameObject> currentEnemies = new List<GameObject>(); // Empty list of the current enemies still on the scene.
        EnemyInfo[] allEnemies = FindObjectsOfType<EnemyInfo>(); // Finds all enemies currently alive.
        foreach (EnemyInfo enemy in allEnemies)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                currentEnemies.Add(enemy.gameObject);
            }
        }

        // Loops through each enemy and checks of the enemy from the old list is still alive.
        foreach (GameObject oldEnemy in previousEnemies) 
        {
            if (oldEnemy == null || !currentEnemies.Contains(oldEnemy)) // If the old enemy is null, then the Enemy GameObject is destroyed OR the Enemy still exists, but not in the new list.
            {
                IncreaseXP(xpPerEnemy);
                Debug.Log($"Enemy defeated! Awarded {xpPerEnemy} XP");
            }
        }

        previousEnemies = currentEnemies; // Makes the new list into the old list for the next check.
    }

    // This method adds XP points and updates everything automatically.
    public void IncreaseXP(int amount)
    {
        currentXP += amount;
        CheckForLevelUp();
        UpdateHUD();
    }

    // This method checks if the player earned enough XP to level up, then it will display it on the screen that you've leveled up and what level you are now.
    private void CheckForLevelUp()
    {
        while(currentXP >= targetXP)
        {
            currentLevel++;
            currentXP -= targetXP;
            targetXP += targetXPIncrease;
            
            ShowLevelUpEffect();
            
            Debug.Log($"Level Up! Now level {currentLevel}");
        }
    }

    // This method updates the XP/level numbers displayed on the HUD.
    private void UpdateHUD()
    {
        if (currentLevelText != null)
            currentLevelText.text = "Level " + currentLevel;
        
        if (xpText != null)
            xpText.text = currentXP + "/" + targetXP;
        
        if (xpBar != null)
            xpBar.fillAmount = (float)currentXP / (float)targetXP;
    }
    
    // This method shows the level up visual effect.
    private void ShowLevelUpEffect()
    {
        if (levelUpText == null)
        {
            Debug.LogWarning("Level Up Text reference not set in LevelsManager!");
            return;
        }
        
        // Stop previous coroutine if still running
        if (levelUpCoroutine != null)
        {
            StopCoroutine(levelUpCoroutine);
        }
        
        // Start new level up effect
        levelUpCoroutine = StartCoroutine(LevelUpAnimation());
    }
    
    // This method plays a smooth level up animation through a curve, it fades in and then fades away.
    private IEnumerator LevelUpAnimation()
    {
        // Update text to show new level
        levelUpText.text = $"LEVEL UP! Now Level {currentLevel}";
        
        // Makees the text visible
        levelUpText.gameObject.SetActive(true);
        
        // Reset the alpha to 0
        Color textColor = levelUpText.color;
        textColor.a = 0f;
        levelUpText.color = textColor;
        
        // Scale reset
        levelUpText.transform.localScale = Vector3.zero;
        
        float timer = 0f;
        
        // Fade in and grows
        while (timer < 1f)
        {
            timer += Time.deltaTime / 0.5f; // 0.5 second fade in
            float curveValue = levelUpAnimationCurve != null ? 
                levelUpAnimationCurve.Evaluate(timer) : timer;
            
            // Fade in
            textColor.a = curveValue;
            levelUpText.color = textColor;
            
            // Scale up
            float scale = Mathf.Lerp(0.5f, 1f, curveValue);
            levelUpText.transform.localScale = new Vector3(scale, scale, scale);
            
            yield return null;
        }
        
        // Display time
        yield return new WaitForSeconds(levelUpDisplayTime);
        
        // Fade out
        timer = 0f;
        Color startColor = levelUpText.color;
        
        while (timer < 1f)
        {
            timer += Time.deltaTime / 0.5f; // Fade out time
            textColor = startColor;
            textColor.a = Mathf.Lerp(1f, 0f, timer);
            levelUpText.color = textColor;
            
            yield return null;
        }
        
        // Hides the text completely
        levelUpText.gameObject.SetActive(false);
    }
}