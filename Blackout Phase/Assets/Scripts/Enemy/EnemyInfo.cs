// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// this is similar to the characterInfo, but it was done by me the stats HP, attkRange,
// enemey damage, detectionRange, but the only useful thing currently are
// damage, attackRange, moveRange, and detectionRange , since player doesn's have an attk hp is kinda useless
// the moveRange is used for chasing range so it doesn't follow the player forever
// Weijun

using Unity.VisualScripting;
using UnityEngine; // default

public class EnemyInfo : UnitCore
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStatsScripObj stats; // enemy stats
    [SerializeField] private int currentHP; // current hp for enemy
    //[SerializeField] private int Health; // enmey's health
    //[SerializeField] private int AttackRange; // enemy's attk range
    //[SerializeField] private int enemyDamage; // how much damage enemy does
    //[SerializeField] private int enemyDetectionRange; // how far is the enemy detection
    //[SerializeField] private int MoveRange; // enemy's moveRange
    //[SerializeField] private int evasionRate; // enemy's dodge rate
    //[SerializeField] private int enemyHitRate; // enemy base hit rate

    [SerializeField] private OverlayTile1 tile; // current tile enmey is on

    // public accessor
    public override int CurrentHP => currentHP; //{ get; protected set; } // enemy currentHp set up
    public override int MaxHP => stats != null ? stats.maxHP : 0; // for the abstract
    public int moveRange => stats.movementRange;// set move range
    public override int AttackRange => stats != null ? stats.attackRange : 0; // attack range
    public override int BaseAttack => stats.baseAttack; // get the enemy's dmg
    public int EnemyDetect => stats.detectionRange; // get the enemy's detection range

    // public int health => stats != null ? stats.maxHP; // hit points
    public override int EvasionRate => stats != null ? stats.evasionRate : 0; // get enemy evasion rate 
    public override int HitRate => stats != null ? stats.hitRate : 0; // enemy base hit rate   
    public override OverlayTile1 CurrentTile => tile; // where the enemy tile is

    private void Awake()
    {
        // if the stats is not found display a msg
        if (stats == null)
            Debug.LogWarning($"{name}: Enemy Stats Scriptable Object Not Assigned!"); // debug msg

        //    enabled = false; // disable the copy function so it can't be access
        //    return;
    }

    public void EnemySetTile(OverlayTile1 newtile)
    {
        // tile exsit, flag is f, before we set enemy, nothing
        if (tile != null)
            tile.hasEnemy = false;

        tile = newtile; // set up the tile

        // after we set the tile, toggle flag to t
        if (tile != null)
            tile.hasEnemy = true;
    }

    public void SetStats(EnemyStatsScripObj newStats)
    {
        // check to make sure newStats is not null
        if (newStats == null)
            throw new System.ArgumentNullException(nameof(newStats), $"{name}: NULL newStats Check enemySpawner"); // debug msg, throw null exception to display error messge instead of crashed

        stats = newStats; // helper function to set the stats for enemy

        //enabled = true; // reenable if passed the null test

        currentHP = stats.maxHP; // HP set up
    }

    public void ResetHPToMAX()
    {
        currentHP = stats.maxHP; // set hp to max HP

        Debug.LogWarning($"Enemy currentHP:{CurrentHP}"); // debug msg
    }

    public void EnemyTakeDamage(int dmg)
    {
        currentHP -= dmg; // total heal - dmg

        if (CurrentHP <= 0)
        {
            currentHP = 0;

            // reset the tile to empty
            if (tile != null)
                tile.hasEnemy = false;

            Debug.Log($"{name} has died.");

            Destroy(this.gameObject);  // destory object enemy
        }
    }

    // abstract override 
    public override void TakeDamage(int dmg)
    {
        EnemyTakeDamage(dmg);
    }
}

//public OverlayTile EnemySetTile() => currentTile;

//private void Start()
//{
//    Debug.Log($"EnemyInfo currentTile = {currentTile}");
//    if (currentTile != null)
//        Debug.Log($"Enemy starts at grid {currentTile.gridLocation}");
//}
