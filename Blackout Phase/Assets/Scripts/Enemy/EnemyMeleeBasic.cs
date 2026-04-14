//
// Weijun

using UnityEngine;

public class EnemyMeleeBasic : EnemyAttackCore // using the EnemyAttackCore to build on
{
    [Header("Enemy Skill Accuracy Settings")]
    [SerializeField] private int skillHitBonus; // does the skill give extra hit rate

    //[SerializeField] private EnemyInfo enemyInfo; // access enemy

    //private void Awake()
    //{
    //    if (enemyInfo == null)
    //        enemyInfo = GetComponent<EnemyInfo>();
    //}

    protected override void Awake()
    {
        base.Awake(); // awake so the base enemyInfo works
    }
    public override bool CanAttackTarget(UnitCore target)
    {
        // player or player current tile is not found return false
        if (target == null || target.CurrentTile == null)
        {
            Debug.Log("[EMB] target/target tile is null!"); // debug msg
            return false;
        }

        // enemy or enemy current tile not found return false
        if (enemyInfo == null || enemyInfo.CurrentTile == null)
        {
            Debug.Log("[EMB] enemy/enemy tile is null!"); // debug msg
            return false;
        }

        int distance = Manhattan(enemyInfo.CurrentTile.gridLocation, target.CurrentTile.gridLocation); // calculate the player/enemy distance

        Debug.Log($"{name} distance:{distance} range:{enemyInfo.AttackRange}"); // debug msg

        return distance <= enemyInfo.AttackRange; // check if distance <= enemy attack range
    }

    // for the normal attack
    public override void AttackTarget(UnitCore target)
    {
        Debug.Log("[EMB] AttackTarget starting..."); // debug msg

        // player is not found return 
        if (target == null)
        {
            Debug.Log("[EMB] target is null!"); // debug msg
            return;
        }

        // if enemy range doesn't reach player get out
        if (!CanAttackTarget(target))
        {
            Debug.Log("[EMB] target out of attack range!"); // debug msg
            return;
        }

        int hitChance = HitRollCheck.FinalHitChanceCal(enemyInfo.HitRate, 0, target.EvasionRate); // get the hit chance on target

        Debug.Log($"[EMB] hitChance:{hitChance}"); // debug msg

        // check to see if it passes the roll test
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log($"[EMB] {name} attack miss! Player Dodged!"); // debug msg
            return;
        }

        int dmg = enemyInfo.BaseAttack; // set up enemy dmg

        target.TakeDamage(dmg);

        Debug.Log($"[EMB] {name} hit player for:{dmg}"); // debug msg
    }

    // overloaded for the skill
    public override void AttackTarget(UnitCore target, SkillData skill)
    {
        Debug.Log("[EMB] AttackTarget starting..."); // debug msg

        // player is not found return 
        if (target == null)
        {
            Debug.Log("[EMB] target is null!"); // debug msg
            return;
        }

        // if enemy range doesn't reach player get out
        if (!CanAttackTarget(target))
        {
            Debug.Log("[EMB] target out of attack range!"); // debug msg
            return;
        }

        int hitChance = HitRollCheck.FinalHitChanceCal(enemyInfo.HitRate, skill.hitRateBonus, target.EvasionRate); // get the hit chance on target

        Debug.Log($"[EMB] hitChance:{hitChance}"); // debug msg

        // check to see if it passes the roll test
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log($"[EMB] {name} attack miss! Player Dodged!"); // debug msg
            return;
        }

        int dmg = skill.AttackDamage + enemyInfo.BaseAttack; // set up enemy dmg

        target.TakeDamage(dmg);

        Debug.Log($"[EMB] {name} hit player for:{dmg}"); // debug msg
    }
}
