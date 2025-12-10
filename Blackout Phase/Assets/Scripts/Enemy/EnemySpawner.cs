using UnityEngine;
using System.Collections;

public class EnemySpwawan : MonoBehaviour
{
    [Header("Enemy set up")]
    [SerializeField] GameObject enemyPrefab; // enemy prefab
    [SerializeField] Vector2Int spawnGridPosition;// where it starts

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

        EnemyInfo enemyInfo = enemy.GetComponent<EnemyInfo>(); // set up the info

        EnemyController1 enemyController = enemy.GetComponent<EnemyController1>(); // controlls enemy

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
