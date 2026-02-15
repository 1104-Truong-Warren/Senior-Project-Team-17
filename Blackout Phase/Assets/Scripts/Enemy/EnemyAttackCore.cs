using UnityEngine;

public abstract class EnemyAttackCore : MonoBehaviour
{
    protected EnemyInfo enemyInfo; // accessor

    protected virtual void Awake()
    {
        enemyInfo = GetComponent<EnemyInfo>(); // set up the enemyInfo
    }

    public abstract bool CanAttackPlayer(CharacterInfo1 player); // just a inheritance, for distance check0

    public abstract void AttackPlayer(CharacterInfo1 player); // the actual attk function check

    protected int Manhattan(Vector3Int a, Vector3Int b) // returns the correct distance bewteen player/enemy
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

}
