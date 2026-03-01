//
// Weijun

using UnityEngine;

public static class HitRollCheck
{
    public static bool HitRollPercent(int hitRate)
    {
        // start from hitRate to 100, if the roll is less than or equal to hitRate passes, else misses
        hitRate = Mathf.Clamp(hitRate, 0, 100); // rate between 100

        int roll = Random.Range(1, 101); // keep them between 1-100

        return roll <= hitRate; // if outcome is less than roll hit
    }

    public static int FinalHitChanceCal(int baseHitR, int skillHitR, int enemyEvasion, int min = 5, int max = 95)
    {
        int hitChance = (baseHitR + skillHitR) - enemyEvasion; // hit Chance is skill+base hitRate - enemy evasion stats

        return Mathf.Clamp(hitChance, min, max); // return the hit Chance, prevent out of range it has to be in 1 - 100 range
    }

    public static int FinalCritChanceCal(int baseCrit, int skillCritB, int min = 10, int max = 100)
    {
        return Mathf.Clamp((baseCrit + skillCritB), min, max); // return the base crit + skill crit, has to be in 0 - 100 range
    }

    public static int CritHit(int dmg, int critDmg)
    {
        float cirtMul = 1f + (critDmg / 100f); // 1 + the crit dmg / 100, 50/100 = .5 + 1, 1.5 more dmg

        return Mathf.RoundToInt(dmg * cirtMul); // returns the dmg * crit multiplier
    }
}
