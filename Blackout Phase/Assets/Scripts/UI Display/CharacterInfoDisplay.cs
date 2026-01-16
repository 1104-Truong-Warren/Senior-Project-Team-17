// Warren

using UnityEngine;
using TMPro;

public class CharacterInfoDisplay : MonoBehaviour
{
    [Header("Top-Left Display")]
    [SerializeField] private string hpPrefix = "HP: ";
    [SerializeField] private string movePrefix = "Move: ";
    
    void Update()
    {
        if (CharacterInfo1.Instance != null)
        {
            TextMeshProUGUI[] textComponents = GetComponentsInChildren<TextMeshProUGUI>(); // ← CHANGE THIS!
            
            if (textComponents.Length >= 1)
            {
                // HP display
                textComponents[0].text = hpPrefix + CharacterInfo1.Instance.hp + "/" + CharacterInfo1.Instance.maxHP;
            }
            
            if (textComponents.Length >= 2)
            {
                // Move range display
                int moveRange = CharacterInfo1.Instance.GetMoveRange();
                textComponents[1].text = movePrefix + moveRange;
            }
        }
    }
}