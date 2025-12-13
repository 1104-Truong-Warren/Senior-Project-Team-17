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
    [SerializeField] private int Health; // enmey's health
    [SerializeField] private int AttackRange; // enemy's attk range
    [SerializeField] private int enemyDamage; // how much damage enemy does
    [SerializeField] private int enemyDetectionRange; // how far is the enemy detection
    [SerializeField] private int MoveRange; // enemy's moveRange


    [SerializeField] OverlayTile1 Tile; // current tile enmey is on


    // public accessor
    public int moveRange => MoveRange;// set move range

    public int attackRange => AttackRange; // attack range

    public int EnemyDmg => enemyDamage; // get the enemy's dmg

    public int EnemyDetect => enemyDetectionRange; // get the enemy's detection range

    public int health => Health; // hit points

    public OverlayTile1 currentTile => Tile; // where the enemy tile is

    //public OverlayTile EnemySetTile() => currentTile;

    //private void Start()
    //{
    //    Debug.Log($"EnemyInfo currentTile = {currentTile}");
    //    if (currentTile != null)
    //        Debug.Log($"Enemy starts at grid {currentTile.gridLocation}");
    //}

    public void EnemySetTile(OverlayTile1 newtile)
    {
        Tile = newtile; // set up the tile
    }

    public void EnemyTakeDamage(int dmg)
    {
        Health -= dmg; // total heal - dmg

        if (Health <= 0)
        {
            Health = 0;

            Debug.Log($"{name} has died.");
            
            Destroy(this.gameObject);  // destory object enemy
        }
    }
}
