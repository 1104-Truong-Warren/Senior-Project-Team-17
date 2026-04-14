//
// Weijun

using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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

    public Dictionary<SkillData, int> skillCooldown = new Dictionary<SkillData, int>(); // counting the skill cooldown

    public List<SkillData> EquippedActiveSkills => equippedActiveSkills; // accessor for the active skills

    public List<SkillData> UnlockedActiveSkills => unlockedActiveSkills; // accessor for the passive skills

    public List<SkillData> UnlockedPassiveSkills => unlockedPassiveSkills; // accessor for the passive skills

    public int MaxActiveSkillSlots => 4; // how many skills the player/enemy can equip as active skills

    private void Awake()
    {
        CheckActiveSkillSlots(); // call the check skill slot function
    }
    
    private void CheckActiveSkillSlots()
    {
        // set up all the unused slot to null
        while (equippedActiveSkills.Count < MaxActiveSkillSlots)
            equippedActiveSkills.Add(null);

        // if the list grows bigger than 4, only keep the frist 4 skills of the list
        if (equippedActiveSkills.Count > MaxActiveSkillSlots)
            equippedActiveSkills = equippedActiveSkills.Take(MaxActiveSkillSlots).ToList();
    }
    
    public void RemoveLockedSkills()
    {
        CheckActiveSkillSlots(); // check for active skills

        // using loop to go through all the skills slots
        for (int i = 0; i < MaxActiveSkillSlots; i++)
        {
            SkillData skill = equippedActiveSkills[i]; // reference as we go through each skill[i]

            // if the skill slot is empty keep on going
            if (skill == null) continue;

            // if the skill is not passed the active skill check set it to null
            if (!unlockedActiveSkills.Contains(skill))
            {
                Debug.Log($"[SA] Removing locked skills from slot:{i + 1} | name:{skill.skillDisplayName}"); // debug msg

                equippedActiveSkills[i] = null;
            }
        }
    }

    public SkillData GetActiveSkill(int index)
    {
        CheckActiveSkillSlots(); // check for active skills

        // if the index is bigger than 4 or less than 0 get out
        if (index < 0 || index >= equippedActiveSkills.Count) return null;

        return equippedActiveSkills[index]; // else return skill by index
    }

    public bool EquipActiveSkillToSlot(SkillData skill, int index)
    {
        CheckActiveSkillSlots(); // check the ative skills

        //  check to see if skill exist
        if (skill == null) return false;

        // if it's a passive skill get out
        if (skill.skillType != SkillType.Active) return false;

        // skill doesn't exist on slot get out
        if (!unlockedActiveSkills.Contains(skill)) return false;

        // skills is more than 4 or less than 1 get out
        if (index < 0 || index >= MaxActiveSkillSlots) return false;

        // skill is already equipped get out
        //if (equippedActiveSkills.Contains(skill)) return false;

        //// always fill the index slots
        //while (equippedActiveSkills.Count < 4)
        //{
        //    equippedActiveSkills.Add(null); // add a place holder
        //}

        // don't overwrrite the exist skill
        //if (equippedActiveSkills[index] != null) return false;

        // check for existing skill is the same
        for (int i = 0; i < equippedActiveSkills.Count; i++)
        {
            // if it's the same remove the first skill slot
            if (equippedActiveSkills[i] == skill)
                equippedActiveSkills[i] = null;
        }

        equippedActiveSkills[index] = skill; // if condition met add the skill to active skill to the index

        Debug.Log($"[SA] Move {skill.skillDisplayName} to slot:{index + 1}"); // debug msg
        return true;
    }

    public bool EquipActiveSkillToEmptySlot(SkillData skill)
    {
        CheckActiveSkillSlots(); // check the ative skills

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
        CheckActiveSkillSlots(); // check the ative skills

        // done in the checkSkillSlot function
        //// loop when the total skill is less than 4
        //while (equippedActiveSkills.Count < MaxActiveSkillSlots)
        //{
        //    equippedActiveSkills.Add(null); // add the placeholders
        //}

        // from 0 to total skill, if null is found return it
        for (int i = 0; i < MaxActiveSkillSlots; i++)
        {
            // if found return the spot
            if (equippedActiveSkills[i] == null) return i;
        }

        return -1; // return -1 if nothing is found which doesn't exist in a list
    }

    public bool UnequipActiveSkillByIndex(int index)
    {
        CheckActiveSkillSlots(); // check the ative skills

        // check if index is great than 1 and less than total skill count
        if (index < 0 || index >= MaxActiveSkillSlots) return false;

        // if the skill is null return 
        if (equippedActiveSkills[index] == null) return false;

        equippedActiveSkills[index] = null; // assign the slot to null
        return true;
    }

    public bool IsSkillEquipped(SkillData skill)
    {
        CheckActiveSkillSlots(); // check the ative skills

        // check to make sure skill reference is not empty
        if (skill == null) return false;

        return equippedActiveSkills.Contains(skill); // return the skill
    }

    public bool UnequipActiveSkill(SkillData skill)
    {
        CheckActiveSkillSlots(); // check the ative skills

        // skill is not found return false
        if (skill == null) return false;

        // loop through all the skills to find the skill
        for (int i = 0; i < MaxActiveSkillSlots; i++)
        {
            // found skill replace it with null
            if (equippedActiveSkills[i] == skill)
            {
                equippedActiveSkills[i] = null;
                return true; // true if found it
            }
        }

        return false; // false didn't find it
    }

    // version 3 changed to void
    public void UnlockSkill(SkillData skill)
    {
        //  check to see if skill exist
        if (skill == null) return;

        // check to see if skill exist in the list
        if (HasUnlockedSkill(skill)) return;

        Debug.Log($"[SA] UnlockSkill for:{skill.skillDisplayName} | Skill Type:{skill.skillType}"); // debug msg

        // check to see if the skill type matches
        if (skill.skillType == SkillType.Active)
        {
            // if the skill list doesn't have the skill add it to list
            if (!unlockedActiveSkills.Contains(skill))
                unlockedActiveSkills.Add(skill); // if not exsit add it to the list

            Debug.Log($"[SA] Active Skill unlocked count:{unlockedActiveSkills.Count}"); // debug msg
        }

        // check to see if the skill type matches
        else if (skill.skillType == SkillType.Passive)
        {
            // if the skill list doesn't have the skill add it to list
            if (!unlockedPassiveSkills.Contains(skill))
                unlockedPassiveSkills.Add(skill); // if not exsit add it to the list
        }

        //return false; // if nothing found return false
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

    // Skill cooldown related
    public void SetSkillCooldown(SkillData skill)
    {
        // check if the skill is found
        if (skill == null) return;

        // if the skill cooldown is larger than 0
        if (skill.skillCoolDown > 0)
        {
            skillCooldown[skill] = skill.skillCoolDown; // save skill.cd to the List

            Debug.Log($"[SA] Set skillCooldown for:{skill.skillDisplayName} to {skill.skillCoolDown}"); // debug msg
        }
    }

    public void CooldownCountDown()
    {
        List<SkillData> keys = new List<SkillData>(skillCooldown.Keys); // get the skill.cd int from the list and make a new list

        // go through the skill in the list that contains int keys
        foreach (SkillData skill in keys)
        {
            // if the skill has a cd go down by one
            if (skillCooldown[skill] > 0)
            {
                skillCooldown[skill]--; // cooldown goes down 

                Debug.Log($"[SA] Set skillCooldown for:{skill.skillDisplayName} to {skill.skillCoolDown}"); // debug msg
            }
        }
    }

    public int GetCooldownRemaining(SkillData skil)
    {
        // try to see if skill is found
        if (skil == null) return 0;

        // try to get the int from the list if the cooldown is larget than 0 return that
        if (skillCooldown.TryGetValue(skil, out int cd) && cd > 0) return cd;

        return 0; // nothing found return 0;
    }

    public bool IsSkillOnCooldown(SkillData skil)
    {
        return GetCooldownRemaining(skil) > 0; // returns the bool using GetCooldonRemaining() function check if it has return T, else F
    }

    public void ClearSkillCooldown(SkillData skill)
    {
        // check to see if skill is found
        if (skill == null) return;

        // if the skill list has the skill, remove it from the list
        if (skillCooldown.ContainsKey(skill))
            skillCooldown.Remove(skill);   
    }

    public void ClearAlSkillCooldowns()
    {
        skillCooldown.Clear(); // clear all the skill.cd list
    }

    // debug helper
    public void DebugPrintSkillSlots()
    {
        CheckActiveSkillSlots(); 

        // equipped test
        for (int i = 0; i < MaxActiveSkillSlots; i++)
        {
            SkillData skill = equippedActiveSkills[i]; // skill is the current equipped skill

            Debug.Log($"[SA] Slot:{i + 1} = {(skill != null ? skill.skillDisplayName : "EMPTY")}"); // debug msg
        }
    }

    // debug for skill names
    public void DebugPrintUnlockedSkills()
    {
        // loop through all skill names
        foreach (SkillData skill in unlockedActiveSkills)
        {
            Debug.Log($"[SA] Unlocked Activve skill:{skill.skillDescription}"); // debug msg
        }
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