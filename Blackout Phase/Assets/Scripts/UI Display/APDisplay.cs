// Warren

// Script no longer in use, but will keep incase.
// The main purpose of this script is to update and re-display the AP count on the top right corner.
// No longer in use because this was an attempt to NOT use TextMeshPro and instead work with Unity's default UI.

using UnityEngine;
using UnityEngine.UI;

public class APDisplay : MonoBehaviour
{
    private Text apText;
    
    void Start()
    {
        // Immediately changes "AP: 2/2" to the real value
        apText = GetComponent<Text>();
    }
    
    void Update()
    {
        if (CharacterInfo1.Instance != null)
        {
            // This line overwrites the text box content every frame
            apText.text = "AP: " + CharacterInfo1.Instance.currentAP + "/2";
        }
    }
}