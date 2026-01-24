// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU
// used to see how scriptableObject works URL: https://www.youtube.com/watch?v=cy49zMBZvhg


using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "PlayerSkills/Ative Skills, Passive Skils")] // what the file is called and what kind of file it is
public class SkillData : ScriptableObject
{
    public Skill_ID id; // access to the skills

    [Header("Basic Attck Display")]
    //  ============ Basic Attack ===============
    public string Attack; // basic attack

    [TextArea] public string AttkDescription; // basic attack

    [Header("Basic Attk EN cost")]
    // =========== EN cost =====================
    public int AttkENCost = 5; // how much energy it takes to use

    [Header("Basic Attk AP cost")]
    // ============= AP Cost ===================
    public int AttkAPCost = 1;

    [Header("Skill 1 Display SwordSlash")]
    // ============ Skill 1 ====================
    public string SwordS; // skill name

    [TextArea] public string SSDescription; // discrption

    [Header("SwordSlash EN cost")]
    // =========== EN cost =====================
    public int SSENCost; // how much energy it takes to use


    [Header("Unlock requirements")]
    public Skill_ID[] requirements; // how to unlock the skill

    //[Header("Skill Type")]
    // ======== passive ===========
    // public bool ExAP; 

}
