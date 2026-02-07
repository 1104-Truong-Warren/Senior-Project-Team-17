// Warren

// The purpose of this script is that it manages the character information that is displayed on the top-left corner of the screen.
// It continuously updates the player's Health Points (HP) and the movement range (Move).
// The script will automatiicaly find and update the TextMeshPro UI elements that are the children of the GameObject it is attached to.

// Source: https://docs.unity3d.com/ScriptReference/GameObject.GetComponentsInChildren.html - Used to find all TextMeshPro children
// Source: https://docs.unity3d.com/ScriptReference/MonoBehaviour.StartCoroutine.html - Coroutine for waiting for player spawn
// Source: https://docs.unity3d.com/ScriptReference/WaitForSeconds.html - Used in coroutine for delayed checks
// Source: https://docs.unity3d.com/ScriptReference/Time-frameCount.html - Frame-based conditional logging
// Source: https://docs.unity3d.com/ScriptReference/UI.Image-fillAmount.html - HP bar fill control
// Source: https://docs.unity3d.com/ScriptReference/UI.Image.html - UI Image component for bar visuals

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CharacterInfoDisplay : MonoBehaviour
{
    [Header("Top-Left Display")]
    [SerializeField] private string hpPrefix = "HP: ";
    [SerializeField] private string movePrefix = "Move: ";
    [SerializeField] private string attackPrefix = "ATK: ";
    [SerializeField] private string enPrefix = "EN: ";

    [Header("HP Bar Visual")]
    [SerializeField] private Image hpBar; // UI Image for HP bar, set to Filled type in Inspector
    [SerializeField] private Image hpBarBackground; // Background image
    
    private CharacterInfo1 playerInfo;
    private TextMeshProUGUI[] textComponents;
    private bool playerFound = false;
    
    void Start()
    {
        textComponents = GetComponentsInChildren<TextMeshProUGUI>();
        
        Debug.Log("CharacterInfoDisplay Start");
        
        StartCoroutine(FindPlayerRoutine());
    }
    
    IEnumerator FindPlayerRoutine()
    {
        Debug.Log("CharacterInfoDisplay: Looking for player...");
        
        // Wait until player spawns
        while (playerInfo == null)
        {
            playerInfo = CharacterInfo1.Instance;
            
            if (playerInfo == null)
            {
                Debug.Log("CharacterInfoDisplay: Player not found yet, waiting...");
                yield return new WaitForSeconds(0.5f); // Check every 0.5 seconds
            }
        }
        
        Debug.Log($"CharacterInfoDisplay: Found player! {playerInfo.gameObject.name}");
        playerFound = true;
        
        // Initial UI update
        UpdateUI();
    }
    
    void Update()
    {
        if (playerFound)
        {
            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        if (playerInfo == null) return;
        
        // Debug log to see values
        if (Time.frameCount % 60 == 0) // Log every second
        {
            Debug.Log($"UI Update - HP: {playerInfo.CurrentHP}/{playerInfo.maxHP}, ATK: {playerInfo.BaseAttk}");
        }
        
        // Update HP text display
        if (textComponents.Length >= 1)
        {
            textComponents[0].text = hpPrefix + playerInfo.CurrentHP + "/" + playerInfo.maxHP;
        }
        
        // Update HP bar visual (if hpBar is assigned)
        if (hpBar != null && playerInfo.maxHP > 0) // Check to avoid division by zero
        {
            // Calculate HP percentage (0.0 to 1.0)
            float hpPercentage = (float)playerInfo.CurrentHP / playerInfo.maxHP;
            
            // Update bar fill amount
            hpBar.fillAmount = hpPercentage;
            
            // Colors, when the health is greater than 60%, it will stay green
            if (hpPercentage > 0.6f)
            {
                hpBar.color = Color.green;
                Debug.Log($"HP {hpPercentage*100}% = GREEN");
            }
            else if (hpPercentage > 0.3f) // If health is greater than 30%, it will turn yellow
            {
                hpBar.color = Color.yellow;
                Debug.Log($"HP {hpPercentage*100}% = YELLOW");
            }
            else
            {
                hpBar.color = Color.red; // If health is less than 30%, it will turn red
                Debug.Log($"HP {hpPercentage*100}% = RED");
            }
        }
        else if (hpBar != null && playerInfo.maxHP <= 0)
        {
            Debug.LogWarning("CharacterInfoDisplay: Player maxHP is 0 or negative, cannot update HP bar.");
        }
        
        // Update Move display
        if (textComponents.Length >= 2)
        {
            int moveRange = playerInfo.GetMoveRange();
            textComponents[1].text = movePrefix + moveRange;
        }
        
        // Update Attack display
        if (textComponents.Length >= 3)
        {
            textComponents[2].text = attackPrefix + playerInfo.BaseAttk;
        }
        
        // Update EN display
        if (textComponents.Length >= 4)
        {
            textComponents[3].text = enPrefix + playerInfo.CurrentEN + "/" + playerInfo.maxEN;
        }
    }
}