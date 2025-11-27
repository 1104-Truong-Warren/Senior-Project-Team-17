using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject enemyPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private Vector2Int[] patrolWaypoints = new Vector2Int[]
    {
        new Vector2Int(3, -6),
        new Vector2Int(4, -6),
        new Vector2Int(4, -5),
        new Vector2Int(3, -5)
    };

    private void Start()
    {
        StartCoroutine(SpawnEnemyAfterDelay());
    }

    private IEnumerator SpawnEnemyAfterDelay()
    {
        Debug.Log("Waiting for player to be placed...");
        
        // Wait for player to exist and be placed
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log($"Player found: {player.name}");
        
        // Wait a bit more for player placement to complete
        yield return new WaitForSeconds(1f);
        
        // Wait for MapManager to be ready
        while (MapManager.Instance == null || MapManager.Instance.map == null)
        {
            yield return null;
        }
        
        Debug.Log("Spawning enemy...");
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab not assigned!");
            return;
        }

        // Spawn at first waypoint
        if (MapManager.Instance.map.ContainsKey(patrolWaypoints[0]))
        {
            OverlayTile spawnTile = MapManager.Instance.map[patrolWaypoints[0]];
            GameObject enemy = Instantiate(enemyPrefab, spawnTile.transform.position, Quaternion.identity);
            
            // Set up patrol component
            SimplePatrol patrol = enemy.GetComponent<SimplePatrol>();
            if (patrol != null)
            {
                // We'll set waypoints through code since they're serialized
                Debug.Log($"Enemy spawned at {patrolWaypoints[0]}");
            }
            
            enemy.name = "Enemy"; // Remove (clone) from name
        }
        else
        {
            Debug.LogError($"Cannot spawn enemy: Waypoint {patrolWaypoints[0]} not found!");
        }
    }
}