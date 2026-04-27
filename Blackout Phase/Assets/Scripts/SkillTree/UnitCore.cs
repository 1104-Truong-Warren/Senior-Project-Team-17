//
// Weijun

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCore : MonoBehaviour
{
    private List<StatusEffectBuff> activeEffectBuffs = new List<StatusEffectBuff>(); // list of the active buffs

    private List<StatsModifier> passiveModifiers = new List<StatsModifier>(); // list of the passives modifiers

    // the abstract skillcore for skillExecutor 
    public abstract OverlayTile1 CurrentTile { get; } // get tile

    // get stats
    public abstract int CurrentHP { get; } 
    public abstract int MaxHP { get; }

    // get combat stats
    public abstract int HitRate { get; }
    public abstract int EvasionRate { get; }
    public abstract int AttackRange { get; }
    public abstract int MoveRange { get; }
    public abstract int BaseAttack { get; }
    public abstract int CritRate { get; }

    // taking dmg
    public abstract void TakeDamage(int dmg);

    // is the unit dead?
    public virtual bool IsDead()
    {
        return CurrentHP <= 0;
    }

    public void AddBuffEffect(StatusEffectBuff newBuffEffect)
    {
        // check if the buff is vaild
        if (newBuffEffect == null) return;

        newBuffEffect.unit = this; // get the user of the buff

        activeEffectBuffs.Add(newBuffEffect); // add it to the list

        Debug.Log($"[UC] {gameObject.name} gained buff effect: {newBuffEffect.buffEffectName}"); // debug msg
    }

    public void RemoveBuffEffect(StatusEffectBuff buffEffect)
    {
        // check if the buff is vaild
        if (buffEffect == null) return;

        // check to see if the list is empty
        if (activeEffectBuffs != null)
        {
            activeEffectBuffs.Remove(buffEffect); // removes the buff effect

            Debug.Log($"[UC] {gameObject.name} removed buff effect: {buffEffect.buffEffectName}"); // debug msg
        }
    }

    public void DebugActiveBuffs()
    {
        Debug.Log($"[UC] {gameObject.name} active buff count: {activeEffectBuffs.Count}"); // debug msg

        // go through all the buffs in active buff list
        foreach (var buffEffect in activeEffectBuffs)
        {
            // skip empty ones
            if (buffEffect == null) continue;

            Debug.Log($"[UC]  Buff: {buffEffect.buffEffectName} | Turns left: {buffEffect.remainingBuffTurns}"); // debug msg
        }
    }

    public void DebugStats()
    {
        Debug.Log($"[UC] {name} Attack: {BaseAttack}"); // debug msg

        //Debug.Log($"[UC] {name} MoveRange: {}"); // debug msg

        Debug.Log($"[UC] {name} AttackRange: {AttackRange}"); // debug msg

        Debug.Log($"[UC] {name} HitRate: {HitRate}"); // debug msg

        Debug.Log($"[UC] {name} Evasion: {EvasionRate}"); // debug msg
    }

    public void TickDownBuffEffects(BuffEffectTimer timer)
    {
        // loop though all the buffs in the list
        for (int i = activeEffectBuffs.Count - 1; i >= 0; --i)
        {
            StatusEffectBuff buffEffect = activeEffectBuffs[i]; // accesss the buff

            // skip the empty ones
            if (buffEffect == null) continue;

            // skip the ones that doesn't match the same CD
            if (buffEffect.tickTimer != timer) continue;

            --buffEffect.remainingBuffTurns; // goes down by one

            // if the buff duradaation is 0 remove it
            if (buffEffect.remainingBuffTurns <= 0)
            {
                Debug.Log($"[UC] {gameObject.name} buff effect expired: {buffEffect.buffEffectName}"); // debug msg

                activeEffectBuffs.RemoveAt(i); // removes the element from the list 
            }
        }
    }

    // find the flat bonus from the lists using foreach loop
    public int GetModifiedStats(StatsType statsType, int baseValue)
    {
        int flatBounusValue = 0; // varable to hold the bonus value

        // go through the passiveModifer list 
        foreach (var mod in passiveModifiers)
        {
            // check if the stats type is flat bonus not percent addons
            if (mod.statsType == statsType && !mod.isPercent)
                flatBounusValue += mod.value; // if it's not percent add it to total flat
        }

        // loop through the effect buffs in the List 
        foreach (var effectBuff in activeEffectBuffs)
        {
            // loop through the skill buff modifiers 
            foreach (var mod in effectBuff.modifiers)
            {
                // they stats type is not in precent add it to flatValue 
                if (mod.statsType == statsType && !mod.isPercent)
                    flatBounusValue += mod.value;
            }
        }

        return flatBounusValue + baseValue; // return the bonus value + baseValue 
    }

    // passive modifier functions
    public void AddPassiveModifierToUnit(StatsModifier modifier)
    {
        // check to make sure modifier is not empty
        if (modifier == null) return;

        passiveModifiers.Add(modifier); // add it to the list

        Debug.Log($"[UC] {name} gained passive: {modifier.statsType} | {modifier.value}"); // debug msg
    }

    public void ClearPassiveModifiers()
    {
        passiveModifiers.Clear(); // clear the list, good for dead enemies/player
    }

    public void LoadPassiveModFromSA(SkillAttachment skillAttachment)
    {
        ClearPassiveModifiers(); // make sure the list is empty, clear it

        // make sure SA is not empty
        if (skillAttachment == null) return;

        // loop through the passive skill in the skill attach ment List to find unlocked passive skills
        foreach (SkillData passiveSkill in skillAttachment.UnlockedPassiveSkills)
        {
            // skip empty passives
            if (passiveSkill == null) continue;

            // loop through the modifiers in passiveSkill list
            foreach (var mod in passiveSkill.passiveModifiers)
            {
                // add the stats using the modifer function
                AddPassiveModifierToUnit(new StatsModifier
                {
                    statsType = mod.statsType, // type of stats

                    value = mod.value, // value

                    isPercent = mod.isPercent // is the value percent?
                });
            }
        }

        Debug.Log($"[UC] {name} passive modifers built!"); // debug msg
    }
}
