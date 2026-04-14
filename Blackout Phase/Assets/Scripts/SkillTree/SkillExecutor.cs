//
// Weijun

using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SkillExecutor : MonoBehaviour
{
    //public static SkillExecutor Instance {get; private set;} // accessor for other scripts

    private CharacterInfo1 player; // player accessor

    private SkillAttachment skillAttachment; // accessor for player active skills

    //private Dictionary<SkillData, int> cooldowns = new Dictionary<SkillData, int>(); // dictionary for cool down

    private void Awake()
    {
        //if (Instance != null && Instance != this)  // if gameobject not found destory it, else set it to this
        //{
        //    Destroy(gameObject);

        //    return;
        //}

        //Instance = this; // found set it up

        // copy over the player's info
        if (player == null)
            player = GetComponent<CharacterInfo1>();

        // copy over the skill attachment 
        if (skillAttachment == null)
            skillAttachment = GetComponent<SkillAttachment>();
    }

    // for enemy
    public bool ExecuteSkill(UnitCore caster,SkillData skill, UnitCore target)
    {
        Debug.Log($"[SkillExecutor] Called useSkill: {skill?.skillDisplayName}"); // debug msg

        // if skill and player are not found return
        if (caster == null || skill == null)
        {
            Debug.Log($"[SkillExecutor] caster/skill is null | Failed!"); // debug msg
            return false;
        }

        SkillAttachment currentSA = caster.GetComponent<SkillAttachment>(); // get the current caster's skill attachment

        // check to see if the current SA is found
        if (currentSA == null)
        {
            Debug.Log($"[SE] No SkillAttachment found:{caster.name}"); // debug msg
            return false;
        }

        // check if skill can be used
        //if (!CanUseSkill(skill)) return false;

        bool skillUsed = false; // flag to check if skill is used

        // switch statements to control the type of skills attack, heal, buff
        switch (skill.skillEffectType)
        {
            case SkillEffectType.Damage:

                // if target is not found
                if (target == null)
                {
                    Debug.Log("[SE] Select a target to deal damage"); // debug msg
                    return false;
                }

                skillUsed = ExecuteDamage(caster,skill, target); // else damge the enemy
                break;

            //case SkillEffectType.Heal:
            //    Debug.Log($"[SkillExecutor] Heal case reached."); // debug msg
            //    skillUsed = ExecuteRecoverySkill(caster,skill);
            //    break;

            default:
                Debug.Log($"[SE] Skilltype doesn't exist:{skill.skillEffectType}"); // debug msg
                break;
        }

        // skill is not used return
        if (!skillUsed) return false;

        Debug.Log($"[SkillExecutor] [usedSkill]: AP:{skill.skillAPCost} | EN:{skill.skillENCost}"); // debug msg

        // is the caster a player? if so uses AP/EN
        if (caster is CharacterInfo1 player)
        {
            player.PlayerSpendEN(skill.skillENCost); // EN goes down

            TurnManager.Instance.PlayerSpendAP(skill.skillAPCost); // AP spent
        }

        //// if skill has cooldown
        //if (skill.skillCoolDown > 0)
        //{
        //    cooldowns[skill] = skill.skillCoolDown; // save it as the skill and cd time

        //    Debug.Log($"[SE] Set Skill cooldown for {skill.skillDisplayName} to {skill.skillCoolDown}"); // debug msg
        //}

        //// if skill has cooldown
        if (currentSA != null)
        {
            //cooldowns[skill] = skill.skillCoolDown; // save it as the skill and cd time

            currentSA.SetSkillCooldown(skill);

            Debug.Log($"[SE] Set Skill cooldown for {skill.skillDisplayName} to {skill.skillCoolDown}"); // debug msg
        }

        Debug.Log($"[SE] SKill uused:{skill.skillDisplayName}"); // debug msg
        return true; // skill used
    }

    // for player
    public bool UseSkill(SkillData skill, EnemyInfo enemy = null)
    {
        Debug.Log($"[SkillExecutor] Called useSkill: {skill?.skillDisplayName}"); // debug msg

        // if skill and player are not found return
        if (skill == null || player == null)
        {
            Debug.Log($"[SkillExecutor] player is null"); // debug msg
            return false;
        }

        // check if skill can be used
        if (!CanUseSkill(skill)) return false;

        bool skillUsed = false; // flag to check if skill is used

        // switch statements to control the type of skills attack, heal, buff
        switch (skill.skillEffectType)
        {
            case SkillEffectType.Damage:
                
                // if enemy target is not found
                if (enemy == null)
                {
                    Debug.Log("[SE] Select enemy target"); // debug msg
                    return false;
                }

                skillUsed = UseDamageSkill(skill, enemy); // else damge the enemy
                break;

            case SkillEffectType.Heal:
                Debug.Log($"[SkillExecutor] Heal case reached."); // debug msg
                skillUsed = UseRecoverySkill(skill);
                break;

            default:
                Debug.Log($"[SE] Skilltype doesn't exist:{skill.skillEffectType}"); // debug msg
                break;
        }

        // skill is not used return
        if (!skillUsed) return false;

        Debug.Log($"[SkillExecutor] [usedSkill]: AP:{skill.skillAPCost} | EN:{skill.skillENCost}"); // debug msg

        // player exist?
        if (player != null)
            player.PlayerSpendEN(skill.skillENCost); // EN goes down

        TurnManager.Instance.PlayerSpendAP(skill.skillAPCost); // AP spent

        //// if skill has cooldown
        //if (skill.skillCoolDown > 0)
        //{
        //    cooldowns[skill] = skill.skillCoolDown; // save it as the skill and cd time

        //    Debug.Log($"[SE] Set Skill cooldown for {skill.skillDisplayName} to {skill.skillCoolDown}"); // debug msg
        //}

        //// if skill has cooldown
        if (skillAttachment != null)
        {
            //cooldowns[skill] = skill.skillCoolDown; // save it as the skill and cd time

            skillAttachment.SetSkillCooldown(skill); 

            Debug.Log($"[SE] Set Skill cooldown for {skill.skillDisplayName} to {skill.skillCoolDown}"); // debug msg
        }

        Debug.Log($"[SE] SKill uused:{skill.skillDisplayName}"); // debug msg
        return true; // skill used
    }

    // for enemy
    private bool ExecuteDamage(UnitCore caster, SkillData skill, UnitCore target)
    {
        int hitChance = HitRollCheck.FinalHitChanceCal(caster.HitRate, skill.HitRate, target.EvasionRate); // calculate the hitchance of the caster

        // check to see if the hit landed
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log($"[SE] {caster.name} missed!"); // debug msg
            return false;
        }

        int dmg = skill.AttackDamage + caster.BaseAttack; // enemy's dmg = baseAttk + skillDmg

        // check to see if damage is negative
        if (dmg <= 0) return false;

        target.TakeDamage(dmg); // target is taking dmg

        Debug.Log($"[SE] {caster.name} hit {target.name} for:{dmg}"); // debug msg
        return true; // return turn if passed all the checkes
    }

    private bool UseDamageSkill(SkillData skill, EnemyInfo enemy)
    {
        // check to see if playerCombat is ready to be used
        if (PlayerCombatCheck.Instance == null) return false;

        return PlayerCombatCheck.Instance.PlayerSkillDmgChecker(skill, enemy); // check to see if skill conditions, crit, misses
    }

    // not needed
    //private bool ExecuteRecoverySkill(UnitCore caster, SkillData skill)
    //{
    //    if (skill == null || caster == null) return false;

    //    //
    //    if (caster is CharacterInfo1 player)
    //    {
    //        // set up both hp/en recovery amount
    //        int hpRecovery = Mathf.RoundToInt(player.MaxHP * skill.hpRecoverP); // max hp * the recovery amount 

    //        int enRecovery = Mathf.RoundToInt(player.MaxEN * skill.enRecoverP); // max en * the recovery amount

    //        player.RestoreHP(hpRecovery); // recover by the amount HP

    //        player.RestoreEN(enRecovery); // recover by the amount EN

    //        Debug.Log($"{name}:Used {skill.skillDisplayName}: Healed+{hpRecovery} HP, En+{enRecovery} EN"); // debug msg
    //    }

    //    return true;
    //}

    private bool UseRecoverySkill(SkillData skill)
    {
        // if playe and skill are not found return false
        if (skill == null || player == null) return false;

        // set up both hp/en recovery amount
        int hpRecovery = Mathf.RoundToInt(player.MaxHP * skill.hpRecoverP); // max hp * the recovery amount 

        int enRecovery = Mathf.RoundToInt(player.MaxEN * skill.enRecoverP); // max en * the recovery amount

        player.RestoreHP(hpRecovery); // recover by the amount HP

        player.RestoreEN(enRecovery); // recover by the amount EN

        Debug.Log($"{name}:Used {skill.skillDisplayName}: Healed+{hpRecovery} HP, En+{enRecovery} EN"); // debug msg
        return true;
    }

    public bool CanUseSkill(SkillData skill)
    {
        // check if skill exist
        if (skill == null)
        {
            Debug.Log($"[SkillExecutor] [CanUseSkill] is null"); // debug msg
            return false;
        }

        // check to see if player is missing
        if (player == null)
        {
            Debug.Log($"[SkillExecutor] Unit is null"); // debug msg
            return false;
        }

        // check if the AP is enough
        if (player.currentAP < skill.skillAPCost)
        {
            Debug.Log($"[SkillExecutor] Not enough AP"); // debug msg
            return false;
        }

        // check if EN is enough for skill
        if (!player.PlayerEnCheck(skill.skillENCost))
        {
            Debug.Log($"[SkillExecutor] Not enough EN"); // debug msg
            return false;        
        }

        // check for cooldown
        if (skillAttachment != null && skillAttachment.IsSkillOnCooldown(skill))
        {
            Debug.Log($"[SkillExecutor] SKill on CD:{skillAttachment.GetCooldownRemaining(skill)}"); // debug msg
            return false;
        }

        Debug.Log($"[SkillExecutor] SKill can be used"); // debug msg
        return true; // if passed all the test true
    }
}

