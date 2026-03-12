// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU
// Weijun

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static PlasticPipe.PlasticProtocol.Messages.Serialization.ItemHandlerMessagesSerialization;

public class PlayerCombatCheck : MonoBehaviour
{
    public static PlayerCombatCheck Instance { get; private set; } // copies static data 

    //[Header("Test skills")]
    //[SerializeField] private SkillData skill1;
    //[SerializeField] private SkillData skill2;
    //[SerializeField] private SkillData skill3;
    //[SerializeField] private SkillData skill4;

    [Header("Player Skill Key settings")]
    [SerializeField] private KeyCode skillKey1; // skill 1
    [SerializeField] private KeyCode skillKey2; // skill 2
    [SerializeField] private KeyCode skillKey3; // skill 3
    [SerializeField] private KeyCode skillKey4; // skill 4

    [Header("Active Skill Settings")]
    [SerializeField] private SkillAttachment playerSkillattach; // accessor the player skills
    [SerializeField] private PlayerSkillExecutor playerSkillExecutor; // accessor to the skill executor
    [SerializeField] private int playerSkillIndexSelect; // index for player skill

    // Added by Warren, sound effect attributes for the player.
    [Header("Sound Effects")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip missSound;


    public int playerSkillIndexCheck => playerSkillIndexSelect; // accessor for other scripts

    //// ================== SkillData Settings ===========================
    //[SerializeField] private SkillData SDbasicAttkSkill; // for access the skill data's data

    private CharacterInfo1 playerInfo; // to access playerInfo

    private void Awake()
    {
        // if Innstance(copy) exsist and not the same as pointer deleted
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this; // setup the this pointer

        // the player attack is null setup 
        if (playerSkillattach == null)
            playerSkillattach = GetComponent<SkillAttachment>();

        // skill executor null set up
        if (playerSkillExecutor == null)
            playerSkillExecutor = GetComponent<PlayerSkillExecutor>();

        PlayerSetUp(); // void function to set up the player status

        SetupSkillKeyInput(); // set up the skill keys
    }

    //private void Start()
    //{
    //    TestLoadSkills(); // test for the skills
    //}

    private void Update()
    {
        SetupSkillKeyInput();
    }

    public void PlayerAttackCheck(EnemyInfo enemy)
    {
        // make sure it's player's turn
        if (TurnManager.Instance.State != TurnState.PlayerAction) return;

        SkillData currentSkill = playerSkillattach.GetActiveSkill(playerSkillIndexSelect); // setup the currentSkill

        var player = CharacterInfo1.Instance; // make a copy of characterInfo as reference

        // check if the player is able to use the skill 
        if (!CanPlayerUseSkill(currentSkill, player, enemy)) return;

        TurnManager.Instance.PlayerSpendAP(currentSkill.skillAPCost); // still cost ap if pass the test

        player.PlayerSpendEN(currentSkill.skillENCost); // EN goes down before attack lands

        bool enemyDodge = EnemyReactToAttack(currentSkill, player, enemy); // did the attack hit?

        // if enemy dodged return 
        if (enemyDodge)
        {
            Debug.Log("Enemy dodged the player's attack!"); // debug msg
            PlaySound(missSound); // Added by Warren
            return;
        }

        // if player attack missed return
        if (!PlayerAttackHits(currentSkill, player, enemy))
        {
            PlaySound(missSound); // Added by Warren
            return;
        }

        int dmg = CalculatePlayerSkillDmg(currentSkill, player, enemy); // for the final attack crit hit
        PlaySound(attackSound); // Added by Warren

        // making sure the dmg is vaild
        if (dmg <= 0) return;

        Debug.Log($"Enemy taking:{dmg} dmg"); // debug msg

        enemy.EnemyTakeDamage(dmg); // calls the dmamge founction pass the amount

        // Added by Warren, for player's damage UI on the enemy
        if (DamageObserver.Instance != null)
        {
            DamageObserver.Instance.ShowPlayerDamage(dmg, enemy.transform.position);
        }
    }

    public void PlayerCounterAttack(EnemyInfo enemy)
    {
        SkillData currentSkill = playerSkillattach.GetActiveSkill(playerSkillIndexSelect); // setup the currentSkill

        var player = CharacterInfo1.Instance; // make a copy of characterInfo as reference

        // check if the player is able to use the skill 
        if (!CanPlayerUseSkill(currentSkill, player, enemy)) return;

        player.PlayerSpendEN(currentSkill.skillENCost); // EN goes down before attack lands

        bool enemyDodge = EnemyReactToAttack(currentSkill, player, enemy); // did the attack hit?

        // if enemy dodged return 
        if (enemyDodge)
        {
            Debug.Log("Enemy dodged the player's attack!"); // debug msg
            return;
        }

        // if player attack missed return
        if (!PlayerAttackHits(currentSkill, player, enemy)) return;

        // if the enemy react to player dodged get out
        if (EnemyReactToAttack(currentSkill, player, enemy)) return;

        int dmg = CalculatePlayerSkillDmg(currentSkill, player, enemy); // for the final attack crit hit

        // making sure the dmg is vaild
        if (dmg <= 0) return;

        Debug.Log($"Enemy taking:{dmg} dmg"); // debug msg

        enemy.EnemyTakeDamage(dmg); // calls the dmamge founction pass the amount

        // Added by Warren, for player's damage UI on the enemy
        if (DamageObserver.Instance != null)
        {
            DamageObserver.Instance.ShowPlayerDamage(dmg, enemy.transform.position);
        }
    }

    private bool PlayerCritOrFuryActiveCheck(int critChance)
    {
        // if player is active return t
        if (PlayerFuryMode.Instance.inFuryMode)
            return true;

        return HitRollCheck.HitRollPercent(critChance); // or check to see if crit roll passes
    }

    public void PlayerSetUp()
    {
        playerInfo = GetComponent<CharacterInfo1>(); // setup the playerInfo

        // if player exist set up the status
        if (playerInfo != null)
        {
            playerSkillattach = playerInfo.GetComponent<SkillAttachment>(); // get the skills from player attachment

            //PlayerStatusSetUp(); // set up player status
            return;
        }

        // ======= if playerInfo still null ========
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player1"); // find it using game object tag

        // if found set up the basic stats for player1
        if (playerObj != null)
        {
            playerInfo = playerObj.GetComponent<CharacterInfo1>(); // set up the playerInfo using object

            playerSkillattach = playerInfo.GetComponent<SkillAttachment>(); // get the skills from player attachment

            //PlayerStatusSetUp(); // calls the player stats function
        }
    }

    private void SetupSkillKeyInput()
    {
        // connect the keyinput to the skill index
        if (Input.GetKeyDown(skillKey1))
        {
            Debug.Log("Pressed SKill Key 1"); // debug msg

            SetSelectedSkillIndex(0);
        }

        else if (Input.GetKeyDown(skillKey2))
        {
            Debug.Log("Pressed SKill Key 2"); // debug msg

            SetSelectedSkillIndex(1);
        }

        else if (Input.GetKeyDown(skillKey3))
        {
            Debug.Log("Pressed SKill Key 3"); // debug msg

            SetSelectedSkillIndex(2);
        }

        else if (Input.GetKeyDown(skillKey4))
        {
            Debug.Log("Pressed SKill Key 4"); // debug msg

            SetSelectedSkillIndex(3);
        }
    }

    private int CalculatePlayerSkillDmg(SkillData skill, CharacterInfo1 player, EnemyInfo enemy)
    {
        // check to see if enemy and skill exist
        if (skill == null || enemy == null) return -1;

        // if player is not found or player tile not found get out
        if (player == null || player.CurrentTile == null) return -1;

        int dmg = (player.BaseAttk + skill.AttackDamage); // for the final attack crit hit

        int critChance = HitRollCheck.FinalCritChanceCal(player.BaseCriticalRate, skill.CritChance); // find the crit chance of attack

        // check for crit roll
        if (PlayerCritOrFuryActiveCheck(critChance))
        {
            dmg = HitRollCheck.CritHit(dmg, (skill.critDmgBonus + player.BaseCritDamage)); // crit dmg calculation

            // if player in Fury mode display crit in Fury mode
            if (PlayerFuryMode.Instance.inFuryMode)
                Debug.Log("In Fury Mode Attacker Critital Hit");

            else
                Debug.Log($"Counter Critical Hit: Damage:{dmg}"); // debug msg
        }

        Debug.Log($"Enemy taking:{dmg} dmg"); // debug msg

        return dmg;
    }

    private bool PlayerAttackHits(SkillData skill, CharacterInfo1 player, EnemyInfo enemy)
    {
        // check to see if enemy and skill exist
        if (skill == null || enemy == null) return false;

        // if player is not found or player tile not found get out
        if (player == null || player.CurrentTile == null) return false;

        int hitChance = HitRollCheck.FinalHitChanceCal(player.BaseHitRate, skill.HitRate, enemy.EvasionRate); // pass over the data to roll a hit

        Debug.Log($"HitChance:{hitChance}"); // debug msg

        // check for hit roll
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log("Attack MISS!"); // debug msg

            // Added by Warren, text that shows on the screen
            if (DamageObserver.Instance != null)
            {
                DamageObserver.Instance.ShowMissText(enemy.transform.position);
            }

            player.PlayerSpendEN(skill.skillENCost); // EN goes down before attack lands

            return false;
        }

        return true; // if hit return true
    }

