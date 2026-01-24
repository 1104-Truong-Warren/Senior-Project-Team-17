// used this video to see how other people make skills URL: https://www.youtube.com/watch?v=V4WrS-Wt2xU

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

        //PlayerSetUp(); // void function to set up the player status
    }

    public void PlayerAttackCheck(EnemyInfo enemy)
    {
        // enemy not found get out
        if (enemy == null) return;

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

        // check to see if enemy is on a tile
        if (enemy.currentTile == null)
        {
            Debug.Log("Enemy is not on any tiles!");
            return;
        }

        Vector3Int p = player.CurrentTile.gridLocation; // player tile location

        Vector3Int e = enemy.currentTile.gridLocation; // enemy tile location

        // get the dist between player/enemy for range check
        int distance = Mathf.Abs(p.x - e.x) + Mathf.Abs(p.y - e.y); // do math to get the correct manhattan methond to get attk range

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

        enemy.EnemyTakeDamage(basicAttkDamage); // calls the dmamge founction pass the amount

        TurnManager.Instance.PlayerSpendAP(basicAttkAPcost); // how much AP the attack cost
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

        basicAttkAPcost = SDbasicAttkSkill.AttkAPCost; // basic Attk AP cost

        basicAttkENcost = SDbasicAttkSkill.AttkENCost; // basic attk EN cost
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