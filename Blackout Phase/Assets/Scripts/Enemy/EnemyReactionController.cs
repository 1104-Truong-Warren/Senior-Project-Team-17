using UnityEngine;

public class EnemyReactionController : MonoBehaviour
{
    [Header("Enemy Reaction Settings")]
    [SerializeField] private int activeDodgeBounus; // the evasion rate when actively selecting dodge
    [SerializeField] private int counterAttackChance; // 100 = always counterAttack
    [SerializeField] private bool allowToCounter = true; // flag for triggering counterAttack

    // Added by Warren, enemy reaction sound effects.
    [Header("Enemy Reaction Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip enemyCounterSound;    
    [SerializeField] private AudioClip enemyDodgeSound;     
    [SerializeField] private AudioClip enemyCounterMissSound;

    private SkillData playerSkillHitRate; // access the playerSkills

    private EnemyInfo enemyInfo; // access enemyInfo

    private EnemyAttackCore enemyAttackCore; // access the core attack for inherits (melee/attack)

    private void Awake()
    {
        enemyInfo = GetComponent<EnemyInfo>(); // set up the enemyInfo

        enemyAttackCore = GetComponentInChildren<EnemyAttackCore>(); // get all of them, if attack is on chidren

        // Added by Warren, sets up audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public bool ReactToPlayerAttack(CharacterInfo1 player, int playerSkillHitBonus)
    {
        // the function return True if enemy dodged successfully, False when dodge/attack failed

        // if enemy not found return false
        if (enemyInfo == null || enemyInfo.currentTile == null) return false;

        // if player not found return false
        if (player == null || player.CurrentTile == null) return false;

        bool canCounter = allowToCounter && enemyAttackCore != null && enemyAttackCore.CanAttackPlayer(player); // check to see if condition are met, enemy in attack range, attack exist

        Debug.Log($"[Enemy React] allow:{allowToCounter} core:{(enemyAttackCore != null)}" +
            $"enemyTile:{(enemyInfo?.currentTile != null)} playerTile:{(player?.CurrentTile != null)}" +
            $"inRange:{(enemyAttackCore != null ? enemyAttackCore.CanAttackPlayer(player) : false)}"); //debug msg

        //bool playerHit = HitRollCheck.HitRollPercent(playerHitChance); // roll check

        // if hit check miss display a msg
        if (canCounter)
        {
            Debug.Log($"{name}: Enemy counterAttacks Player!"); // debug msg

            PlaySound(enemyCounterSound); // Added by Warren

            enemyAttackCore.AttackPlayer(player);  // attacks the player
            return false; // counter didn't dodge
        }

        int playerHitChance = HitRollCheck.FinalHitChanceCal(player.BaseHitRate, playerSkillHitBonus, enemyInfo.EvasionRate + activeDodgeBounus); // enemy dodge roll

        bool playerHit = HitRollCheck.HitRollPercent(playerHitChance); // hit roll 

        if (!playerHit)
        {
            Debug.Log($"{name}: Enemy dodged player attack!"); // debug msg

            PlaySound(enemyDodgeSound);
            
            // Added by Warren, for player's damage UI on the enemy
            if (DamageObserver.Instance != null)
            {
                DamageObserver.Instance.ShowDodgedText(enemyInfo.transform.position); // enemy dodge
            }

            return true; // enemy dodged
        }

        return false; // didn't dodge
    }

    // Added by Warren, plays sound effects
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

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
