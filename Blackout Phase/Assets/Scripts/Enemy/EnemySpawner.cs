// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// for prefab enemy spawn positions, pick a vaild x,y on the map and spawn enemies acorrdingly
// since the enemy has a spawn point I used the same starting point in patrol points so it skips to
// the first array for patrol instead of spawn position path[0] -> path[1], goes like this path[1] -> path[2]
// Weijun

using UnityEngine; // default
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic; // for the array list we have also IEnumerator for delay funciton calls yield returns. loading map first then do something else

public class EnemySpwawan : MonoBehaviour
{
    [Header("Enemy set up")]
    [SerializeField] private GameObject enemyPrefab; // enemy prefab
    [SerializeField] private EnemyStatsScripObj enemyStats; // enemy stats
    [SerializeField] private Vector2Int spawnGridPosition;// where it starts

    private EnemyInfo enemyInfo; // set up accessor

    [Header("Patrol points")]
    [SerializeField] private List<Vector2Int> patrolPoints = new List<Vector2Int>(); // array list of enemy patrol points

    //private EnemyInfo enemyInfo; // accessor

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

        GameObject enemy = Instantiate(enemyPrefab, tile.transform.position, Quaternion.identity); // setup the enemy throgh prefab

        enemyInfo = enemy.GetComponentInChildren<EnemyInfo>(); // set up the info even the child object

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

        TurnManager.Instance.RegisterEnemy(enemyController); // send over the enemy control 

        Debug.Log($"Enemy spawned at " + tile.gridLocation); // debug
    }
}
