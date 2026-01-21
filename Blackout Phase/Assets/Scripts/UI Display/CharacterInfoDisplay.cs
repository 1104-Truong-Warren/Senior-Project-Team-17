// Warren

// The purpose of this script is that it manages the character information that is displayed on the top-left corner of the screen.
// It continuously updates the player's Health Points (HP) and the movement range (Move).
// The script will automatiicaly find and update the TextMeshPro UI elements that are the children of the GameObject it is attached to.

// Resources:

using UnityEngine;
using TMPro;

public class CharacterInfoDisplay : MonoBehaviour
{
    [Header("Top-Left Display")] // Header to created labled section in Unity inspector.
    [SerializeField] private string hpPrefix = "HP: "; // Customizable prefix for HP text.
    [SerializeField] private string movePrefix = "Move: "; // Customizable prefix for movement range.
    
    void Update()
    {

        // Checks if CharacterInfo1 script is available, on contains the HP and movement data.
        if (CharacterInfo1.Instance != null)
        {
            // Finds all TextMeshPro components that are the children of the GameObject.
            // Resource: https://docs.unity3d.com/ScriptReference/GameObject.GetComponentsInChildren.html
            TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>();
            
            // Updates HP display if at least one TextMeshPro child exists.
            if (textComponents.Length >= 1)
            {
                // HP display
                // Displays HP information, where the to the left of the "/" is the current HP, and the right of the "/" is the max HP.
                // Resource: https://docs.unity3d.com/ScriptReference/GameObject.GetComponentsInChildren.html
                textComponents[0].text = hpPrefix + CharacterInfo1.Instance.hp + "/" + CharacterInfo1.Instance.maxHP;
            }
            
            // Updates movement range display if at least 2 TextMeshPro children exist.
            if (textComponents.Length >= 2)
            {
                // Move range display
                int moveRange = CharacterInfo1.Instance.GetMoveRange();
                // Displays the calculated movement range.
                textComponents[1].text = movePrefix + moveRange;
            }
        }
    }
}