    private bool CanPlayerUseSkill(SkillData skill, CharacterInfo1 player, EnemyInfo enemy)
    {
        // check to see if enemy and skill exist
        if (skill == null || enemy == null) return false;

        // if player is not found or player tile not found get out
        if (player == null || player.CurrentTile == null) return false;

        // check if enemy still has health left
        if (enemy.CurrentHP <= 0) return false;

        // if player AP is not enough display message
        if (player.currentAP < skill.skillAPCost)
        {
            Debug.Log("Not enough AP to attack!");
            return false;
        }

        // check if the player has enough EN
        if (!player.PlayerEnCheck(skill.skillENCost))
        {
            Debug.Log("Insufficent EN amount!"); // debug msg
            return false;
        }

        OverlayTile1 playerTile = player.CurrentTile; // player tile location

        OverlayTile1 enemyTile = enemy.currentTile; //MapManager1.Instance.GetWorldTileFromTransform(enemy.transform); // enemy tile location

        //var e = enemyTile.gridLocation; // enemy tile location

        // if the enemy tile is empty get out
        if (enemyTile == null || playerTile == null)
        {
            Debug.Log("Player/Enemy Tile not found!"); // debug msg
            return false;
        }
        int distance = Manhattan(playerTile.gridLocation, enemyTile.gridLocation); //Mathf.Abs(playerTile.gridLocation.x - enemyTile.gridLocation.x) + Mathf.Abs(playerTile.gridLocation.y - enemyTile.gridLocation.y);

        Debug.Log($"Distance = {distance}, PlayerAttkRange:{skill.AttackRange} (PlayerBaseRange:{player.BaseRange})");

        // check to see if enemy is in range for attk
        if (distance > skill.AttackRange)
        {
            Debug.Log("Enemy out of reach!");
            return false;
        }

        return true; // true if passed all the test
    }

