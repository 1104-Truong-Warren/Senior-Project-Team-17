// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU
// Weijun

public enum Skill_ID
{
    // ----------------------- Active Skills ------------------------------
    // ======================= Attack Skills ==============================
    // Active Skills ID/names
    NormalAttack, // normal attack 

    // ======================= Buff Skills ================================
    // Buff Skills, temperature stats up/ Recovery
    // ====================================================================
    MechaCheck, // restore Hp/En

    // Sword Master
    SwordSlash, // melee attack, melee class, SM 

    SwordToss, // throws the swords to target

    UltimateBladeWorks, // secret sword move that deals tons of damge and gain attack buff/movement buff

    // ======================= Buff Skills ================================
    // Buff Skills, temperature stats up
    // ====================================================================
    // Sword Master
    SwordStand, // changing stand to sword stance, boost stats temporarily  

    // ---------------------- Passive skills ------------------------------
    // ====================== Melee Passive ===============================
    // ====================== SM Sub Class passive ===============================
    Swordsman, // passive for SM sub class gain extera hp/en

    MindsEyes, // passive for SM sub class gain crit buff/evasion/attack up lvl3

    RageGoAgain, // when you slay enemies depending on level how many times you can attack before next turn (alway crits attive after 3 enemy kills)

    ExtraAP, // by unlocking the passive get more AP points per turn

    ExtraMovement, // more move range level or skill dependent 

    // --------------------- Enemy Skills ---------------------------------
    // ===================== Melee Enemy ==================================
    NormalSlash, // enemy normal attack

    NormalCanon, // range normal attack

    HeavySlam, // higher damage/ with EN reduction

    // ==================== passive skils =================================
    AttackUpSmall // lvl 1 attack up
}
