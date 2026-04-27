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
using UnityEditor.Experimental.GraphView;
using log4net.Core;

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

    // Reference to the attached skills
    // Weijun
    //[Header("Player Attachment")]
    [SerializeField] private SkillAttachment attachment; // accessor for player skills

    [Header("New Unlocked Skills")]
    [SerializeField] private List<SkillData> newUnlockedActiveSkills = new List<SkillData>(); // list of newly unlocked skills
    public List<SkillData> NewUnlockedActiveSkills => newUnlockedActiveSkills; // accessor for other scripts;

    private bool startSkillsInitialized = false; // flag to check when can player skill attach can be initialized

    private int currentLevel;
    private int currentXP;
    
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
    //private List<SkillData> unlockedSkills = new List<SkillData>();

    // Singleton pattern implementation, so that any script can give XP without complication (if needed).
    public static LevelsManager Instance { get; private set; }

    // Enum for types of level up choices
    private enum ChoiceType
    {
        //Skill, not skill selection when leveling up
        HealthIncrease,
        EnergyIncrease,
        AttackIncrease
    }

    // Class to store level up choice data
    private class LevelUpChoice
    {
        public ChoiceType type;
        //public SkillData skill; // Only for Skill type , levels only give EN, HP, Attack
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
        //InitializeStartingSkills();

        UpdateHUD();
        ScanForEnemies();

        // Setup button listeners
        SetupChoiceButtons();
    }
    private void Start()
    {
        Debug.Log("LevelManager Start!"); // debug msg

        SkillAttachment attachment = GetPlayerSkillAttachment(); // accessor to player attachment

        // check to see if attachment exist
        if (attachment == null) return;

        GivePlayerStartingSkills(); // unlock the starter skills for player

        UnlockSkillsDependingOnCurrentLevel(); // check what skills can be unlocked

        attachment.RemoveLockedSkills(); // removed the skills from slot if level not met

        //AutoEquipUnlockedSkillsTest(); // equip the skills
    }

    private void Update()
    {
        // if the skill initialize flag is false try to initialize the skills
        if (!startSkillsInitialized)
            TryInitializeStarterSkills();
       

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

        // Level test if L is pressed
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("[LevelManager] EXP Debug key pressed!"); // debug msg

            IncreaseXP(100);
        } 
    }

    // Initialize starting skills (like basic attack)
    // Addon
    // Weijun
    //private void InitializeStartingSkills()
    //{
    //    // Find basic attack skill
    //    SkillData basicAttack = System.Array.Find(allSkills, skill => skill.id == Skill_ID.NormalAttack);

    //    if (basicAttack != null && !unlockedSkills.Contains(basicAttack))
    //    {
    //        unlockedSkills.Add(basicAttack);

    //        // player attachment exsit equit the skill
    //        if (playerSkillAttachment != null)
    //        {
    //            playerSkillAttachment.UnlockSkill(basicAttack); // unlock the skill

    //            playerSkillAttachment.EquipActiveSkillToSlot(basicAttack, 0); // equip the skill
    //        }
    //        Debug.Log($"Starting skill unlocked: {basicAttack.AttackDamage}");
    //    }
    //}

    // This method tracks all of the enemies that are still alive in the scene.
    private void ScanForEnemies()
    {
        previousEnemies.Clear();
        EnemyInfo[] allEnemies = FindObjectsByType<EnemyInfo>(FindObjectsSortMode.None);
        
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
        EnemyInfo[] allEnemies = FindObjectsByType<EnemyInfo>(FindObjectsSortMode.None);
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

    // This method checks if the player earned enough XP to level up,
    // then it will display it on the screen that you've leveled up and what level you are now.
    // Addon
    // Weijun
    private void CheckForLevelUp()
    {
        while(currentXP >= targetXP)
        {
            currentLevel++;
            currentXP -= targetXP;
            targetXP += targetXPIncrease;

            AddBaseStats();  // give player base stats increase when leveled up

            attachment = GetPlayerSkillAttachment(); // set up the attach ment

            UnlockSkillsDependingOnCurrentLevel(); // check for new unlockable skill

            attachment.RemoveLockedSkills(); // removed the skills from slot if level not met

            OpenSkilEquipMenu(); // show player the skill equipment menu

            //AutoEquipUnlockedSkillsTest(); // equip the skills

            CharacterInfo1 player = CharacterInfo1.Instance; // set up the copy of the playerInfo

            // if player found keep track of the player's level
            if (player != null)
                player.LevelUp();

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
    // new version only needs HP, EN, Attack because skill is unlocked by level
    // Weijun changed
    private void GenerateLevelUpChoices()
    {
        currentChoices[0] = new LevelUpChoice
        {
            type = ChoiceType.HealthIncrease,
            title = "Increase Max HP",
            description = $"+{healthIncreaseAmount} Max HP"
        };

        currentChoices[1] = new LevelUpChoice
        {
            type = ChoiceType.EnergyIncrease,
            title = "Increase Max EN",
            description = $"+{energyIncreaseAmount} Max EN"
        };

        currentChoices[2] = new LevelUpChoice
        {
            type = ChoiceType.AttackIncrease,
            title = "Increase Max Attack",
            description = $"+{attackIncreaseAmount} Max Attack"
        };
    }
    
    // Check if skill requirements are met
    // Added some more requirement checks
    // Weijun
    private bool AreSkillRequirementsMet(SkillData skill)
    {
        attachment = GetPlayerSkillAttachment(); // set up the attach ment

        // check to see if skill and skill names exist
        if (skill == null || attachment == null) return false; 

        // if it doesn't required skills return true
        if (skill.requirdSkills == null || skill.requirdSkills.Length == 0)
        {
            return true;
        }
        
        // loop through the skill ID to find the required skill name
        foreach (Skill_ID requiredSkill in skill.requirdSkills)
        {
            bool hasRequired = attachment.UnlockedActiveSkills.Exists(s => s != null && s.id == requiredSkill);

            Debug.Log($"[LM] Skill prerequisite for{skill.skillDisplayName} needs:{requiredSkill} | HasRequired:{hasRequired}"); // debug msg

            // did it find it? if not return false
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
            // no longer needed 
            //case ChoiceType.Skill:
            //    UnlockSkill(choice.skill);
            //    break;
                
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

    // base stats added when leveled up
    // weijun
    private void AddBaseStats()
    {
        // increase the base stats
        IncreasePlayerAttack();

        IncreasePlayerHealth();

        IncreasePlayerEnergy();
    }

    // Unlocking skills according to level
    // Weijun
    private void UnlockSkillsDependingOnCurrentLevel()
    {
        Debug.Log("[LM] UnlockSkills called"); // debug msg

        attachment = GetPlayerSkillAttachment(); // accessor for player attachment

        // does the skill attachment exist?
        if (attachment == null) return;

        newUnlockedActiveSkills.Clear(); // clear the new skill list

        // loop through the skill data to find the skill
        foreach (SkillData skill in allSkills)
        {
            // if skill is not found keep on going
            if (skill == null) continue;

            // if the skill meets level requirement and if the skill can be found
            if (currentLevel >= skill.requiredLevel && AreSkillRequirementsMet(skill))
            {
                Debug.Log($"[LM] Checking unlockable skill:{skill.skillDisplayName} | requiredLevel:{skill.requiredLevel} | currentLevel:{currentLevel}"); // debug msg

                // if the unlock list does contian the skill add it
                if (!attachment.HasUnlockedSkill(skill))
                {
                    //unlockedSkills.Add(skill); // add skill to unlock list

                    attachment.UnlockSkill(skill); // add the skill to skill attachment list

                    // add the skill to the new skill list
                    if (skill.skillType == SkillType.Active)
                    {
                        newUnlockedActiveSkills.Add(skill);

                        Debug.Log($"[LM] Unlocked Active skill:{skill.skillDisplayName}"); // debug msg
                    }

                    else if (skill.skillType == SkillType.Passive)
                    {
                        CharacterInfo1 player = CharacterInfo1.Instance; // copies over the player info

                        player.LoadPassiveModFromSA(attachment); // load in the skills 

                        Debug.Log($"[LM] Unlocked Passive skill:{skill.skillDisplayName}"); // debug msg
                    }
                }
            }
        }

        //// skill test 
        //if (newUnlockedActiveSkills.Count > 0)
        //{
        //    int emptySlot = attachment.GetEmptySkillSlot(); // find the empty slot

        //    Debug.Log($"[LM] Empty skill slot:{emptySlot + 1}"); // debug msg

        //    //
        //    if (emptySlot != -1)
        //    {
        //        attachment.EquipActiveSkillToSlot(newUnlockedActiveSkills[0], emptySlot);
        //    }
        //}

        //Debug.Log($"[LM] SkillAttachment reference:{attachment.GetInstanceID()}"); // debug msg

        //Debug.Log($"[LM] Live player obj:{attachment.gameObject.name}"); // debug msg

        //attachment.DebugPrintSkillSlots(); // test to see the skill slots
    }

    // give player the base skills
    // version 3 
    // Weijun
    private void GivePlayerStartingSkills()
    {
        //Debug.Log("[LM] Start GivePlayerStartingSkills..."); // debug msg

        attachment = GetPlayerSkillAttachment(); // accessor for player attachment

        Debug.Log($"[LM] Attachment Instance:{attachment.GetInstanceID()}"); // debug msg

        // check to see if skill attachment is null
        if (attachment == null)
        {
            Debug.Log($"[LM] attachment is null in GivePlayerStartingSkills()"); // debug msg
            return;
        }

        // loop through the skills to find the starting skills by name
        foreach (SkillData skill in allSkills)
        {
            // if skill doesn't exist skip
            if (skill == null) continue;

            // if the skill id is equal to the starter skills add it to player skill attachment
            if (skill.id == Skill_ID.MechaCheck || skill.id == Skill_ID.NormalAttack)
            {
                // if the skill is not unlocked yet add it
                if (!attachment.HasUnlockedSkill(skill))
                {
                    //unlockedSkills.Add(skill); // add it to the unlocked list

                    attachment.UnlockSkill(skill); // add it to the attachment
                }
            }
        }

        // equip the starter skills
        SkillData mechaScan = attachment.UnlockedActiveSkills.Find(s => s != null && s.id == Skill_ID.MechaCheck);

        SkillData normalAttk = attachment.UnlockedActiveSkills.Find(s => s != null && s.id == Skill_ID.NormalAttack);

        // equip the skill if found
        if (mechaScan != null)
            attachment.EquipActiveSkillToSlot(mechaScan, 0);

        // equip the skill if found
        if (normalAttk != null)
            attachment.EquipActiveSkillToSlot(normalAttk, 1);
    }

    // initializes the starting skills
    // Weijun
    private void TryInitializeStarterSkills()
    {
        // if the flag is true return, defualt it's false
        if (startSkillsInitialized) return;

        attachment = GetPlayerSkillAttachment(); // accessor to the skill attachment

        CharacterInfo1 player = CharacterInfo1.Instance; // access the player info

        // check to see if attachment or player is initialized
        if (attachment == null || player == null)
        {
            Debug.Log("[LM] Starter skills initialize skipped: attachment/player not ready"); // debug msg
            return;
        }

        //Debug.Log("[LM] Before GivePlayerStartingSKills..."); // debug msg

        GivePlayerStartingSkills(); // call starter skill set up

        //Debug.Log("[LM] Returned from GivePlayerStartingSKills..."); // debug msg

        attachment.DebugPrintSkillSlots(); // shows all the skills which slot is on

        // new check for skill UI
        SkillEquipmentUI UI = FindFirstObjectByType<SkillEquipmentUI>(); //(FindObjectsInactive.Include); // search for inactive object too

        // UI is found do a load and refresh the UI
        if (UI != null)
        {
            UI.ShowPanel();

            Debug.Log("[LM] SkillPanel active using ShowPanel}"); // debug msg

            UI.LoadPlayerAndRefresh();
        }

        startSkillsInitialized = true; // falg is true 
    }

    private void OpenSkilEquipMenu()
    {
        // new check for skill UI
        SkillEquipmentUI UI = FindFirstObjectByType<SkillEquipmentUI>(); //(FindObjectsInactive.Include); // search for inactive object too

        Debug.Log("[LM] Unlocked a new skill! Showing EquipMenu}"); // debug msg

        // UI is found do a load and refresh the UI
        if (UI != null)
        {
            UI.ShowPanel();

            UI.RefreshUI();
        }
    }

    // use the up to date helper for player skill attachment
    // Weijun
    private SkillAttachment GetPlayerSkillAttachment()
    {
        CharacterInfo1 player = CharacterInfo1.Instance; // access the player info

        // player not found get out
        if (player == null) return null;   
        
        return player.GetComponent<SkillAttachment>(); // return the reference
    }

    // helper function to check the newly added active skills
    // Weijun
    public bool HasNewUnlockedActiveSkills()
    {
        return newUnlockedActiveSkills.Count > 0; 
    }

    // helper function to clear the newly added active skills list
    // Weijun
    public void ClearNewUnlockedActiveSkills()
    {
        newUnlockedActiveSkills.Clear(); 
    }

    // This method to checks if a skill is unlocked
    public bool IsSkillUnlocked(Skill_ID skillId)
    {
        return attachment.HasUnlockedSkillID(skillId);
    }
    
    //// This method gets all unlocked skills
    //public SkillData[] GetUnlockedSkills()
    //{
    //    return unlockedSkills.ToArray();
    //}
}

//// Unlock a skill
///  not needed in SkillAttachment
//// Addons 
//// Weijun
//private void UnlockSkill(SkillData skill)
//{
//    attachment = GetPlayerSkillAttachment(); // set up the attach ment

//    // check if attachment is empty
//    if (attachment == null) return;

//    // skill exist?
//    if (skill == null) return;

//    if (!unlockedSkills.Contains(skill))
//    {
//        unlockedSkills.Add(skill);
//        Debug.Log($"Unlocked skill: {skill.AttackDamage}");

//        // playerSkillAttachment found 
//        if (attachment != null)
//        {
//            attachment.UnlockSkill(skill); // check the skill to unlock
//        }

//        Debug.Log($"Unlocked skill:{skill.skillDisplayName}");

//        // TODO: Notify skill system
//        // SkillSystem.Instance?.AddSkill(skill); 
//        // Depends on SkillSystem script, unsure if it will work.
//    }
//}

// Test for auto skill equipment 
// Weijun
//private void AutoEquipUnlockedSkillsTest()
//{
//    Debug.Log("AutoEquipSkills called"); // debug msg

//    // attachment skill exist?
//    if (playerSkillAttachment == null) return;

//    // using loop to go through the skill and equip the ones meets the condition, player lvl, has skills
//    foreach (SkillData skill in playerSkillAttachment.UnlockedActiveSkills)
//    {
//        // skill not found skip
//        if (skill == null) continue;

//        // ignores the passive skills
//        if (skill.skillType != SkillType.Active) continue;

//        // if skill is equipped skip
//        if (playerSkillAttachment.IsSkillEquipped(skill)) continue;

//        bool equippedSkill = playerSkillAttachment.EquipActiveSkillToEmptySlot(skill); // check to see if skill is equipped

//        // if the skill is equipped display a msg
//        if (equippedSkill)
//            Debug.Log($"Equipped{skill.skillDisplayName} to empty slot"); // debug msg      
//    }
//}

// not needed Old version
// Generate 3 random choices (skills or stat increases)
//private void GenerateLevelUpChoices()
//{
//    List<LevelUpChoice> allPossibleChoices = new List<LevelUpChoice>();

//    Add available skills not needed anymroe
//        foreach (SkillData skill in allSkills)
//    {
//        if (!unlockedSkills.Contains(skill) && AreSkillRequirementsMet(skill))
//        {
//            LevelUpChoice skillChoice = new LevelUpChoice
//            {
//                //type = ChoiceType.Skill,
//                //skill = skill,
//                title = skill.skillDisplayName,
//                description = $"{skill.skillDescription}\nCost: {skill.skillAPCost} AP, {skill.skillENCost} EN"
//            };
//            allPossibleChoices.Add(skillChoice);
//        }
//    }
//

//    // Add stat increases (always available)
//    LevelUpChoice healthChoice = new LevelUpChoice
//    {
//        type = ChoiceType.HealthIncrease,
//        title = "Increase Max HP",
//        description = $"by {healthIncreaseAmount}"
//    };
//    allPossibleChoices.Add(healthChoice);

//    LevelUpChoice energyChoice = new LevelUpChoice
//    {
//        type = ChoiceType.EnergyIncrease,
//        title = "Increase Max EN",
//        description = $"by {energyIncreaseAmount}"
//    };
//    allPossibleChoices.Add(energyChoice);

//    LevelUpChoice attackChoice = new LevelUpChoice
//    {
//        type = ChoiceType.AttackIncrease,
//        title = "Increase Attack",
//        description = $"by {attackIncreaseAmount}"
//    };
//    allPossibleChoices.Add(attackChoice);

//    // Shuffle and select 3 choices
//    System.Random rng = new System.Random();
//    allPossibleChoices = allPossibleChoices.OrderBy(x => rng.Next()).ToList();

//    int choicesToOffer = Mathf.Min(3, allPossibleChoices.Count);
//    for (int i = 0; i < choicesToOffer; i++)
//    {
//        currentChoices[i] = allPossibleChoices[i];
//    }

//    // Clear any unused slots
//    for (int i = choicesToOffer; i < 3; i++)
//    {
//        currentChoices[i] = null;
//    }
//}