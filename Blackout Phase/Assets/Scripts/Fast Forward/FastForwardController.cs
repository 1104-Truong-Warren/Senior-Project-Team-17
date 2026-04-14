// Warren

// The purpose of this script is it creates a toggle button that speeds up time in the level.
// When pressed, it multiples the game speed by 2, and when pressed again, it will return
// to normal speed.

// Resource: https://www.youtube.com/watch?v=I1f-Urt9uo8&t=6s - for setting up the button and for time scale multiplication.

using UnityEngine;
using UnityEngine.UI;  
using TMPro;           

public class FastForwardToggle : MonoBehaviour
{
    private bool isFastForwarding = false;
    
    public TextMeshProUGUI buttonText;
    
    public void ToggleFastForward()
    {
        isFastForwarding = !isFastForwarding;
        
        if (isFastForwarding)
        {
            Time.timeScale = 2f;
            buttonText.text = "Stop Fast Forward";
        }
        else
        {
            Time.timeScale = 1f;
            buttonText.text = "Fast Forward";
        }
    }
}