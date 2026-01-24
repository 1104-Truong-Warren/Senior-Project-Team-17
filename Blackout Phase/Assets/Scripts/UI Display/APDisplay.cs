// Warren

using UnityEngine;
using UnityEngine.UI;


public class APDisplay : MonoBehaviour
{
    private Text apText;
    
    void Start()
    {
        apText = GetComponent<Text>();
        // At Start, it will immediately change "AP: 2/2" to the real value
    }
    
    void Update()
    {
        if (CharacterInfo1.Instance != null)
        {
            // This line OVERWRITES the Text box content every frame
            apText.text = "AP: " + CharacterInfo1.Instance.currentAP + "/2";
        }
    }
}