// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU
// used to see how scriptableObject works URL: https://www.youtube.com/watch?v=cy49zMBZvhg
// Weijun

using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "PlayerSkills/Skill Data")] // what the file is called and what kind of file it is
public class SkillData : ScriptableObject
{
    [Header("Skill Name")]
    public Skill_ID id; // access to the skills

    [Header("Skill Type")]
    public SkillType skillType; // active/passive
    public TargetType targetType; // belongs to who? player/enemy
    public WeaponType weaponType; // what weapn?/any?
    public ClassType classType; // melee/range
    public SubClassType subClassType; // SwordMaster/Berserker 
    public SkillEffectType skillEffectType; // damage/heal/buff

    [Header("Skill Descriptions/Display")]
    public string skillDisplayName; // ingame name
    [TextArea] public string skillDescription; // basic attack

    [Header("Skill Combat stats (active skilks)")]
    [Range(0, 100)] public int HitRate; // skill hit rate
    [Range(0, 100)] public int CritChance; // skill crit rate
    public int AttackDamage; // dmg of attack
    public int AttackRange; // range of the attack

    [Header("Skill Combat stats (passive skills")]
    [Range(0, 100)] public int critBonus; // extra stats add to base stats
    [Range(0, 100)] public int critDmgBonus; // extra stats add to base stats
    [Range(0, 100)] public int hitRateBonus; // extra stats add to base stats
    [Range(0f, 1f)] public float hpRecoverP; // recover hp in %
    [Range(0f, 1f)] public float enRecoverP; // recover en in %
    public int attackBonus; // extra stats add to base stats
    public int hpBonus; // extra stats add to base stats
    public int enBouns; // extra stats add to base stats
    public int movementBonus; // extra stats add to base stats
    public int apBonus; // extra stats add to base stats

    [Header("Skill Cost/Duration (EN, AP, CD)")]
    // =========== AP/EN cost =====================
    public int skillENCost; // how much energy it takes to use
    public int skillAPCost; // AP cost
    public int skillCoolDown; // skill cooldown
    public int skillDuration; // how long it lasts

    [Header("Unlock requirements")]
    [Range(0, 20)] public int requiredLevel; // what's the requirement level
    public Skill_ID[] requirdSkills; // how to unlock the skill
}

//[Header("Basic Attck Display")]
////  ============ Basic Attack ===============
//public string Attack; // basic attack

//[Header("Basic Attk Crit rate")]
//// =========== EN cost =====================
//public int AttkCritChance = 0; // skill crit rate

//[Header("Basic Attk AP cost")]
//// ============= AP Cost ===================
//public int AttkAPCost = 1;

//[Header("Skill 1 Display SwordSlash")]
//// ============ Skill 1 ====================
//public string SwordS; // skill name

//[TextArea] public string SSDescription; // discrption

//[Header("Skill Type")]
// ======== passive ===========
// public bool ExAP; 