// not needed
//private bool ExecuteRecoverySkill(UnitCore caster, SkillData skill)
//{
//    if (skill == null || caster == null) return false;

//    //
//    if (caster is CharacterInfo1 player)
//    {
//        // set up both hp/en recovery amount
//        int hpRecovery = Mathf.RoundToInt(player.MaxHP * skill.hpRecoverP); // max hp * the recovery amount 

//        int enRecovery = Mathf.RoundToInt(player.MaxEN * skill.enRecoverP); // max en * the recovery amount

//        player.RestoreHP(hpRecovery); // recover by the amount HP

//        player.RestoreEN(enRecovery); // recover by the amount EN

//        Debug.Log($"{name}:Used {skill.skillDisplayName}: Healed+{hpRecovery} HP, En+{enRecovery} EN"); // debug msg
//    }

//    return true;
//}

// CanUseSkill()
//// check to see if the cool down has reached 0
//if (cooldowns.TryGetValue(skill, out int cd) && cd > 0)
//{
//    Debug.Log($"[PlayerSkillExecutor] skill on CD:{cd}"); // debug msg
//    return false;
//}

// moved to skillAttachment
//public void CountCoolDownAtStart()
//{
//    Debug.Log("[SE] CountCoolDownAtStart Called!"); // debug msg

//    List<SkillData> keys = new List<SkillData>(cooldowns.Keys); // set up the cd keys

//    // use a loop to go through all the skills that has coold down keys
//    foreach (SkillData skill in keys)
//    {
//        Debug.Log($"[SE] Before cooldown count tick:{skill.skillDisplayName} = {cooldowns[skill]}"); // debug msg

//        // if the skill has a cd goes down by 1
//        if (cooldowns[skill] > 0)
//            cooldowns[skill]--;

//        Debug.Log($"[SE] After cooldown count tick:{skill.skillDisplayName} = {cooldowns[skill]}"); // debug msg
//    }
//}

//public int GetCoolDownRemaining(SkillData skill)
//{
//    // skill not found return
//    if (skill == null) return 0;

//    // if they skill has a cd return the cd turns
//    if (cooldowns.TryGetValue(skill, out int cd) && cd > 0) return cd;

//    return 0; // if nothing found return nothing
//}
