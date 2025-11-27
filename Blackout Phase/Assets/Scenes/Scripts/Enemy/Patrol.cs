using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private List<Vector2Int> waypoints = new List<Vector2Int>();
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTimeAtWaypoint = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private CharacterInfo characterInfo;
    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private OverlayTile targetTile;

    private void Start()
    {
        characterInfo = GetComponent<CharacterInfo>();
        StartCoroutine(WaitForMapThenStart());
    }

    private IEnumerator WaitForMapThenStart()
    {
        // Wait for MapManager to be ready
        while (MapManager.Instance == null || MapManager.Instance.map == null)
        {
            yield return null;
        }

        if (showDebug) Debug.Log("Map ready! Starting patrol...");
        
        // Auto-place on first waypoint if not placed
        if (characterInfo.standingOnTile == null && waypoints.Count > 0)
        {
            AutoPlaceOnFirstWaypoint();
        }

        // Start patrolling
        if (waypoints.Count > 1)
        {
            MoveToNextWaypoint();
        }
        else
        {
            Debug.LogWarning("Need at least 2 waypoints for patrol");
        }
    }

    private void AutoPlaceOnFirstWaypoint()
    {
        Vector2Int firstWaypoint = waypoints[0];
        if (MapManager.Instance.map.ContainsKey(firstWaypoint))
        {
            OverlayTile tile = MapManager.Instance.map[firstWaypoint];
            transform.position = tile.transform.position;
            characterInfo.standingOnTile = tile;
            if (showDebug) Debug.Log($"Enemy auto-placed at {firstWaypoint}");
        }
        else
        {
            Debug.LogError($"Cannot place enemy: Waypoint {firstWaypoint} not found in map!");
            // Try to find any valid tile as fallback
            FindFallbackPosition();
        }
    }

    private void FindFallbackPosition()
    {
        foreach (var tile in MapManager.Instance.map.Values)
        {
            if (!tile.isBlocked)
            {
                transform.position = tile.transform.position;
                characterInfo.standingOnTile = tile;
                Debug.Log($"Enemy fallback-placed at {tile.gridLocation}");
                break;
            }
        }
    }

    private void MoveToNextWaypoint()
    {
        if (waypoints.Count < 2) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        Vector2Int targetCoord = waypoints[currentWaypointIndex];

        if (MapManager.Instance.map.ContainsKey(targetCoord))
        {
            targetTile = MapManager.Instance.map[targetCoord];
            isMoving = true;
            if (showDebug) Debug.Log($"Moving to waypoint {currentWaypointIndex}: {targetCoord}");
        }
        else
        {
            Debug.LogError($"Waypoint {targetCoord} not found! Skipping...");
            // Skip to next waypoint
            StartCoroutine(WaitAndMoveNext());
        }
    }

    private void Update()
    {
        if (isMoving && targetTile != null)
        {
            // Simple direct movement toward target tile
            Vector2 targetPosition = targetTile.transform.position;
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);

            // Check if reached target
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                transform.position = targetPosition;
                characterInfo.standingOnTile = targetTile;
                isMoving = false;
                if (showDebug) Debug.Log($"Reached waypoint {currentWaypointIndex}");

                // Wait, then move to next waypoint
                StartCoroutine(WaitAndMoveNext());
            }
        }
    }

    private IEnumerator WaitAndMoveNext()
    {
        yield return new WaitForSeconds(waitTimeAtWaypoint);
        MoveToNextWaypoint();
    }

    [ContextMenu("Add Current Position as Waypoint")]
    private void AddCurrentPositionAsWaypoint()
    {
        if (characterInfo != null && characterInfo.standingOnTile != null)
        {
            Vector2Int currentCoord = new Vector2Int(
                characterInfo.standingOnTile.gridLocation.x,
                characterInfo.standingOnTile.gridLocation.y
            );
            waypoints.Add(currentCoord);
            Debug.Log($"Added waypoint: {currentCoord}");
        }
    }

    [ContextMenu("Print Current Position")]
    private void PrintCurrentPosition()
    {
        if (characterInfo != null && characterInfo.standingOnTile != null)
        {
            Debug.Log($"Current position: {characterInfo.standingOnTile.gridLocation}");
        }
        else
        {
            Debug.Log("Not on a tile or character info missing");
        }
    }
}