    private bool EnemyReactToAttack(SkillData skill, CharacterInfo1 player, EnemyInfo enemy)
    {
        // check to see if player, skill, enemy exist
        if (skill == null || player == null || enemy == null) return false;

        EnemyReactionController enemyReact = enemy.GetComponent<EnemyReactionController>(); // set up the enemy ract access

        // does the enemy reaction exist? 
        if (enemyReact == null) return false;

        bool dodge = enemyReact.ReactToPlayerAttack(player, skill.HitRate); // check for enemy dodged 

        // did the dodge went through?
        if (dodge)
        {
            Debug.Log("Enemy Dodged! Attack Missed!"); // debug msg
            return true; // dodge 
        }

        return false;  // hit
    }

    public void UseSelectedSkill(EnemyInfo enemy = null)
    {
        SkillData currentSkill = GetCurrentSkill(); // find the current skill

        // if skill not foun return
        if (currentSkill == null) return;

        // switch statment to decided what tyep of skills player is using
        switch (currentSkill.skillEffectType)
        {
            case SkillEffectType.Damage: // damage skill

                // if enemy not found return
                if (enemy == null) return;

                PlayerAttackCheck(enemy); // if enemy found attack
                break;

            case SkillEffectType.Heal: // heal skill
                playerSkillExecutor.UseSkill(currentSkill);
                break;
        }

    }

    public void SetSelectedSkillIndex(int index)
    {
        // if player has no skills attach get out
        if (playerSkillattach == null) return;

        // make sure index is correct, can't be less than 0, and greater than the total equipped skils
        if (index < 0 || index >= playerSkillattach.EquippedActiveSkills.Count) return;

        SkillData selectedSkill = playerSkillattach.GetActiveSkill(index); // set up the reference active skill index from skill attachment

        // check to see if the selected skill exist
        if (selectedSkill == null)
        {
            Debug.Log($"Skill slot:{index + 1} is empty not skill!"); // debug msg
            return;
        }

        playerSkillIndexSelect = index; // setup the player skill index

        Debug.Log($"Skill slot:{index + 1} : {selectedSkill.skillDisplayName}"); // debug msg
    }

