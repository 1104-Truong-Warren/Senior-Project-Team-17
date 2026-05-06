// Used this scriptitable enemy as reference URL: https://www.youtube.com/watch?v=PoglGJoDcZg
// Used this for Inheritance reference URL: https://www.youtube.com/watch?v=F7Wu6_uzD1I
// Used this for enemy attack behavior reference URL: https://www.youtube.com/watch?v=aA2CSfCBf7w, URL: https://www.youtube.com/watch?v=iOYo7flBUW4
// EnemyMeleeBasic inherit on the EnemyAttackCore script, by overloading the attack skill functions
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

        return distance <= enemyInfo.AttackRange; // check if distance <= enemy attack range; use the passed attackRange
    }

    // over load using a skill range
    public override bool CanAttackTarget(UnitCore target, int attackRange)
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

        Debug.Log($"{name} distance:{distance} range:{attackRange}"); // debug msg

        return distance <= attackRange; //enemyInfo.AttackRange; // check if distance <= enemy attack range; use the passed attackRange
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

        int hitChance = HitRollCheck.FinalHitChanceCal(enemyInfo.HitRate, skillHitBonus, target.EvasionRate); // get the hit chance on target

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
        if (!CanAttackTarget(target, skill.AttackRange))
        {
            Debug.Log("[EMB] target out of attack range!"); // debug msg
            return;
        }

        Debug.Log($"[EMB] Enemy HitRate:{enemyInfo.HitRate}"); // debug msg

        Debug.Log($"[EMB] Enemy Skill HitRate:{skill.HitRate}"); // debug msg

        int hitChance = HitRollCheck.FinalHitChanceCal(enemyInfo.HitRate, skill.HitRate, target.EvasionRate); // get the hit chance on target

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

    // overloaded for the tanking the normal
    public override void AttackTarget(UnitCore target, int finalDamage)
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

        Debug.Log($"[EMB] Enemy HitRate:{enemyInfo.HitRate}"); // debug msg

        Debug.Log($"[EMB] Enemy Skill HitRate:{enemyInfo.HitRate}"); // debug msg

        int hitChance = HitRollCheck.FinalHitChanceCal(enemyInfo.HitRate, enemyInfo.HitRate, target.EvasionRate); // get the hit chance on target

        Debug.Log($"[EMB] hitChance:{hitChance}"); // debug msg

        // check to see if it passes the roll test
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log($"[EMB] {name} attack miss! Player Dodged!"); // debug msg
            return;
        }

        //int dmg = skill.AttackDamage + enemyInfo.BaseAttack; // set up enemy dmg

        target.TakeDamage(finalDamage);

        Debug.Log($"[EMB] {name} hit player for:{finalDamage}"); // debug msg
    }

    // overloaded for tanking skill dmg
    public override void AttackTarget(UnitCore target, int finalDamage, SkillData skill)
    {
        Debug.Log("[EMB] AttackTarget starting..."); // debug msg

        // player is not found return 
        if (target == null)
        {
            Debug.Log("[EMB] target is null!"); // debug msg
            return;
        }

        // if enemy range doesn't reach player get out
        if (!CanAttackTarget(target, skill.AttackRange))
        {
            Debug.Log("[EMB] target out of attack range!"); // debug msg
            return;
        }

        Debug.Log($"[EMB] Enemy HitRate:{enemyInfo.HitRate}"); // debug msg

        Debug.Log($"[EMB] Enemy Skill HitRate:{skill.HitRate}"); // debug msg

        int hitChance = HitRollCheck.FinalHitChanceCal(enemyInfo.HitRate, skill.HitRate, target.EvasionRate); // get the hit chance on target

        Debug.Log($"[EMB] hitChance:{hitChance}"); // debug msg

        // check to see if it passes the roll test
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log($"[EMB] {name} attack miss! Player Dodged!"); // debug msg
            return;
        }

        //int dmg = skill.AttackDamage + enemyInfo.BaseAttack; // set up enemy dmg

        target.TakeDamage(finalDamage);

        Debug.Log($"[EMB] {name} hit player for:{finalDamage}"); // debug msg
    }
}
