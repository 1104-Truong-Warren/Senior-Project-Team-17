//
//
// Weijun

using UnityEngine;

public enum EnemyRank
{
    Normal, // regular mob

    Elite, // Strong mob

    Boss // Bosses
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance {  get; private set; } // accesser or other scripts

    // defining all the enemy scores, mobs, elites, bosses
    [SerializeField] private int normalScore = 200;
    [SerializeField] private int eliteSocre = 400;
    [SerializeField] private int bossScore = 800;

    // current/max score
    private int currentScore;
    private int maxPossibleScore;

    // helper functions to access the data
    public int CurrentScore => currentScore;
    public int MaxPossibleScore => maxPossibleScore;

    private void Awake()
    {
        Instance = this; // set up the instance
    }

    public void RegisterEnemyScore(EnemyRank rank)
    {
        maxPossibleScore += GetEnemyBaseScore(rank); // find the max score by each enemy
    }

    public void AddEnemyKillScore(EnemyRank rank, bool killedInFuryMode)
    {
        int score = GetEnemyBaseScore(rank); // score based on enemy rank

        //  if plyaer is in Furymode when socre is double
        if (killedInFuryMode) //(PlayerFuryMode.Instance != null && PlayerFuryMode.Instance.inFuryMode)
            score *= 2;

        currentScore += score; // add to current score

        Debug.Log($"[SM] killed {rank} | + {score} | Total: {currentScore}"); // debug msg
    }

    private int GetEnemyBaseScore(EnemyRank rank)
    {
        // use switch statement for each enemy score
        switch (rank)
        {
            // for mobs
            case EnemyRank.Normal:
                return normalScore;

            // for elite
            case EnemyRank.Elite:
                return eliteSocre;

            // for boss
            case EnemyRank.Boss:
                return bossScore;
 
            // default
            default: 
                return 0;
        }
    }

    public string GetRank()
    {
        // if the score is 0 low rank
        if (maxPossibleScore <= 0) return "C";

        float scoreRatio = (float)currentScore / maxPossibleScore; // to find the score, current/max

        // score is higher than 120% base score
        if (scoreRatio >= 1.2f)
            return "S";

        // Score is higher than 100% base score
        else if (scoreRatio >= 1.0)
            return "A";

        // Score is higher than 80% base score
        else if(scoreRatio >= 0.8f)
            return "B";
        
        // anything below that
        else
            return "C";
    }

}