    public SkillData GetCurrentSkill()
    {
        // check too see if player skill attach is null
        if (playerSkillattach == null) return null;

        return playerSkillattach.GetActiveSkill(playerSkillIndexCheck); // returns the skill 
    }

    private int Manhattan(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // returns the player/enemy distance
    }

    // Added by Warren, function is used to play the audio clip.
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

//private void TestLoadSkills()
//{
//    //
//    if (playerSkillattach == null) return;

//    playerSkillattach.UnlockSkill(skill1);
//    playerSkillattach.UnlockSkill(skill2);
//    playerSkillattach.UnlockSkill(skill3);
//    playerSkillattach.UnlockSkill(skill4);

//    playerSkillattach.EquipActiveSkillToSlot(skill1, 0);
//    playerSkillattach.EquipActiveSkillToSlot(skill2, 1);
//    playerSkillattach.EquipActiveSkillToSlot(skill3, 2);
//    playerSkillattach.EquipActiveSkillToSlot(skill4, 3);
//}


// just for testing
//[Header("Temp basic attk settings")]
//[SerializeField] private int basicAttkDamage; // player base damage
//[SerializeField] private int basicAttckRange; // B attk range
//[SerializeField] private int basicAttkAPcost; // ap cost for Battack
//[SerializeField] private int basicAttkENcost; // en cost for Battack
//[SerializeField] private int basicAttkHitRate; // base skill hitRate
//[SerializeField] private int basicAttkSkillCrit; // base skill crit chance
//[SerializeField] private int basicAttkCritDmg; // base attk crit dmg

//private void PlayerStatusSetUp()
//{
//    basicAttkDamage = playerInfo.BaseAttk; // basic attk stats

//    basicAttckRange = playerInfo.BaseRange; // basic attk range

//    basicAttkCritDmg = playerInfo.BaseCritDamage; // basic crit dmg

//    basicAttkAPcost = SDbasicAttkSkill.AttkAPCost; // basic Attk AP cost

//    basicAttkENcost = SDbasicAttkSkill.AttkENCost; // basic attk EN cost

//    basicAttkHitRate = SDbasicAttkSkill.HitRate; // basic attk hit rate

//    basicAttkSkillCrit = SDbasicAttkSkill.CritChance; // basic attk crit add on
//}



//// debug msg in PlayerStatusSetUp()
//if (playerInfo == null)
//{
//    Debug.LogWarning("playerInfo is null!");
//    return;
//}

//// debug msg
//if (SDbasicAttkSkill == null)
//{
//    Debug.LogWarning("SDbasicAttkSkill is null!");
//    return;
//}

// Old Version
//public void PlayerCounterAttack(EnemyInfo enemy)
//{
//    SkillData currentSkill = playerSkillattach.GetActiveSkill(playerSkillIndexSelect); // setup the currentSkill

//    // check too see if the skill exist
//    if (currentSkill == null)
//    {
//        Debug.Log("No skill equipped in that skill slot!"); // debug msg
//        return;
//    }

//    // enemy not found get out
//    if (enemy == null) return;

//    var player = CharacterInfo1.Instance; // make a copy of characterInfo as reference

//    // if player is not found or player tile not found get out
//    if (player == null || player.CurrentTile == null) return;

//    // check if enemy still has health left
//    if (enemy.CurrentHP <= 0) return;

//    int hitChance = HitRollCheck.FinalHitChanceCal(playerInfo.BaseHitRate, currentSkill.HitRate, enemy.EvasionRate); // pass over the data to roll a hit

//    Debug.Log($"HitChance:{hitChance}"); // debug msg

//    // check if the player has enough EN
//    if (!playerInfo.PlayerEnCheck(currentSkill.AttkENCost))
//    {
//        Debug.Log("Insufficent EN amount!"); // debug msg
//        return;
//    }

//    CharacterInfo1.Instance.PlayerSpendEN(currentSkill.AttkENCost); // EN goes down  

//    // check for hit roll
//    if (!HitRollCheck.HitRollPercent(hitChance))
//    {
//        Debug.Log("Counter Attack MISS!"); // debug msg

//        //TurnManager.Instance.PlayerSpendAP(basicAttkAPcost); // still - ap
//        return;
//    }

//    int dmg = (player.BaseAttk + currentSkill.AttackDamage); // for the final attack crit hit

//    int critChance = HitRollCheck.FinalCritChanceCal(playerInfo.BaseCriticalRate, currentSkill.CritChance); // find the crit chance of attack

//    // check for crit roll
//    if (PlayerCritOrFuryActiveCheck(critChance))
//    {
//        dmg = HitRollCheck.CritHit(dmg, (player.BaseCritDamage + currentSkill.critDmgBonus)); // crit dmg calculation

//        // if player in Fury mode display crit in Fury mode
//        if (PlayerFuryMode.Instance.inFuryMode)
//            Debug.Log("In Fury Mode Attacker Critital Hit");

//        else
//            Debug.Log($"Counter Critical Hit: Damage:{dmg}"); // debug msg
//    }

//    var enemyReact = enemy.GetComponent<EnemyReactionController>(); // access enemyReaction

//    // if the enemyReact is found 
//    if (enemyReact != null)
//    {
//        bool dodged = enemyReact.ReactToPlayerAttack(CharacterInfo1.Instance, (currentSkill.HitRate + player.BaseHitRate)); // copies player stats over, and use the function to check the flag

//        // is the flag true? is yest player missed
//        if (dodged)
//        {
//            Debug.Log("Enemy Dodged! Attack Missed"); // debug msg

//            return;
//        }
//    }

//    enemy.EnemyTakeDamage(dmg); // calls the dmamge founction pass the amount

//    // Added by Warren, for player's damage UI on the enemy 
//    DamageObserver.Instance.ShowPlayerDamage(dmg, enemy.transform.position);
//}


//public void PlayerCounterAttack(EnemyInfo enemy)
//{
//    SkillData currentSkill = playerSkillattach.GetActiveSkill(playerSkillIndexSelect); // setup the currentSkill

//    // check too see if the skill exist
//    if (currentSkill == null)
//    {
//        Debug.Log("No skill equipped in that skill slot!"); // debug msg
//        return;
//    }

//    // enemy not found get out
//    if (enemy == null) return;

//    var player = CharacterInfo1.Instance; // make a copy of characterInfo as reference

//    // if player is not found or player tile not found get out
//    if (player == null || player.CurrentTile == null) return;

//    // check if enemy still has health left
//    if (enemy.CurrentHP <= 0) return;

//    int hitChance = HitRollCheck.FinalHitChanceCal(playerInfo.BaseHitRate, currentSkill.HitRate, enemy.EvasionRate); // pass over the data to roll a hit

//    Debug.Log($"HitChance:{hitChance}"); // debug msg

//    // check if the player has enough EN
//    if (!playerInfo.PlayerEnCheck(currentSkill.AttkENCost))
//    {
//        Debug.Log("Insufficent EN amount!"); // debug msg
//        return;
//    }

//    CharacterInfo1.Instance.PlayerSpendEN(currentSkill.AttkENCost); // EN goes down  

//    // check for hit roll
//    if (!HitRollCheck.HitRollPercent(hitChance))
//    {
//        Debug.Log("Counter Attack MISS!"); // debug msg

//        //TurnManager.Instance.PlayerSpendAP(basicAttkAPcost); // still - ap
//        return;
//    }

//    int dmg = (player.BaseAttk + currentSkill.AttackDamage); // for the final attack crit hit

//    int critChance = HitRollCheck.FinalCritChanceCal(playerInfo.BaseCriticalRate, currentSkill.CritChance); // find the crit chance of attack

//    // check for crit roll
//    if (PlayerCritOrFuryActiveCheck(critChance))
//    {
//        dmg = HitRollCheck.CritHit(dmg, (player.BaseCritDamage + currentSkill.critDmgBonus)); // crit dmg calculation

//        // if player in Fury mode display crit in Fury mode
//        if (PlayerFuryMode.Instance.inFuryMode)
//            Debug.Log("In Fury Mode Attacker Critital Hit");

//        else
//            Debug.Log($"Counter Critical Hit: Damage:{dmg}"); // debug msg
//    }

//    var enemyReact = enemy.GetComponent<EnemyReactionController>(); // access enemyReaction

//    // if the enemyReact is found 
//    if (enemyReact != null)
//    {
//        bool dodged = enemyReact.ReactToPlayerAttack(CharacterInfo1.Instance, (currentSkill.HitRate + player.BaseHitRate)); // copies player stats over, and use the function to check the flag

//        // is the flag true? is yest player missed
//        if (dodged)
//        {
//            Debug.Log("Enemy Dodged! Attack Missed"); // debug msg

//            return;
//        }
//    }

//    enemy.EnemyTakeDamage(dmg); // calls the dmamge founction pass the amount

//    // Added by Warren, for player's damage UI on the enemy 
//    DamageObserver.Instance.ShowPlayerDamage(dmg, enemy.transform.position);
//}