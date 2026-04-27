// From Warren's AP display as reference
// Displays the score 
// Weijun

using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    private TextMeshProUGUI scoreText; // text for the score
    private int lastScore = -1; // int holder for the score

    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>(); // setup the text mesh
    }

    private void Update()
    {
        // if the score manager is found test display score
        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("[ScoreUI] ScoreManager.Instance is null!"); // debug msg
            return;
        }

        Debug.LogWarning("[ScoreUI] Current Score: " + ScoreManager.Instance.CurrentScore); // debug msg

        scoreText.text = "Score: " + ScoreManager.Instance.CurrentScore;

        // make sure sore manager and score text is vaild
        if (ScoreManager.Instance == null || scoreText == null) return;

        int currentSore = ScoreManager.Instance.CurrentScore; // copy the current score from score manager

        // the current score is different than last score
        if (currentSore != lastScore)
        {
            lastScore = currentSore; // save it as last score

            scoreText.text = $"Score: {currentSore}"; // display current score
        }


    }
}
