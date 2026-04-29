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
        foreach (SkillData skill in skillAttachment.EquippedActiveSkills)
        {
            // check for null slots, keep on going
            if (skill == null) continue;

            // skip skills that are on cooldown
            if (skillAttachment.IsSkillOnCooldown(skill)) continue;

            // skip if it's not an attack skill
            if (skill.skillEffectType != SkillEffectType.Damage) continue;

            // skip if the target is not player/both
            if (skill.targetType != TargetType.Player && skill.targetType != TargetType.both) continue;

            int distance = Manhattan(enemy.CurrentTile.gridLocation, target.CurrentTile.gridLocation); // find the distance if attack is in range

            int score = skill.AttackDamage; // set the score to the skill damage

            // check to see if skill has effects if so add it
            foreach (var effect in skill.skillEffects)
            {
                // skip empty effect
                if (effect == null) continue;

                // check if it's a debuff
                if (effect.targetType == EffectTargetType.Target)
                    score += 3; // debuff enemy gives more score

                // check if it's a buff
                else if (effect.targetType == EffectTargetType.Self)
                    score += 1; // self buff less points
            }

            // if the distance is greater than the skill skip it
            if (distance > skill.AttackRange) continue;

            // check the attack damage is greater than target's HP + 100
            if (skill.AttackDamage >= target.CurrentHP)
                score += 100;

            // if the score is higher best score save it and use that skill
            if (score > bestScore)
            {
                bestScore = score;

                bestSkillToUse = skill;
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

        TurnManager.Instance.StartPlayerReaction(enemy, target, bestSkillToUse.AttackDamage + enemy.BaseAttack, hitChance, bestSkillToUse); // pass it to playerReaction

        return true; //skillExecutor.ExecuteSkill(enemy, bestSkillToUse, target); // run the SkilExecutor if true it worked!
    }

    public SkillData GetBestSkillForAttackRange(UnitCore target)
    {
        SkillData bestSkill = null; // access the skill holder

        int bestScore = int.MinValue; // holds the best score

        //
        foreach(SkillData skill in skillAttachment.EquippedActiveSkills)
        {
            // skip empty skills
            if (skill == null) continue;

            // skill on cd skip
            if (skillAttachment.IsSkillOnCooldown(skill)) continue;

            // skip if the skill is not a damage skill
            if (skill.skillEffectType != SkillEffectType.Damage) continue;

            int score = skill.AttackDamage; // base on the highest dmg

            // does it have effects? buff/debuff
            if (skill.skillEffects != null)
                score += skill.skillEffects.Count * 2; // each skill count as 2 

            // check if the score higher best 
            if (score > bestScore)
            {
                bestScore = score; // swap them, score if it's higher

                bestSkill = skill; // swap the skill 
            }
        }

        return bestSkill; // return the skill for use
    }

    private int Manhattan(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // returns the player/enemy distance
    }
}
