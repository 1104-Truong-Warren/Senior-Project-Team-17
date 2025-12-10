using Unity.VisualScripting;
using UnityEngine;

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
