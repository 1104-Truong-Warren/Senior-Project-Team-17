using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EnemySkillLogic : MonoBehaviour
{
    // accesor for other scripts
    [Header("Attachments")]
    [SerializeField] private EnemyInfo enemy;
    [SerializeField] private SkillAttachment skillAttachment;
    //[SerializeField] private SkillExecutor skillExecutor;

    private void Awake()
    {
        // setup the accessors
        enemy = GetComponent<EnemyInfo>();

        skillAttachment = GetComponent<SkillAttachment>();

        //skillExecutor = GetComponent<SkillExecutor>();
    }

    public bool TryQueueBestSkill(UnitCore target)
    {
        // check if the setup worked
        if (enemy == null || skillAttachment == null || target == null)
        {
            Debug.Log("[ESL] skillAttachment/enemy/skillExecutor not found!"); // debug msg
            return false;
        }

        SkillData bestSkillToUse = null; // define a skill holder

        int bestScore = int.MinValue; // place hoder for the best score calculation

        // loop through the active skills
        foreach (SkillData skil in skillAttachment.EquippedActiveSkills)
        {
            // check for null slots, keep on going
            if (skil == null) continue;

            // skip skills that are on cooldown
            if (skillAttachment.IsSkillOnCooldown(skil)) continue;

            // skip if it's not an attack skill
            if (skil.skillEffectType != SkillEffectType.Damage) continue;

            int distance = Manhattan(enemy.CurrentTile.gridLocation, target.CurrentTile.gridLocation); // find the distance if attack is in range

            int score = skil.AttackDamage; // set the score to the skill damage

            // if the distance is greater than the skill skip it
            if (distance > skil.AttackRange) continue;

            // check the attack damage is greater than target's HP + 100
            if (skil.AttackDamage >= target.CurrentHP)
                score += 100;

            // if the score is higher best score save it and use that skill
            if (score > bestScore)
            {
                bestScore = score;

                bestSkillToUse = skil;
            }
        }

        // if the best skill not found display a msg
        if (bestSkillToUse == null)
        {
            Debug.Log($"[ESL] no susable skill for {enemy.name}"); // debug msg
            return false;
        }

        int hitChance = HitRollCheck.FinalHitChanceCal(enemy.HitRate, bestSkillToUse.HitRate, target.EvasionRate); // find the hitChance

        Debug.Log($"[ESL] {enemy.name} used {bestSkillToUse.skillDisplayName}"); // debug msg

        TurnManager.Instance.StartPlayerReaction(enemy, target, bestSkillToUse.AttackDamage, hitChance, bestSkillToUse); // pass it to playerReaction

        return true; //skillExecutor.ExecuteSkill(enemy, bestSkillToUse, target); // run the SkilExecutor if true it worked!
    }

    private int Manhattan(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // returns the player/enemy distance
    }
}
