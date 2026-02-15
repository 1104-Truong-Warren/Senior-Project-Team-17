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
    public override bool CanAttackPlayer(CharacterInfo1 player)
    {
        // player or player current tile is not found return false
        if (player == null || player.CurrentTile == null) return false;

        // enemy or enemy current tile not found return false
        if (enemyInfo == null || enemyInfo.currentTile == null) return false;

        int distance = Manhattan(enemyInfo.currentTile.gridLocation, player.CurrentTile.gridLocation); // calculate the player/enemy distance

        Debug.Log($"{name} distance:{distance} range:{enemyInfo.attackRange}"); // debug msg

        return distance <= enemyInfo.attackRange; // check if distance <= enemy attack range
    }

    public override void AttackPlayer(CharacterInfo1 player)
    {
        // if enemy range doesn't reach player get out
        if (!CanAttackPlayer(player)) return;

        int hitChance = HitRollCheck.FinalHitChanceCal(enemyInfo.EnemyHitRate, skillHitBonus, player.BaseEvasion); // get the hit chance on player

        // check to see if it passes the roll test
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log($"{name} attack miss! Player Dodged!"); // debug msg
            return;
        }

        int dmg = enemyInfo.EnemyDmg; // set up enemy dmg

        player.PlayerTakeDamage(dmg);

        Debug.Log($"{name} hit player for:{dmg}"); // debug msg
    }
}
