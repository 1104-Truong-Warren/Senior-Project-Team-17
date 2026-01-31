using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Source: https://www.youtube.com/watch?v=Hd1xWdt3cP8 - Replicated from the YouTuber "Can With Code".
public class LevelsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI currentLevelText;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] Image xpBar;

    [Space(10)]
    [Header("Settings")]
    [SerializeField] int targetXP = 100;
    [SerializeField] int targetXPIncrease = 50;

    int currentLevel;
    int currentXP;

    private void Start()
    {
        currentLevel = 1;
        UpdateHUD();
    }

    public void IncreaseXP(int amount)
    {
        currentXP += amount;

        CheckForLevelUp();
        UpdateHUD();
    }

    private void CheckForLevelUp()
    {
        while(currentXP >= targetXP)
        {
            currentLevel++;
            currentXP -= targetXP;
            targetXP += targetXPIncrease;
        }
    }

    private void UpdateHUD()
    {
        currentLevelText.text = "Level " + currentLevel;
        xpText.text = currentXP + "/" + targetXP;
        xpBar.fillAmount = (float)currentXP / (float)targetXP;
    }
}
