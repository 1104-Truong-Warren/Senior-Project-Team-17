//
// Weijun

using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerSkillExecutor : MonoBehaviour
{
    public static PlayerSkillExecutor Instance {get; private set;} // accessor for other scripts

    private CharacterInfo1 player; // player accessor

    private SkillAttachment playerSkillAttachment; // accessor for player active skills

    private Dictionary<SkillData, int> cooldowns = new Dictionary<SkillData, int>(); // dictionary for cool down

    private void Awake()
    {
        if (Instance != null && Instance != this)  // if gameobject not found destory it, else set it to this
        {
            Destroy(gameObject);

            return;
        }

        Instance = this; // found set it up

        // copy over the player's info
        if (player == null)
            player = GetComponent<CharacterInfo1>();

        // copy over the skill attachment 
        if (playerSkillAttachment == null)
            playerSkillAttachment = GetComponent<SkillAttachment>();
    }

    public bool UseSkill(SkillData skill)
    {
        Debug.Log($"[PlayerSkillExecutor] Called useSkill: {skill?.skillDisplayName}"); // debug msg

        // if skill and player are not found return
        if (skill == null || player == null)
        {
            Debug.Log($"[PlayerSkillExecutor] player is null"); // debug msg
            return false;
        }

        // check if skill can be used
        if (!CanUseSkill(skill)) return false;

        bool skillUsed = false; // flag to check if skill is used

        // switch statements to control the type of skills attack, heal, buff
        switch (skill.skillEffectType)
        {
            case SkillEffectType.Heal:
                Debug.Log($"[PlayerSkillExecutor] Heal case reached."); // debug msg
                skillUsed = UseRecoverySkill(skill);
                break;
        }

        // skill is used
        if (skillUsed)
        {
            Debug.Log($"[PlayerSkillExecutor] [usedSkill]: AP:{skill.skillAPCost} | EN:{skill.skillENCost}"); // debug msg

            // player exist?
            if (player != null)
                player.PlayerSpendEN(skill.skillENCost); // EN goes down

            TurnManager.Instance.PlayerSpendAP(skill.skillAPCost); // AP spent

            // if skill has cooldown
            if (skill.skillCoolDown > 0)
                cooldowns[skill] = skill.skillCoolDown; // save it as the skill and cd time
        }

        Debug.Log($"[PlayerSkillExecutor] SKill uused:{skillUsed}"); // debug msg
        return skillUsed; // skill didn't use
    }

    private bool UseRecoverySkill(SkillData skill)
    {
        // if playe and skill are not found return false
        if (skill == null || player == null) return false;

        // set up both hp/en recovery amount
        int hpRecovery = Mathf.RoundToInt(player.maxHP * skill.hpRecoverP); // max hp * the recovery amount 

        int enRecovery = Mathf.RoundToInt(player.maxEN * skill.enRecoverP); // max en * the recovery amount

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
            Debug.Log($"[PlayerSkillExecutor] [CanUseSkill] is null"); // debug msg
            return false;
        }

        // check to see if player is missing
        if (player == null)
        {
            Debug.Log($"[PlayerSkillExecutor] player is null"); // debug msg
            return false;
        }

        // check if the AP is enough
        if (player.currentAP < skill.skillAPCost)
        {
            Debug.Log($"[PlayerSkillExecutor] Not enough AP"); // debug msg
            return false;
        }

        // check if EN is enough for skill
        if (!player.PlayerEnCheck(skill.skillENCost))
        {
            Debug.Log($"[PlayerSkillExecutor] Not enough EN"); // debug msg
            return false;        
        }

        // check to see if the cool down has reached 0
        if (cooldowns.TryGetValue(skill, out int cd) && cd > 0)
        {
            Debug.Log($"[PlayerSkillExecutor] skill on CD:{cd}"); // debug msg
            return false;
        }

        Debug.Log($"[PlayerSkillExecutor] SKill can be used"); // debug msg
        return true; // if passed all the test true
    }

    public void CountCoolDownAtStart()
    {
        List<SkillData> keys = new List<SkillData>(cooldowns.Keys); // set up the cd keys

        // use a loop to go through all the skills that has coold down keys
        foreach (SkillData skill in keys)
        {
            // if the skill has a cd goes down by 1
            if (cooldowns[skill] > 0)
                cooldowns[skill]--;
        }
    }

    public int GetCoolDownRemaining(SkillData skill)
    {
        // skill not found return
        if (skill == null) return 0;

        // if they skill has a cd return the cd turns
        if (cooldowns.TryGetValue(skill, out int cd) && cd > 0) return cd;

        return 0; // if nothing found return nothing
    }
}
