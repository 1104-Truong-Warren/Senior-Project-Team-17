// Warren

// The purpose of this script is it is used to monitor and display the player's current Action Points (AP) in the top right corner of the screen.
// It reads the AP value from the CharacterInfo1 script and updates the TextMeshPro text element.
// The UI element shows "AP: X/2" where the X is the current AP count.

using UnityEngine;
using TMPro; // Library that is required for TextMeshPro.

public class FixedAPDisplay : MonoBehaviour
{
    private TextMeshProUGUI apText; // Reference to TextMeshPro UI component that displays the AP.

    private int lastAP = -1; // Indicates that no value has been displayed yet.
    
    void Start()
    {
        apText = GetComponent<TextMeshProUGUI>(); // Gets the TextMeshPro component that is attached to the GameObject.
        StartCoroutine(InitializeDisplay()); // This function starts the initialization coroutine and waits for the required systems.
    }
    
    // Coroutine that makes sure that all required systems are ready before displaying the AP
    // Resource: https://docs.unity3d.com/Manual/Coroutines.html
    System.Collections.IEnumerator InitializeDisplay()
    {
        // Wait for CharacterInfo1 to be ready
        yield return new WaitUntil(() => CharacterInfo1.Instance != null);
        
        // Wait for TurnManager to be ready
        yield return new WaitUntil(() => TurnManager.Instance != null);
        
        Debug.Log("FixedAPDisplay: Both managers ready!");
        
        UpdateAPDisplay(); // Perform the initial display update
    }
    
    void Update()
    {
        UpdateAPDisplay(); // This function will be called every frame to see if the AP changes.
    }
    
    // This is a core method that checks for the AP changes and updates the display
    void UpdateAPDisplay()
    {
        // Checks if Character1 is available, which conists of the AP data.
        if (CharacterInfo1.Instance != null)
        {
            // Only updates if the AP value has changed, prevents unnecessary updates.
            if (CharacterInfo1.Instance.currentAP != lastAP)
            {
                lastAP = CharacterInfo1.Instance.currentAP; // Stores the new AP value.
                apText.text = "AP: " + lastAP + "/2"; // Updates the UI text to show current AP out of 2.
                Debug.Log("FixedAPDisplay: Updated to " + lastAP + " AP"); 
            }
            return;
        }
        
        // If CharacterInfo1 script is not available, then the display will not show.
        // It will only show the default value.
        if (lastAP == -1)
        {
            apText.text = "AP: 2/2";
        }
    }
}