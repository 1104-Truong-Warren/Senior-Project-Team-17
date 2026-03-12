//
// Weijun

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttachment : MonoBehaviour
{
    public static SkillAttachment Instance { get; private set; } // copies static data 

    [Header("Equipped Active Skills (maximum 4 active skills")] // choose 4 active skills
    [SerializeField] private List<SkillData> equippedActiveSkills = new List<SkillData>(4); // a list for the 4 active skills

    [Header("Unlocked Active Skills")] // passive skils to choice from
    [SerializeField] private List<SkillData> unlockedActiveSkills = new List<SkillData>(); // list of how many active skills player unlocked

    [Header("Unlocked Passive Skills")] // passive skils to choice from
    [SerializeField] private List<SkillData> unlockedPassiveSkills = new List<SkillData>(); // list of how many passives player unlocked

    public List<SkillData> EquippedActiveSkills => equippedActiveSkills; // accessor for the active skills

    public List<SkillData> UnlockedActiveSkills => unlockedActiveSkills; // accessor for the passive skills

    public List<SkillData> UnlockedPassiveSkills => unlockedPassiveSkills; // accessor for the passive skills

    public int MaxActiveSkillSlots => 4; // how many skills the player/enemy can equip as active skills


    public SkillData GetActiveSkill(int index)
    {
        // if the index is bigger than 4 or less than 0 get out
        if (index < 0 || index >= equippedActiveSkills.Count) return null;

        return equippedActiveSkills[index]; // else return skill by index
    }

    public bool EquipActiveSkillToSlot(SkillData skill, int index)
    {
        //  check to see if skill exist
        if (skill == null) return false;

        // if it's a passive skill get out
        if (skill.skillType != SkillType.Active) return false;

        // skill doesn't exist on slot get out
        if (!unlockedActiveSkills.Contains(skill)) return false;

        // skills is more than 4 or less than 1 get out
        if (index < 0 || index >= 4) return false;

        // skill is already equipped get out
        if (equippedActiveSkills.Contains(skill)) return false;

        // always fill the index slots
        while (equippedActiveSkills.Count < 4)
        {
            equippedActiveSkills.Add(null); // add a place holder
        }

        // don't overwrrite the exist skill
        if (equippedActiveSkills[index] != null) return false;

        equippedActiveSkills[index] = skill; // if condition met add the skill to active skill to the index
        return true;
    }

    public bool EquipActiveSkillToEmptySlot(SkillData skill)
    {
        //  check to see if skill exist
        if (skill == null) return false;

        // if it's a passive skill get out
        if (skill.skillType != SkillType.Active) return false;

        // skill doesn't exist on slot get out
        if (!unlockedActiveSkills.Contains(skill)) return false;

        // skill is already equipped get out
        if (equippedActiveSkills.Contains(skill)) return false;

        int emptySlot = GetEmptySkillSlot(); // find the first empty slot

        // check if it has empty spot 
        if (emptySlot == -1) return false;

        equippedActiveSkills[emptySlot] = skill; // if condition met add the skill to active skill to the index

        return true;
    }

    public int GetEmptySkillSlot()
    {
        // loop when the total skill is less than 4
        while (equippedActiveSkills.Count < 4)
        {
            equippedActiveSkills.Add(null); // add the placeholders
        }

        // from 0 to total skill, if null is found return it
        for (int i = 0; i < equippedActiveSkills.Count; i++)
        {
            // if found return the spot
            if (equippedActiveSkills[i] == null) return i;
        }

        return -1; // return -1 if nothing is found which doesn't exist in a list
    }

    public bool UnequipActiveSkillByIndex(int index)
    {
        // check if index is great than 1 and less than total skill count
        if (index < 0 || index >= equippedActiveSkills.Count) return false;

        equippedActiveSkills[index] = null; // assign the slot to null

        return true;
    }

    public bool IsSkillEquipped(SkillData skill)
    {
        // check to make sure skill reference is not empty
        if (skill == null) return false;

        return equippedActiveSkills.Contains(skill); // return the skill
    }

    public bool UnequipActiveSkill(SkillData skill)
    {
        // skill is not found return false
        if (skill == null) return false;

        return equippedActiveSkills.Remove(skill); // if found remove it from the list
    }

    public bool UnlockSkill(SkillData skill)
    {
        //  check to see if skill exist
        if (skill == null) return false;

        // check to see if skill exist in the list
        if (HasUnlockedSkill(skill)) return false;

        // check to see if the skill type matches
        if (skill.skillType == SkillType.Active)
        {
            unlockedActiveSkills.Add(skill); // if not exsit add it to the list
            return true;
        }

        // check to see if the skill type matches
        if (skill.skillType == SkillType.Passive)
        {
            unlockedPassiveSkills.Add(skill); // if not exsit add it to the list
            return true;
        }

        return false; // if nothing found return false
    }

    public bool HasUnlockedSkill(SkillData skill)
    {
        //  check to see if skill exist
        if (skill == null) return false;

        // check to see if skill is active
        if (skill.skillType == SkillType.Active)
            return unlockedActiveSkills.Contains(skill);

        // check to see if skill is passive
        if (skill.skillType == SkillType.Passive)
            return unlockedPassiveSkills.Contains(skill);

        return false; // nothing found return false
    }

    public bool HasUnlockedSkillID(Skill_ID id)
    {
        // using loop to go through all the active skills
        foreach (SkillData skill in unlockedActiveSkills)
        {
            // skill matches the id return found/true
            if (skill != null && skill.id == id) return true;
        }

        // using loop to go through all the passive skills
        foreach (SkillData skill in unlockedPassiveSkills)
        {
            // skill matches the id return found/true
            if (skill != null && skill.id == id) return true;
        }

        return false; // nothing found return false
    }
}

//// check for passive 
//public bool HasPassiveSkill(Skill_ID id)
//{
//    // using a loop to go through the skills 
//    foreach (SkillData skill in unlockedPassiveSkills)
//    {
//        // if the skill id is found and equal to id return true
//        if (skill != null && skill.id == id) return true; 
//    }

//    return false; // nothing matches return false
//}