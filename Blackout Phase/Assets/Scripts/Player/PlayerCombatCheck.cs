// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU
// Weijun

using UnityEngine;

public class PlayerCombatCheck : MonoBehaviour
{ 
    public static PlayerCombatCheck Instance { get; private set; } // copies static data 

    // just for testing
    [Header("Temp basic attk settings")]
    [SerializeField] private int basicAttkDamage; // player base damage
    [SerializeField] private int basicAttckRange; // B attk range
    [SerializeField] private int basicAttkAPcost; // ap cost for Battack
    [SerializeField] private int basicAttkENcost; // en cost for Battack
    [SerializeField] private int basicAttkHitRate; // base skill hitRate
    [SerializeField] private int basicAttkSkillCrit; // base skill crit chance
    [SerializeField] private int basicAttkCritDmg; // base attk crit dmg

    // ================== SkillData Settings ===========================
    [SerializeField] private SkillData SDbasicAttkSkill; // for access the skill data's data

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

        PlayerSetUp(); // void function to set up the player status
    }

    public void PlayerAttackCheck(EnemyInfo enemy)
    {
        // enemy/enemy tile not found get out
        if (enemy == null || enemy.currentTile == null) return;

        // if the current turn state is not player's acttion returns
        if (TurnManager.Instance.State != TurnState.PlayerAction) return;

        var player = CharacterInfo1.Instance; // make a copy of characterInfo as reference

        // if player is not found or player tile not found get out
        if (player == null || player.CurrentTile == null) return;

        // if player AP is not enough display message
        if (player.currentAP < basicAttkAPcost)
        {
            Debug.Log("Not enough AP to attack!");
            return;
        }

        TurnManager.Instance.PlayerSpendAP(basicAttkAPcost); // how much AP the attack cost

        //var pTile = CharacterInfo1.Instance.CurrentTile; // copies player's current tile

        //var eTile = enemy.currentTile != null ? enemy.currentTile : MapManager1.Instance.GetWorldTilePosition((Vector2)enemy.transform.position); // use world position to find the enemy tile

        //Debug.Log($"Player grid:{pTile.gridLocation} world:{pTile.transform.position} name:{pTile.name}"); // debug msg

        //Debug.Log($"Enemy grid:{eTile.gridLocation} world:{eTile.transform.position} name:{eTile.name}"); // debug msg

        //Debug.Log($"Player object world position:{CharacterInfo1.Instance.transform.position}"); // debug msg

        //Debug.Log($"Enemy object world position:{enemy.transform.position}"); // debug msg


        OverlayTile1 playerTile = CharacterInfo1.Instance.CurrentTile; // player tile location

        OverlayTile1 enemyTile = enemy.currentTile; //MapManager1.Instance.GetWorldTileFromTransform(enemy.transform); // enemy tile location

        //var e = enemyTile.gridLocation; // enemy tile location

        // if the enemy tile is empty get out
        if (enemyTile == null || playerTile == null)
        {
            Debug.Log("Player/Enemy Tile not found!"); // debug msg
            return;
        }

        // get the dist between player/enemy for range check
        //int distance = Mathf.Abs(p.x - e.x) + Mathf.Abs(p.y - e.y); // do math to get the correct manhattan methond to get attk range

        int distance = Manhattan(playerTile.gridLocation, enemyTile.gridLocation); //Mathf.Abs(playerTile.gridLocation.x - enemyTile.gridLocation.x) + Mathf.Abs(playerTile.gridLocation.y - enemyTile.gridLocation.y);

        Debug.Log($"Distance = {distance}, PlayerAttkRange:{basicAttckRange} (PlayerBaseRange:{playerInfo.BaseRange})");

        // check to see if enemy is in range for attk
        if (distance > basicAttckRange)
        {
            Debug.Log("Enemy out of reach!");
            return;
        }

        // check if the player has enough EN
        if (!playerInfo.PlayerEnCheck(basicAttkENcost))
        {
            Debug.Log("Insufficent EN amount!"); // debug msg
            return;
        }

        CharacterInfo1.Instance.PlayerSpendEN(basicAttkENcost); // EN goes down 

        int hitChance = HitRollCheck.FinalHitChanceCal(playerInfo.BaseHitRate, basicAttkHitRate, enemy.EvasionRate); // pass over the data to roll a hit

        Debug.Log($"HitChance:{hitChance}"); // debug msg

        // check for hit roll
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log("Attack MISS!"); // debug msg

            TurnManager.Instance.PlayerSpendAP(basicAttkAPcost); // still - ap

            return;
        }

        int dmg = basicAttkDamage; // for the final attack crit hit

        int critChance = HitRollCheck.FinalCritChanceCal(playerInfo.BaseCriticalRate, SDbasicAttkSkill.AttkCritChance); // find the crit chance of attack

        // check for crit roll
        if (PlayerCritOrFuryActiveCheck(critChance))
        {
            dmg = HitRollCheck.CritHit(dmg, basicAttkCritDmg); // crit dmg calculation

            // if player in Fury mode display crit in Fury mode
            if (PlayerFuryMode.Instance.inFuryMode)
                Debug.Log("In Fury Mode Attacker Critital Hit");

            else
                Debug.Log($"Counter Critical Hit: Damage:{dmg}"); // debug msg
        }

        enemy.EnemyTakeDamage(dmg); // calls the dmamge founction pass the amount

        // Added by Warren, for player's damage UI on the enemy
        DamageObserver.Instance.ShowPlayerDamage(dmg, enemy.transform.position);
    }

    public void PlayerCounterAttack(EnemyInfo enemy)
    {
        // enemy not found get out
        if (enemy == null) return;

        var player = CharacterInfo1.Instance; // make a copy of characterInfo as reference

        // if player is not found or player tile not found get out
        if (player == null || player.CurrentTile == null) return;

        // check if enemy still has health left
        if (enemy.health <= 0) return;

        int hitChance = HitRollCheck.FinalHitChanceCal(playerInfo.BaseHitRate, basicAttkHitRate, enemy.EvasionRate); // pass over the data to roll a hit

        Debug.Log($"HitChance:{hitChance}"); // debug msg

        // check if the player has enough EN
        if (!playerInfo.PlayerEnCheck(basicAttkENcost))
        {
            Debug.Log("Insufficent EN amount!"); // debug msg
            return;
        }

        CharacterInfo1.Instance.PlayerSpendEN(basicAttkENcost); // EN goes down  

        // check for hit roll
        if (!HitRollCheck.HitRollPercent(hitChance))
        {
            Debug.Log("Counter Attack MISS!"); // debug msg

            //TurnManager.Instance.PlayerSpendAP(basicAttkAPcost); // still - ap
            return;
        }

        int dmg = basicAttkDamage; // for the final attack crit hit

        int critChance = HitRollCheck.FinalCritChanceCal(playerInfo.BaseCriticalRate, SDbasicAttkSkill.AttkCritChance); // find the crit chance of attack

        // check for crit roll
        if (PlayerCritOrFuryActiveCheck(critChance))
        {
            dmg = HitRollCheck.CritHit(dmg, basicAttkCritDmg); // crit dmg calculation

            // if player in Fury mode display crit in Fury mode
            if (PlayerFuryMode.Instance.inFuryMode)
                Debug.Log("In Fury Mode Attacker Critital Hit");

            else
                Debug.Log($"Counter Critical Hit: Damage:{dmg}"); // debug msg
        }

        enemy.EnemyTakeDamage(dmg); // calls the dmamge founction pass the amount

        // Added by Warren, for player's damage UI on the enemy 
        DamageObserver.Instance.ShowPlayerDamage(dmg, enemy.transform.position);
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
            PlayerStatusSetUp(); // set up player status
        }

        // ======= if playerInfo still null ========
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player1"); // find it using game object tag

        // if found set up the basic stats for player1
        if (playerObj != null)
        {
            playerInfo = playerObj.GetComponent<CharacterInfo1>(); // set up the playerInfo using object

            PlayerStatusSetUp(); // calls the player stats function
        }
    }

    private void PlayerStatusSetUp()
    {
        basicAttkDamage = playerInfo.BaseAttk; // basic attk stats

        basicAttckRange = playerInfo.BaseRange; // basic attk range

        basicAttkDamage = playerInfo.BaseCritDamage; // basic crit dmg

        basicAttkAPcost = SDbasicAttkSkill.AttkAPCost; // basic Attk AP cost

        basicAttkENcost = SDbasicAttkSkill.AttkENCost; // basic attk EN cost

        basicAttkHitRate = SDbasicAttkSkill.AttkHitRate; // basic attk hit rate

        basicAttkSkillCrit = SDbasicAttkSkill.AttkCritChance; // basic attk crit add on
    }

    private int Manhattan(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // returns the player/enemy distance
    }
}



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