// tutorial enemy spawner from EnemySpawner.cs
// has a lot of functionality from enemy movement and stats so we don't edit the prefab
// -Ellison
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialEnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public SpriteRenderer spriteRenderer;

    [Header("Enemy set up")]
    [SerializeField] private GameObject enemyPrefab; // enemy prefab
    [SerializeField] private EnemyStatsScripObj enemyStats; // enemy stats
    [SerializeField] private Vector2Int spawnGridPosition;// where it starts

    public EnemyInfo enemyInfo; // set up accessor

    [Header("Patrol points")]
    [SerializeField] private List<Vector2Int> patrolPoints = new List<Vector2Int>(); // array list of enemy patrol points

    private EnemyMovement movement; // access enemy movement

    [Header("Scripted events")]
    [SerializeField] public Vector2Int spot1TilePosition;
    [SerializeField] public Vector2Int spot2TilePosition;
    [SerializeField] public Vector2Int spot3TilePosition;
    [SerializeField] public Vector2Int spot4TilePosition;

    public bool spot1Triggered = false; // flag for the first spot
    public bool finalSpotTriggered = false;

    public int health = 10;
    public bool isAlive = true;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MapManager1.Instance != null &&
            MapManager1.Instance.map != null &&
            MapManager1.Instance.map.Count > 0); // wait until the map is fully setup before anything else

        SpawnAfterMapReady(); // delay call 
    }

    private void SpawnAfterMapReady()
    {
        // wait until map is spawned and the map count > 0
        //yield return new WaitUntil(() => MapManager.Instance.map != null && MapManager.Instance.map.Count > 0);

        OverlayTile1 tile = MapManager1.Instance.GetTile(spawnGridPosition); // get the spawn tile

        if (tile == null)
        {
            Debug.LogError($"Spawn failed No tile found at {spawnGridPosition}"); // nothing found
            return; // get out
        }

        enemy = Instantiate(enemyPrefab, tile.transform.position, Quaternion.identity); // setup the enemy throgh prefab
        spriteRenderer = enemy.GetComponent<SpriteRenderer>();
        enemyInfo = enemy.GetComponentInChildren<EnemyInfo>(); // set up the info even the child object

        // get the movement component for the enemy
        movement = enemy.GetComponent<EnemyMovement>();

        // check to see if enemyInfo exist
        if (enemyInfo == null)
        {
            Debug.Log("EnemyInfo not found!"); // debug msg
            return;
        }

        enemyInfo.SetStats(enemyStats); // set up the enemy stats

        enemyInfo.ResetHPToMAX(); // set enemy HP back to max before spawn

        EnemyController1 enemyController = enemy.GetComponent<EnemyController1>(); // controlls enemy

        enemyController.SetPatrolPoints(patrolPoints, 0); // set up the patrol points for enemy

        // enemyInfo or control not found displays debug and get out
        if (enemyInfo == null || enemyController == null)
        {
            Debug.LogError("Enemy Prefab is missing or Control is missing!"); //debug
            return;
        }

        enemyInfo.EnemySetTile(tile); // set up the tile

        tile.hasEnemy = true; // triggers the nemey flag

        enemy.transform.position = new Vector3(tile.transform.position.x,
            tile.transform.position.y + 0.01f, // y is offset by a little
            tile.transform.position.z);

        //enemyController.SetInitialized(); // flag set to true

        //Debug.Log($"[Spawner] set curretnTIle for {enemy.name} to {enemyInfo.currentTile.gridLocation}"); //debug

        //enemyInfo.currentTile.gridLocation = new Vector3Int(enemyInfo.currentTile.gridLocation.x, // keeps x,y but not z
        //    enemyInfo.currentTile.gridLocation.y, 0);

        //enemyController.transform.position = new Vector3(enemyController.transform.position.x, // grid logic fix so it uses correct z
        //    enemyController.transform.position.y, tile.transform.position.z);

        //TurnManager.Instance.RegisterEnemy(enemyController); // send over the enemy control 

        Debug.Log($"Enemy spawned at " + tile.gridLocation); // debug
    }

    public Vector2Int GetCurrentMilestoneTarget()
    {
        if (!spot1Triggered)
            return spot1TilePosition;

        // all milestones reached, no valid target
        return Vector2Int.zero;
    }

    public void TriggerOneTileMove(Vector2Int spotToMove)
    {
        if (movement != null)
        {
            OverlayTile1 targetTile = MapManager1.Instance.GetTile(spotToMove);
            if (targetTile != null)
            {
                StartCoroutine(movement.MoveAlong(new List<OverlayTile1> { targetTile }));
                spot1Triggered = true; // set the flag to true after triggering the move
            }
            else
            {
                Debug.LogError($"No tile found at {spotToMove} for enemy move.");
            }
        }
        else
        {
            Debug.LogError("EnemyMovement component not found on the enemy.");
        }
    }

    public void TriggerPathMove()
    {
        OverlayTile1 target1 = MapManager1.Instance.GetTile(spot1TilePosition);
        OverlayTile1 target2 = MapManager1.Instance.GetTile(spot2TilePosition);
        OverlayTile1 target3 = MapManager1.Instance.GetTile(spot3TilePosition);
        OverlayTile1 target4 = MapManager1.Instance.GetTile(spot4TilePosition);

        StartCoroutine(movement.MoveAlong(new List<OverlayTile1> { target1, target2, target3, target4 }));

        finalSpotTriggered = true;
    }

    public void loseHealth(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            isAlive = false;
            spriteRenderer.enabled = false;
        }
    }
}
