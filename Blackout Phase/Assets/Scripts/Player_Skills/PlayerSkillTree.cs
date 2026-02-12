// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU
// Weijun

using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillTree : MonoBehaviour
{
    // using hashset for fast look up
    private HashSet<Skill_ID> unlockedSkills = new HashSet<Skill_ID>(); // what skills are unlocked

    // ================== What Skills ===================

    // does skill exist in the skill enum list 
    public bool HasSkill(Skill_ID skillID) 
    {
        return unlockedSkills.Contains(skillID); // returns the skill ID/ name
    }

    // ================== Skills to unlock =================

    // unlock the skill check first
    public bool UnlockSkill(Skill_ID skillID)
    {
        return unlockedSkills.Add(skillID); // check to see if it's already unlocked 
    }

    // if pass unlock
    public bool SkillUnlock(SkillData skillData)
    {
        // using a foreach to go through the requirements
        foreach (var req in skillData.requirements)
            if (!HasSkill(req)) return false; // if name is not found return false

        return true; // if passed test return true
    }
}
