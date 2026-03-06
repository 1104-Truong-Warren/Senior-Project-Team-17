// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// this is similar to the characterInfo, but it was done by me the stats HP, attkRange,
// enemey damage, detectionRange, but the only useful thing currently are
// damage, attackRange, moveRange, and detectionRange , since player doesn's have an attk hp is kinda useless
// the moveRange is used for chasing range so it doesn't follow the player forever
// Weijun

using Unity.VisualScripting;
using UnityEngine; // default

public class EnemyInfo : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyStatsScripObj stats; // enemy stats
    //[SerializeField] private int Health; // enmey's health
    //[SerializeField] private int AttackRange; // enemy's attk range
    //[SerializeField] private int enemyDamage; // how much damage enemy does
    //[SerializeField] private int enemyDetectionRange; // how far is the enemy detection
    //[SerializeField] private int MoveRange; // enemy's moveRange
    //[SerializeField] private int evasionRate; // enemy's dodge rate
    //[SerializeField] private int enemyHitRate; // enemy base hit rate

    [SerializeField] OverlayTile1 Tile; // current tile enmey is on

    // public accessor
    public int CurrentHP { get; private set; } // enemy currentHp set up
    public int moveRange => stats.movementRange;// set move range

    public int attackRange => stats.attackRange; // attack range

    public int EnemyDmg => stats.damage; // get the enemy's dmg

    public int EnemyDetect => stats.detectionRange; // get the enemy's detection range

    // public int health => stats != null ? stats.maxHP; // hit points
    public int EvasionRate => stats.evasionRate; // get enemy evasion rate 
    public int EnemyHitRate => stats.hitRate; // enemy base hit rate   


    public OverlayTile1 currentTile => Tile; // where the enemy tile is

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
        if (Tile != null)
            Tile.hasEnemy = false;

        Tile = newtile; // set up the tile

        // after we set the tile, toggle flag to t
        if (Tile != null)
            Tile.hasEnemy = true;
    }

    public void SetStats(EnemyStatsScripObj newStats)
    {
        // check to make sure newStats is not null
        if (newStats == null)
            throw new System.ArgumentNullException(nameof(newStats), $"{name}: NULL newStats Check enemySpawner"); // debug msg, throw null exception to display error messge instead of crashed

        stats = newStats; // helper function to set the stats for enemy

        //enabled = true; // reenable if passed the null test

        CurrentHP = stats.maxHP; // HP set up
    }

    public void ResetHPToMAX()
    {
        CurrentHP = stats.maxHP; // set hp to max HP

        Debug.LogWarning($"Enemy currentHP:{CurrentHP}"); // debug msg
    }

    public void EnemyTakeDamage(int dmg)
    {
        CurrentHP -= dmg; // total heal - dmg

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;

            // reset the tile to empty
            if (Tile != null)
                Tile.hasEnemy = false;

            Debug.Log($"{name} has died.");

            Destroy(this.gameObject);  // destory object enemy
        }
    }
}

//public OverlayTile EnemySetTile() => currentTile;

//private void Start()
//{
//    Debug.Log($"EnemyInfo currentTile = {currentTile}");
//    if (currentTile != null)
//        Debug.Log($"Enemy starts at grid {currentTile.gridLocation}");
//}
