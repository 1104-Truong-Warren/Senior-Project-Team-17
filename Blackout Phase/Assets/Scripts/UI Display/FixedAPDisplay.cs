// Warren

using UnityEngine;
using TMPro;

public class FixedAPDisplay : MonoBehaviour
{
    private TextMeshProUGUI apText;
    private int lastAP = -1;
    
    void Start()
    {
        apText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(InitializeDisplay());
    }
    
    System.Collections.IEnumerator InitializeDisplay()
    {
        // Wait for CharacterInfo1 to be ready
        yield return new WaitUntil(() => CharacterInfo1.Instance != null);
        
        // Wait for TurnManager to be ready
        yield return new WaitUntil(() => TurnManager.Instance != null);
        
        Debug.Log("FixedAPDisplay: Both managers ready!");
        
        UpdateAPDisplay();
    }
    
    void Update()
    {
        UpdateAPDisplay();
    }
    
    void UpdateAPDisplay()
    {
        if (CharacterInfo1.Instance != null)
        {
            if (CharacterInfo1.Instance.currentAP != lastAP)
            {
                lastAP = CharacterInfo1.Instance.currentAP;
                apText.text = "AP: " + lastAP + "/2";
                Debug.Log("FixedAPDisplay: Updated to " + lastAP + " AP");
            }
            return;
        }
        
        if (lastAP == -1)
        {
            apText.text = "AP: 2/2";
        }
    }
}