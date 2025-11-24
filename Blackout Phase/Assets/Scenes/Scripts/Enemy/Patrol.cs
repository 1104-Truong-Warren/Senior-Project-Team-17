using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    private List<Vector2Int> waypoints;
    private int currentWaypointIndex = 0;
    private bool isPatrolling = false;
    private float patrolSpeed;
    
    private CharacterInfo characterInfo;
    private PathFinder pathFinder;
    private List<OverlayTile> currentPath;
    private int currentPathIndex = 0;

    public void Initialize(List<Vector2Int> patrolWaypoints, float speed)
    {
        waypoints = patrolWaypoints;
        patrolSpeed = speed;
        characterInfo = GetComponent<CharacterInfo>();
        pathFinder = new PathFinder();
        
        // AUTO-PLACE ENEMY ON FIRST WAYPOINT IF NOT ALREADY PLACED
        AutoPlaceEnemy();
    }

    // NEW METHOD: Auto-place enemy on first waypoint
    private void AutoPlaceEnemy()
    {
        if (characterInfo.standingOnTile == null && waypoints != null && waypoints.Count > 0)
        {
            Vector2Int spawnCoord = waypoints[0];
            if (MapManager.Instance != null && MapManager.Instance.map.ContainsKey(spawnCoord))
            {
                OverlayTile spawnTile = MapManager.Instance.map[spawnCoord];
                PositionCharacterOnTile(spawnTile);
                Debug.Log($"Enemy auto-placed at grid position: {spawnCoord}");
            }
            else
            {
                Debug.LogWarning($"Cannot auto-place enemy: No tile found at {spawnCoord}");
            }
        }
    }

    // NEW METHOD: Position character on tile
    private void PositionCharacterOnTile(OverlayTile tile)
    {
        if (characterInfo == null) return;
        
        // Position the enemy on the tile (slightly offset for visibility)
        transform.position = new Vector3(
            tile.transform.position.x,
            tile.transform.position.y + 0.0001f,
            tile.transform.position.z
        );
        
        // Set sorting order to match the tile
        SpriteRenderer enemyRenderer = GetComponent<SpriteRenderer>();
        if (enemyRenderer != null)
        {
            enemyRenderer.sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
        
        // Update character info
        characterInfo.standingOnTile = tile;
    }

    public void StartPatrol()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning("No patrol waypoints defined for " + gameObject.name);
            return;
        }
        
        // Ensure enemy is placed before starting patrol
        if (characterInfo.standingOnTile == null)
        {
            AutoPlaceEnemy();
        }
        
        isPatrolling = true;
        currentWaypointIndex = 0;
        CalculatePatrolPath();
    }

    public void StopPatrol()
    {
        isPatrolling = false;
        currentPath = null;
    }

    public void UpdatePatrol()
    {
        if (!isPatrolling || currentPath == null || currentPath.Count == 0)
            return;

        if (currentPathIndex < currentPath.Count)
        {
            MoveToNextTile();
        }
        else
        {
            // Reached current waypoint, get next one
            GetNextWaypoint();
            CalculatePatrolPath();
        }
    }

    public void GetNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Count)
        {
            currentWaypointIndex = 0; // Loop back to start
        }
        
        currentPathIndex = 0;
    }

    public void CalculatePatrolPath()
    {
        if (characterInfo == null || characterInfo.standingOnTile == null)
        {
            Debug.LogWarning("Cannot calculate path: Character not placed on tile");
            return;
        }

        Vector2Int targetWaypoint = waypoints[currentWaypointIndex];
        
        if (MapManager.Instance.map.ContainsKey(targetWaypoint))
        {
            OverlayTile targetTile = MapManager.Instance.map[targetWaypoint];
            currentPath = pathFinder.FindPath(characterInfo.standingOnTile, targetTile);
            currentPathIndex = 0;
            
            if (currentPath.Count == 0)
            {
                Debug.LogWarning("No path found to waypoint: " + targetWaypoint);
                GetNextWaypoint(); // Skip to next waypoint if no path
            }
            else
            {
                Debug.Log($"Path calculated with {currentPath.Count} tiles to waypoint {targetWaypoint}");
            }
        }
        else
        {
            Debug.LogWarning($"Waypoint {targetWaypoint} not found in map");
        }
    }

    private void MoveToNextTile()
    {
        if (currentPathIndex >= currentPath.Count)
            return;

        OverlayTile targetTile = currentPath[currentPathIndex];
        Vector2 targetPosition = targetTile.transform.position;
        float step = patrolSpeed * Time.deltaTime;
        
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
        
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            characterInfo.standingOnTile = targetTile;
            currentPathIndex++;
            Debug.Log($"Enemy moved to tile {currentPathIndex}/{currentPath.Count}");
        }
    }

    // Helper method to add waypoints at runtime
    public void AddWaypoint(Vector2Int waypoint)
    {
        if (waypoints == null)
            waypoints = new List<Vector2Int>();
            
        waypoints.Add(waypoint);
    }

    // Helper method to clear waypoints
    public void ClearWaypoints()
    {
        if (waypoints != null)
            waypoints.Clear();
    }
}