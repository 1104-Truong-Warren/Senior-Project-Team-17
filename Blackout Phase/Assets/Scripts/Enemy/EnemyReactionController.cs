using UnityEngine;

public class EnemyReactionController : MonoBehaviour
{
    [Header("Enemy Reaction Settings")]
    [SerializeField] private int activeDodgeBounus; // the evasion rate when actively selecting dodge
    [SerializeField] private int counterAttackChance; // 100 = always counterAttack
    [SerializeField] private bool allowToCounter = true; // flag for triggering counterAttack

    private SkillData playerSkillHitRate; // access the playerSkills

    private EnemyInfo enemyInfo; // access enemyInfo

    private EnemyAttackCore enemyAttackCore; // access the core attack for inherits (melee/attack)

    private void Awake()
    {
        enemyInfo = GetComponent<EnemyInfo>(); // set up the enemyInfo

        enemyAttackCore = GetComponentInChildren<EnemyAttackCore>(); // get all of them, if attack is on chidren
    }

    public bool ReactToPlayerAttack(CharacterInfo1 player, int playerSkillHitBonus)
    {
        // the function return True if enemy dodged successfully, False when dodge/attack failed

        // if enemy not found return false
        if (enemyInfo == null || enemyInfo.currentTile == null) return false;

        // if player not found return false
        if (player == null || player.CurrentTile == null) return false;

        Debug.Log($"[Enemy React] allow:{allowToCounter} core:{(enemyAttackCore != null)}" +
            $"enemyTile:{(enemyInfo?.currentTile != null)} playerTile:{(player?.CurrentTile != null)}" +
            $"inRange:{(enemyAttackCore != null ? enemyAttackCore.CanAttackPlayer(player) : false)}"); //debug msg

        int playerHitChance = HitRollCheck.FinalHitChanceCal(player.BaseHitRate, playerSkillHitBonus, enemyInfo.EvasionRate + 10); // enemy dodge roll

        bool playerHit = HitRollCheck.HitRollPercent(playerHitChance); // roll check

        // if hit check miss display a msg
        if (!playerHit)
        {
            Debug.Log($"{name}: Enemy dodged Player Attack!"); // debug msg
            return true; 
        }

        // if enemyAttack is not null and player is in range attack
        if (allowToCounter && enemyAttackCore != null && enemyAttackCore.CanAttackPlayer(player))
        {
           // // flag check to see if player pressed dodge

           // counterAttackChance = HitRollCheck.FinalHitChanceCal(enemyInfo.EnemyHitRate, 0, player.BaseEvasion); // check for the chance of attack hit rate

           //// check too see if attack landed
           //if (!HitRollCheck.HitRollPercent(counterAttackChance))
           // {
           //     Debug.Log($"{name}: CounterAttacked Missed!"); // debug msg

           //     return false; 
           // }
            
            enemyAttackCore.AttackPlayer(player); // if it landed attack player
            //return false;
        }

        return false;

        //int playerSkillHitChance = playerSkillHitRate.AttkHitRate; // find the attack/skill chance

        //int playerHitChance = HitRollCheck.FinalHitChanceCal(player.BaseHitRate, playerSkillHitChance, (enemyInfo.EvasionRate + activeDodgeBounus)); // calculate the player hit chance

        //bool playerHit = HitRollCheck.HitRollPercent(playerHitChance);

        //// if attck missed
        //if (!playerHit)
        //{
        //    Debug.Log($"{name}: Dodged the Attack! (Evasion successed)"); // debug msg

        //    return true;
        //}

        //Debug.Log($"{name}: Failled to Dodge the Attack!"); // debug msg

        //return false;
    }
}
