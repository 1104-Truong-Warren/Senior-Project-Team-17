//
// Weijun

using UnityEngine;

public abstract class EnemyAttackCore : MonoBehaviour
{
    protected EnemyInfo enemyInfo; // accessor

    protected virtual void Awake()
    {
        enemyInfo = GetComponentInParent<EnemyInfo>() ?? GetComponentInParent<EnemyInfo>(); // set up the enemyInfo, garb it from parent the main not copies

        // check to see if enemy is setup correctly
        if (enemyInfo == null)
            Debug.LogError($"{name} is not find: EnemyInfo"); // debug msg
    }

    public abstract bool CanAttackTarget(UnitCore target); // just a inheritance, for distance check

    public abstract bool CanAttackTarget(UnitCore target, int attackRange); // just a inheritance, for skill distance check

    public abstract void AttackTarget(UnitCore target); // the actual attk function check, Normal Attack

    public abstract void AttackTarget(UnitCore target, SkillData skill); // the skill versioin

    public abstract void AttackTarget(UnitCore target, int finalDamage); // for modified dmg for tank damage

    public abstract void AttackTarget(UnitCore target, int finalDamage, SkillData skill); // for modified dmg for tank damage

    protected int Manhattan(Vector3Int a, Vector3Int b) // returns the correct distance bewteen player/enemy
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

}
