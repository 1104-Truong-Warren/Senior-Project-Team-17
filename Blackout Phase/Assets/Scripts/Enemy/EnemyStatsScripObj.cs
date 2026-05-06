// Used this scriptitable enemy as reference URL: https://www.youtube.com/watch?v=PoglGJoDcZg
// Used this for Inheritance reference URL: https://www.youtube.com/watch?v=F7Wu6_uzD1I
// Enemy stats for scriptable object, the EnemyInfo script has data for these variables
// Weijun

using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemies/Enemy Stats")]
public class EnemyStatsScripObj : ScriptableObject
{
    [Header("Enemy Base Stats Settings")]
    public int maxHP; // health
    public int attackRange; // enemy attk range
    public int baseAttack; // base attack
    public int detectionRange; // how far it can detect player
    public int movementRange; // how far it can move

    [Header("Enemy Combat Settings")]
    public int evasionRate; // dodge rate
    public int hitRate; // base hit chance
    public int critRate; // base crit rate

    [Header("Enemy Type")]
    public string enemyType; // describ what type of nemey it is
    public EnemyRank enemyRank; // what rank of enemy, mob/elite/boss
}
