// Warren

// The main purpose of this script is it creates a a complete XP and Leveling System that automatically tracks enemy kills, XP gained, and displays the Level HUD visually.
// When the player levels up, a big text will fade in briefly, and then fades away.

// Source: https://www.youtube.com/watch?v=Hd1xWdt3cP8 - Replicated from the YouTuber "Can With Code". Such as the IncreaseXP, UpdateHUD, and CheckForLevelUp method.
// Source: https://docs.unity3d.com/Manual/execution-order.html - Singleton pattern for managing different classes if leveling system is needed.
// Source: https://docs.unity3d.com/Manual/Coroutines.html - Animation timing and coroutines of the text display pop up.
// Source: https://www.youtube.com/watch?v=SH25f3cXBVc - For upgrade and stats upgrade panel and system, from the YouTuber "Kryzarel".

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LevelsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TextMeshProUGUI currentLevelText;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] Image xpBar;
    
    [Header("Level Up Effect")]
    [SerializeField] private TextMeshProUGUI levelUpText; // Drag a TextMeshPro UI element here
    [SerializeField] private float levelUpDisplayTime = 2f; // How long to show "Level Up!"
    [SerializeField] private AnimationCurve levelUpAnimationCurve; // For fade in/out effect
    
    [Header("Level Up Choice UI")]
    [SerializeField] private GameObject levelUpChoicePanel; // Panel that contains the choice buttons
    [SerializeField] private Button[] choiceButtons = new Button[3]; // Array of 3 choice buttons
    [SerializeField] private TextMeshProUGUI[] choiceTexts = new TextMeshProUGUI[3]; // Text for each button
    
    [Header("Available Skills")]
    [SerializeField] private SkillData[] allSkills; // Drag all skill ScriptableObjects here
    
    [Header("Stat Increase Amounts")]
    [SerializeField] private int healthIncreaseAmount = 10; // HP increase per choice
    [SerializeField] private int energyIncreaseAmount = 5;  // EN increase per choice
    [SerializeField] private int attackIncreaseAmount = 2;  // Attack increase per choice
    
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
    
    // Track if we're showing level up choice
    private bool showingLevelUpChoice = false;
    
    // Track which choices are offered
    private LevelUpChoice[] currentChoices = new LevelUpChoice[3];
    
    // Track unlocked skills
    private List<SkillData> unlockedSkills = new List<SkillData>();

    // Singleton pattern implementation, so that any script can give XP without complication (if needed).
    public static LevelsManager Instance { get; private set; }

    // Enum for types of level up choices
    private enum ChoiceType
    {
        Skill,
        HealthIncrease,
        EnergyIncrease,
        AttackIncrease
    }

    // Class to store level up choice data
    private class LevelUpChoice
    {
        public ChoiceType type;
        public SkillData skill; // Only for Skill type
        public string title;
        public string description;
    }

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
        
        // Hides the level up text and choice panel
        if (levelUpText != null)
        {
            levelUpText.gameObject.SetActive(false);
        }
        
        if (levelUpChoicePanel != null)
        {
            levelUpChoicePanel.SetActive(false);
        }
        
        // Initialize unlocked skills (start with basic attack if exists)
        InitializeStartingSkills();
        
        UpdateHUD();
        ScanForEnemies();
        
        // Setup button listeners
        SetupChoiceButtons();
    }

    private void Update()
    {
        // Only check for enemy deaths if we're not showing level up choice
        if (!showingLevelUpChoice)
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkInterval)
            {
                checkTimer = 0f;
                CheckForEnemyDeaths(); 
            }
        }
    }

    // Initialize starting skills (like basic attack)
    private void InitializeStartingSkills()
    {
        // Find basic attack skill
        SkillData basicAttack = System.Array.Find(allSkills, skill => skill.id == Skill_ID.BasicAttack);
        if (basicAttack != null && !unlockedSkills.Contains(basicAttack))
        {
            unlockedSkills.Add(basicAttack);
            Debug.Log($"Starting skill unlocked: {basicAttack.Attack}");
        }
    }

    // This method tracks all of the enemies that are still alive in the scene.
    private void ScanForEnemies()
    {
        previousEnemies.Clear();
        EnemyInfo[] allEnemies = FindObjectsOfType<EnemyInfo>();
        
        foreach (EnemyInfo enemy in allEnemies)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                previousEnemies.Add(enemy.gameObject);
            }
        }
    }

    // This method detects when the enemies die
    private void CheckForEnemyDeaths()
    {
        List<GameObject> currentEnemies = new List<GameObject>();
        EnemyInfo[] allEnemies = FindObjectsOfType<EnemyInfo>();
        foreach (EnemyInfo enemy in allEnemies)
        {
            if (enemy != null && enemy.gameObject != null)
            {
                currentEnemies.Add(enemy.gameObject);
            }
        }

        foreach (GameObject oldEnemy in previousEnemies) 
        {
            if (oldEnemy == null || !currentEnemies.Contains(oldEnemy))
            {
                IncreaseXP(xpPerEnemy);
                Debug.Log($"Enemy defeated! Awarded {xpPerEnemy} XP");
            }
        }

        previousEnemies = currentEnemies;
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
            
            // Show level up celebration first
            ShowLevelUpEffect();
            
            // Then show the choice panel after celebration
            StartCoroutine(ShowLevelUpChoiceAfterDelay());
            
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
        
        if (levelUpCoroutine != null)
        {
            StopCoroutine(levelUpCoroutine);
        }
        
        levelUpCoroutine = StartCoroutine(LevelUpAnimation());
    }
    
    // This method plays a smooth level up animation through a curve, it fades in and then fades away.
    private IEnumerator LevelUpAnimation()
    {
        levelUpText.text = $"LEVEL UP! Now Level {currentLevel}";
        levelUpText.gameObject.SetActive(true);
        
        Color textColor = levelUpText.color;
        textColor.a = 0f;
        levelUpText.color = textColor;
        levelUpText.transform.localScale = Vector3.zero;
        
        float timer = 0f;
        
        while (timer < 1f)
        {
            timer += Time.deltaTime / 0.5f;
            float curveValue = levelUpAnimationCurve != null ? 
                levelUpAnimationCurve.Evaluate(timer) : timer;
            
            textColor.a = curveValue;
            levelUpText.color = textColor;
            
            float scale = Mathf.Lerp(0.5f, 1f, curveValue);
            levelUpText.transform.localScale = new Vector3(scale, scale, scale);
            
            yield return null;
        }
        
        yield return new WaitForSeconds(levelUpDisplayTime);
        
        timer = 0f;
        Color startColor = levelUpText.color;
        
        while (timer < 1f)
        {
            timer += Time.deltaTime / 0.5f;
            textColor = startColor;
            textColor.a = Mathf.Lerp(1f, 0f, timer);
            levelUpText.color = textColor;
            
            yield return null;
        }
        
        levelUpText.gameObject.SetActive(false);
    }
    
    // Shows level up choice panel after leveling up
    private IEnumerator ShowLevelUpChoiceAfterDelay()
    {
        yield return new WaitForSeconds(levelUpDisplayTime + 1f);
        
        // Generate choices of skills or stat increases.
        GenerateLevelUpChoices();
        
        ShowLevelUpChoice();
    }
    
    // Generate 3 random choices (skills or stat increases)
    private void GenerateLevelUpChoices()
    {
        List<LevelUpChoice> allPossibleChoices = new List<LevelUpChoice>();
        
        // Add available skills
        foreach (SkillData skill in allSkills)
        {
            if (!unlockedSkills.Contains(skill) && AreSkillRequirementsMet(skill))
            {
                LevelUpChoice skillChoice = new LevelUpChoice
                {
                    type = ChoiceType.Skill,
                    skill = skill,
                    title = skill.Attack,
                    description = $"{skill.AttkDescription}\nCost: {skill.AttkAPCost} AP, {skill.AttkENCost} EN"
                };
                allPossibleChoices.Add(skillChoice);
            }
        }
        
        // Add stat increases (always available)
        LevelUpChoice healthChoice = new LevelUpChoice
        {
            type = ChoiceType.HealthIncrease,
            title = "Increase Max HP",
            description = $"by {healthIncreaseAmount}"
        };
        allPossibleChoices.Add(healthChoice);
        
        LevelUpChoice energyChoice = new LevelUpChoice
        {
            type = ChoiceType.EnergyIncrease,
            title = "Increase Max EN",
            description = $"by {energyIncreaseAmount}"
        };
        allPossibleChoices.Add(energyChoice);
        
        LevelUpChoice attackChoice = new LevelUpChoice
        {
            type = ChoiceType.AttackIncrease,
            title = "Increase Attack",
            description = $"by {attackIncreaseAmount}"
        };
        allPossibleChoices.Add(attackChoice);
        
        // Shuffle and select 3 choices
        System.Random rng = new System.Random();
        allPossibleChoices = allPossibleChoices.OrderBy(x => rng.Next()).ToList();
        
        int choicesToOffer = Mathf.Min(3, allPossibleChoices.Count);
        for (int i = 0; i < choicesToOffer; i++)
        {
            currentChoices[i] = allPossibleChoices[i];
        }
        
        // Clear any unused slots
        for (int i = choicesToOffer; i < 3; i++)
        {
            currentChoices[i] = null;
        }
    }
    
    // Check if skill requirements are met
    private bool AreSkillRequirementsMet(SkillData skill)
    {
        if (skill.requirements == null || skill.requirements.Length == 0)
        {
            return true;
        }
            
        foreach (Skill_ID requiredSkill in skill.requirements)
        {
            bool hasRequired = unlockedSkills.Exists(s => s.id == requiredSkill);
            if (!hasRequired)
            {
                return false;
            }
        }
        
        return true;
    }
    
    // Shows the level up choice panel and pauses the game
    private void ShowLevelUpChoice()
    {
        if (levelUpChoicePanel == null)
        {
            Debug.LogWarning("Level Up Choice Panel not set!");
            return;
        }
        
        showingLevelUpChoice = true;
        Time.timeScale = 0f;
        
        // Update button texts with choice information
        for (int i = 0; i < 3; i++)
        {
            if (currentChoices[i] != null && choiceButtons[i] != null)
            {
                // Enable button and set text
                choiceButtons[i].gameObject.SetActive(true);
                
                if (choiceTexts[i] != null)
                {
                    LevelUpChoice choice = currentChoices[i];
                    choiceTexts[i].text = $"{choice.title}\n{choice.description}";
                }
            }
            else if (choiceButtons[i] != null)
            {
                // No choice for this slot, disable button
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
        
        levelUpChoicePanel.SetActive(true);
        Debug.Log("Level Up Choice Panel Shown - Choose a reward!");
    }
    
    // Hides the level up choice panel and resumes the game
    private void HideLevelUpChoice()
    {
        if (levelUpChoicePanel != null)
        {
            levelUpChoicePanel.SetActive(false);
        }
        
        Time.timeScale = 1f;
        showingLevelUpChoice = false;
        
        // Clear current choices
        for (int i = 0; i < 3; i++)
        {
            currentChoices[i] = null;
        }
        
        Debug.Log("Level Up Choice Panel Hidden - Game Resumed");
    }
    
    // Setup button click listeners
    private void SetupChoiceButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            int index = i; // Important: Capture index for closure
            if (choiceButtons[i] != null)
            {
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => MakeChoice(index));
            }
        }
    }
    
    // Player makes a choice
    private void MakeChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= 3 || currentChoices[choiceIndex] == null)
        {
            Debug.LogWarning("Invalid choice!");
            HideLevelUpChoice();
            return;
        }
        
        LevelUpChoice choice = currentChoices[choiceIndex];
        
        switch (choice.type)
        {
            case ChoiceType.Skill:
                UnlockSkill(choice.skill);
                break;
                
            case ChoiceType.HealthIncrease:
                IncreasePlayerHealth();
                break;
                
            case ChoiceType.EnergyIncrease:
                IncreasePlayerEnergy();
                break;
                
            case ChoiceType.AttackIncrease:
                IncreasePlayerAttack();
                break;
        }
        
        HideLevelUpChoice();
    }
    
    // Unlock a skill
    private void UnlockSkill(SkillData skill)
    {
        if (!unlockedSkills.Contains(skill))
        {
            unlockedSkills.Add(skill);
            Debug.Log($"Unlocked skill: {skill.Attack}");
            
            // TODO: Notify skill system
            // SkillSystem.Instance?.AddSkill(skill); 
            // Depends on SkillSystem script, unsure if it will work.
        }
    }
    
    // Increase player health
    private void IncreasePlayerHealth()
    {
        CharacterInfo1 player = CharacterInfo1.Instance;
        if (player != null)
        {
            player.IncreaseMaxHP(healthIncreaseAmount);
            
            Debug.Log($"Max HP increased by {healthIncreaseAmount}");
        }
    }
    
    // Increase player energy points
    private void IncreasePlayerEnergy()
    {
        CharacterInfo1 player = CharacterInfo1.Instance;
        if (player != null)
        {
            player.IncreaseMaxEN(energyIncreaseAmount);
            
            Debug.Log($"Max EN increased by {energyIncreaseAmount}");
        }
    }
    
    // Increase player attack
    private void IncreasePlayerAttack()
    {
        CharacterInfo1 player = CharacterInfo1.Instance;
        if (player != null)
        {
            player.IncreaseAttack(attackIncreaseAmount);
            
            Debug.Log($"Attack increased by {attackIncreaseAmount}");
        }
    }
    
    // This method to checks if a skill is unlocked
    public bool IsSkillUnlocked(Skill_ID skillId)
    {
        return unlockedSkills.Exists(skill => skill.id == skillId);
    }
    
    // This method gets all unlocked skills
    public SkillData[] GetUnlockedSkills()
    {
        return unlockedSkills.ToArray();
    }
}