//
// Weijun

using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EnemyReactionController : MonoBehaviour
{
    [Header("Enemy Reaction Settings")]
    [SerializeField] private int activeDodgeBounus; // the evasion rate when actively selecting dodge
    [SerializeField] private int counterAttackChance; // 100 = always counterAttack
    [SerializeField] private bool allowToCounter = true; // flag for triggering counterAttack

    //private SkillData playerSkillHitRate; // access the playerSkills

    private EnemyInfo enemyInfo; // access enemyInfo

    private EnemyAttackCore enemyAttackCore; // access the core attack for inherits (melee/attack)

    private void Awake()
    {
        enemyInfo = GetComponent<EnemyInfo>(); // set up the enemyInfo

        enemyAttackCore = GetComponent<EnemyAttackCore>() ?? GetComponentInChildren<EnemyAttackCore>() ?? GetComponentInParent<EnemyAttackCore>(); // get all of them, if attack is on chidren/parent/normal
    }

    public void ReactToPlayerAttack(CharacterInfo1 player, SkillData skill)
    {
        // the function return True if enemy dodged successfully, False when dodge/attack failed

        Debug.Log("[ERC] ReactToPlayerAttack starting..."); // debug msg

        // if enemy not found return false
        if (enemyInfo == null || enemyInfo.CurrentTile == null)
        {
            Debug.Log("[ERC] enemyInfo/enemy tile is null!"); // debug msg
            return;
        }

        // if player not found return false
        if (player == null || player.CurrentTile == null)
        {
            Debug.Log("[ERC] player/player tile is null!"); // debug msg
            return;
        }

        // check to see if enemy is dead
        if (enemyInfo.CurrentHP <= 0)
        {
            Debug.Log("[ERC] enemy is dead..."); // debug msg
            return;
        }

        UnitCore target = player; // player is target

        bool canCounter = allowToCounter && enemyAttackCore != null && enemyAttackCore.CanAttackTarget(target); // check to see if condition are met, enemy in attack range, attack exist

        Debug.Log($"[ERC] allowToCounter:{allowToCounter} | enemyAttackCore:{enemyAttackCore != null} | "); // debug msg

        // if the conditions are not met return
        if (!canCounter) return;

        Debug.Log("[ERC] Enemy attacking back..."); // debug msg

        enemyAttackCore.AttackTarget(target); // attacks the target(player) if conditon met
    }
}

//private bool TryDodgeIncomingAttack(int incomingSkillHitChance)
//{
//    return !HitRollCheck.HitRollPercent(incomingSkillHitChance);
//}

// old version , the new version split the dodge logic into the EnemySkillLogic
//public bool ReactToPlayerAttack(CharacterInfo1 player, int playerSkillHitBonus, SkillData skill)
//{
//    // the function return True if enemy dodged successfully, False when dodge/attack failed

//    // if enemy not found return false
//    if (enemyInfo == null || enemyInfo.CurrentTile == null) return;

//    // if player not found return false
//    if (player == null || player.CurrentTile == null) return;

//    UnitCore target = player; // player is target

//    bool canCounter = allowToCounter && enemyAttackCore != null && enemyAttackCore.CanAttackTarget(target); // check to see if condition are met, enemy in attack range, attack exist

//    //Debug.Log($"[Enemy React] allow:{allowToCounter} core:{(enemyAttackCore != null)}" +
//    $"enemyTile:{(enemyInfo?.CurrentTile != null)} playerTile:{(target?.CurrentTile != null)}" +
//    $"inRange:{(enemyAttackCore != null ? enemyAttackCore.CanAttackTarget(target) : false)}"); //debug msg

//bool playerHit = HitRollCheck.HitRollPercent(playerHitChance); // roll check

// if hit check miss display a msg
//if (canCounter)
//{
//    Debug.Log($"{name}: Enemy counterAttacks Player!"); // debug msg

//    enemyAttackCore.AttackTarget(target);  // attacks the player
//    return false; // counter didn't dodge
//}

// dodge will be moved into the EnemySkillLogic
//int playerHitChance = HitRollCheck.FinalHitChanceCal(player.HitRate, playerSkillHitBonus, enemyInfo.EvasionRate + activeDodgeBounus); // enemy dodge roll

//bool dodged = TryDodgeIncomingAttack(playerHitChance); // hit roll 

//if (dodged)
//{
//    Debug.Log($"{name}: Enemy dodged player attack!"); // debug msg

//    // Added by Warren, for player's damage UI on the enemy
//    if (DamageObserver.Instance != null)
//    {
//        DamageObserver.Instance.ShowDodgedText(enemyInfo.transform.position); // enemy dodge
//    }

//    player.PlayerSpendEN(skill.skillENCost); // EN goes down before return

//    player.ApUsed(skill.skillAPCost); // AP goes down before 

////    return true; // enemy dodged
////}

//return false; // didn't dodge
//}

//// if enemyAttack is not null and player is in range attack
//if (allowToCounter && enemyAttackCore != null && enemyAttackCore.CanAttackPlayer(player))
//{
//   // // flag check to see if player pressed dodge

//   // counterAttackChance = HitRollCheck.FinalHitChanceCal(enemyInfo.EnemyHitRate, 0, player.BaseEvasion); // check for the chance of attack hit rate

//   //// check too see if attack landed
//   //if (!HitRollCheck.HitRollPercent(counterAttackChance))
//   // {
//   //     Debug.Log($"{name}: CounterAttacked Missed!"); // debug msg

//   //     return false; 
//   // }

//    enemyAttackCore.AttackPlayer(player); // if it landed attack player
//    //return false;
//}

// old
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
