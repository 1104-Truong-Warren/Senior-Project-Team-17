// Warren
// The purpose of this script is to manage the UI that appears during Player Reaction state.
// It displays options for the player to choose how to react to enemy attacks.
// Shows success rates for Dodge and Counter options based on enemy hit chance.
// Keyboard shortcuts: D = Dodge, T = Direct Hit/Tank, F = Counterattack

// Resource: https://docs.unity3d.com/ScriptReference/GameObject.SetActive.html - For showing/hiding UI
// Resource: https://docs.unity3d.com/Manual/script-Button.html - For button click events

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections; // For IEnumerator

public class ReactionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject reactionPanel; // The main panel that appears during enemy attack
    [SerializeField] private TextMeshProUGUI titleText; 
    [SerializeField] private TextMeshProUGUI infoText; // Shows damage and enemy hit chance
    [SerializeField] private TextMeshProUGUI successRateText; // Shows dodge and counter success rates
    
    [Header("Buttons")]
    [SerializeField] private Button dodgeButton; // Button for dodge option
    [SerializeField] private Button tankButton; // Button for direct hit/tank option
    [SerializeField] private Button counterButton; // Button for counterattack option
    
    [Header("Button Text")]
    [SerializeField] private TextMeshProUGUI dodgeButtonText; // Text component of dodge button
    [SerializeField] private TextMeshProUGUI tankButtonText; // Text component of tank button
    [SerializeField] private TextMeshProUGUI counterButtonText; // Text component of counter button
    
    [Header("Settings")]
    //[SerializeField] private int dodgeENCost = 10; // How much EN it costs to dodge (for display only) - No longer need, but keep in case
    [SerializeField] private int counterENCost; // How much EN it costs to counterattack
    
    [Header("Timing Settings")]
    [SerializeField] private float uiAppearDelay = 0.8f; // Delay before UI appears (adjust in Inspector)
    private CharacterInfo1 playerInfo; // Reference to player info for EN checks

    // Ellison - added for tutorial support
    public bool isTutorial = false;
    public TutorialManager tutorialManager;
    
    private CharacterInfo1 playerInfo; // Reference to player info for EN checks
    private bool isWaitingToShow = false; // Flag to prevent multiple coroutines

    void Start()
    {
        // Hide panel at start, GameObjects are inactive until enemy attacks
        if (reactionPanel != null)
            reactionPanel.SetActive(false);
        
        // Get player info for EN checks
        playerInfo = CharacterInfo1.Instance;
        
        // Clear existing listeners and add new ones
        if (dodgeButton != null)
        {
            dodgeButton.onClick.RemoveAllListeners();
            dodgeButton.onClick.AddListener(OnDodgeClicked);
            Debug.Log("ReactionUI: Dodge button listener added");
        }
            
        if (tankButton != null)
        {
            tankButton.onClick.RemoveAllListeners();
            tankButton.onClick.AddListener(OnTankClicked);
            Debug.Log("ReactionUI: Tank button listener added");
        }
            
        if (counterButton != null)
        {
            counterButton.onClick.RemoveAllListeners();
            counterButton.onClick.AddListener(OnCounterClicked);
            Debug.Log("ReactionUI: Counter button listener added");
        }
    }
    
    void Update()
    {
        if (isTutorial) return;

        // Check TurnManager state to see if we should show/hide the panel
        if (TurnManager.Instance != null)
        {
            // Weijun
            bool shouldShow = TurnManager.Instance.WaitForPlayerReact && TurnManager.Instance.inComingAttackEnemy != null &&
                TurnManager.Instance.inComingAttackEnemy.CurrentHP > 0;

            // CHANGED: Use coroutine for delayed show, but immediate hide
            if (shouldShow && !reactionPanel.activeSelf && !isWaitingToShow)
            {
                // Start coroutine to show UI after delay
                StartCoroutine(ShowReactionUIDelayed());
            }
            else if (!shouldShow && reactionPanel.activeSelf)
            {
                // IMMEDIATELY hide the UI when player reaction is done
                HideReactionUI();
            }
        }
        
        // Update button states (interactable, text, colors) every frame
        // Only update if panel is active to save performance
        if (reactionPanel.activeSelf)
        {
            UpdateButtonStates();
        }
        
        // Keyboard shortcuts when panel is active (optional backup)
        if (reactionPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.D) && dodgeButton != null && dodgeButton.interactable)
            {
                OnDodgeClicked();
            }
            else if (Input.GetKeyDown(KeyCode.T) && tankButton != null && tankButton.interactable)
            {
                OnTankClicked();
            }
            else if (Input.GetKeyDown(KeyCode.F) && counterButton != null && counterButton.interactable)
            {
                OnCounterClicked();
            }
        }
    }
    
    // Coroutine to show UI after delay
    IEnumerator ShowReactionUIDelayed()
    {
        isWaitingToShow = true;
        
        // Specified delay, so enemy attack text can display first
        yield return new WaitForSeconds(uiAppearDelay);
        
        // Double-checks that we're still in reaction state
        if (TurnManager.Instance != null && TurnManager.Instance.WaitForPlayerReact)
        {
            ShowReactionUI();
        }
        
        isWaitingToShow = false;
    }
    
    // Displays the reaction UI with current attack information
    public void ShowReactionUI()
    {
        if (reactionPanel == null) return;
        
        if (TurnManager.Instance != null)
        {
            int damage = TurnManager.Instance.inComingDamage; // Get incoming damage from TurnManager
            int enemyHitChance = TurnManager.Instance.inComingHitChance; // Get enemy hit chance
            
            // Calculate success rates based on enemy hit chance
            // Dodge success = 100 - enemy hit chance (if enemy misses, you dodge)
            int dodgeSuccessRate = 100 - enemyHitChance;
            // Counter success = half of dodge rate (counter is harder to pull off)
            int counterSuccessRate = 100; //(100 - enemyHitChance) / 2;
            
            // Update info text with damage and enemy hit chance
            if (infoText != null)
                infoText.text = $"Incoming Damage: {damage} | Enemy Hit: {enemyHitChance}%";
            
            // Update success rate text with calculated percentages
            if (successRateText != null)
                successRateText.text = $"Dodge: {dodgeSuccessRate}% | Counter: {counterSuccessRate}%";
        }
        
        // Show the panel
        reactionPanel.SetActive(true);
        Debug.Log("ReactionUI: Showing reaction options");
    }
    
    // Hides the reaction UI immediately
    void HideReactionUI()
    {
        if (reactionPanel == null) return;
        reactionPanel.SetActive(false);
        
        // Cancel any pending show coroutine
        if (isWaitingToShow)
        {
            StopAllCoroutines();
            isWaitingToShow = false;
        }
        
        Debug.Log("ReactionUI: Hidden immediately");
    }
    
    // Updates button appearance based on EN availability and success rates
    void UpdateButtonStates()
    {
        if (playerInfo == null || TurnManager.Instance == null) return;

        RefreshCounterSkillCost(); // update in real time

        int enemyHitChance = TurnManager.Instance.inComingHitChance;
        int dodgeSuccessRate = 100 - enemyHitChance;
        int counterSuccessRate = (100 - enemyHitChance) / 2;
        
        if (dodgeButton != null)
        {
            dodgeButton.interactable = true;
            
            if (dodgeButtonText != null)
            {
                dodgeButtonText.text = $"DODGE (D)\n{dodgeSuccessRate}%";
                dodgeButtonText.color = Color.white;
            }
        }
        
        // Update Direct Hit/Tank button
        if (tankButton != null)
        {
            tankButton.interactable = true;
            if (tankButtonText != null)
            {
                tankButtonText.text = $"DIRECT HIT (T)\n100% DMG";
            }
        }
        
        // Update Counter button 
        if (counterButton != null)
        {
            // Check if player has enough EN using HasEN method
            bool canCounter = playerInfo.HasEN(counterENCost);
            counterButton.interactable = canCounter;
            
            if (counterButtonText != null)
            {
                counterButtonText.text = $"COUNTER (F) ({counterENCost} EN)\n{counterSuccessRate}%";
                counterButtonText.color = canCounter ? Color.white : Color.gray;
            }
        }
    }
    
    // Dodge button function that handles user's input, and calls TurnManager's corresponding method for player combat.
    public void OnDodgeClicked()
    {
        Debug.Log("ReactionUI: Dodge button clicked");
        
        if (TurnManager.Instance == null)
        {
            Debug.LogError("ReactionUI: TurnManager.Instance is NULL!");
            return;
        }
        
        // Hide UI immediately when player makes a choice
        HideReactionUI();
        
        // Call TurnManager directly
        TurnManager.Instance.PlayerDodgeReaction();
    }
    
    // Tank button / Direct Hit button function where the player will get hit by the enemy. Originally calls TurnManager's PlayerTankDamageReaction method but there was a bug where it calls the method twice and player takes 6 HP damage.
    public void OnTankClicked()
    {
        Debug.Log("ReactionUI: Tank button clicked");
        
        if (TurnManager.Instance == null)
        {
            Debug.LogError("ReactionUI: TurnManager.Instance is NULL!");
            return;
        }

        // Hide UI immediately when player makes a choice
        HideReactionUI();

        // Call TurnManager directly 
        TurnManager.Instance.PlayerTankDamageReaction();
        
    }
    
    // Counter Attack button function where player will have a chance in counter attacking the enemy, costs 5 EN, and will be deducted in the game scene interface.
    public void OnCounterClicked()
    {
        if (isTutorial)
        {
            tutorialManager.reactionSelectDone = true;
            return;
        }

        Debug.Log("ReactionUI: Counter button clicked");
        
        if (TurnManager.Instance == null)
        {
            Debug.LogError("ReactionUI: TurnManager.Instance is NULL!");
            return;
        }
        
        // Hide UI immediately when player makes a choice
        HideReactionUI();
        
        // Call TurnManager directly 
        // Counter EN cost is handled inside PlayerCounterAttackReaction or PlayerCombatCheck
        TurnManager.Instance.PlayerCounterAttackReaction();
    }
    private void RefreshCounterSkillCost()
    {
        // Now it displays the correct EN cost
        // Weijun
        // the player attack is null setup 

        SkillData currentSkill = PlayerCombatCheck.Instance.GetCurrentSkill(); // find the current skill index

        // check to see if currentSkill is not null before using
        if (currentSkill == null) return;

        counterENCost = currentSkill.skillENCost; // setup how much the EN cost
    }
}