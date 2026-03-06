using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemies/Enemy Stats")]
public class EnemyStatsScripObj : ScriptableObject
{
    [Header("Enemy Base Stats Settings")]
    public int maxHP; // health
    public int attackRange; // enemy attk range
    public int damage; // base dmg
    public int detectionRange; // how far it can detect player
    public int movementRange; // how far it can move

    [Header("Enemy Combat Settings")]
    public int evasionRate; // dodge rate
    public int hitRate; // base hit chance

    [Header("Enemy Type")]
    public string enemyType; // describ what type of nemey it is
}
