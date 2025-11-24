using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Searching,
        Idle
    }

    [Header("Enemy Configuration")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolSpeed = 2f;
    
    [Header("Patrol Settings")]
    [SerializeField] private List<Vector2Int> patrolWaypoints = new List<Vector2Int>();

    private EnemyState currentState = EnemyState.Patrolling;
    private Patrol patrolBehavior;
    private CharacterInfo characterInfo;
    private GameObject player;
    
    // Visual indicators for debugging
    [SerializeField] private SpriteRenderer stateIndicator;

    private void Start()
    {
        characterInfo = GetComponent<CharacterInfo>();
        patrolBehavior = GetComponent<Patrol>();
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Initialize patrol behavior with waypoints
        if (patrolBehavior != null && patrolWaypoints.Count > 0)
        {
            patrolBehavior.Initialize(patrolWaypoints, patrolSpeed);
            patrolBehavior.StartPatrol();
        }
        
        UpdateStateIndicator();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                UpdatePatrolState();
                break;
            case EnemyState.Chasing:
                UpdateChaseState();
                break;
            case EnemyState.Searching:
                UpdateSearchState();
                break;
            case EnemyState.Idle:
                UpdateIdleState();
                break;
        }
    }

    private void UpdatePatrolState()
    {
        // Check for player detection
        if (IsPlayerInDetectionRange() && HasLineOfSightToPlayer())
        {
            ChangeState(EnemyState.Chasing);
            return;
        }
        
        // Continue patrolling
        patrolBehavior.UpdatePatrol();
    }

    private void UpdateChaseState()
    {
        if (!IsPlayerInDetectionRange() || !HasLineOfSightToPlayer())
        {
            ChangeState(EnemyState.Searching);
            return;
        }
        
        // Chase the player
        ChasePlayer();
    }

    private void UpdateSearchState()
    {
        // Implement search behavior (look around, move to last known position)
        // After searching time, return to patrolling
        ChangeState(EnemyState.Patrolling);
    }

    private void UpdateIdleState()
    {
        // Do nothing or look around
    }

    private bool IsPlayerInDetectionRange()
    {
        if (player == null) return false;
        
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= detectionRange;
    }

    private bool HasLineOfSightToPlayer()
    {
        if (player == null) return false;
        
        Vector2 direction = player.transform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange);
        
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        
        return false;
    }

    private void ChasePlayer()
    {
        // Get player's current tile
        CharacterInfo playerInfo = player.GetComponent<CharacterInfo>();
        if (playerInfo != null && playerInfo.standingOnTile != null)
        {
            // Calculate path to player
            PathFinder pathFinder = new PathFinder();
            List<OverlayTile> path = pathFinder.FindPath(characterInfo.standingOnTile, playerInfo.standingOnTile);
            
            // Move along path with chase speed
            if (path.Count > 0)
            {
                MoveToTile(path[0], chaseSpeed);
            }
        }
    }

    private void MoveToTile(OverlayTile targetTile, float moveSpeed)
    {
        // Similar to MouseController's MoveAlongPath but simplified for single tile movement
        Vector2 targetPosition = targetTile.transform.position;
        float step = moveSpeed * Time.deltaTime;
        
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
        
        // Update standing tile when close enough
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            characterInfo.standingOnTile = targetTile;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        OnStateChanged(newState);
        UpdateStateIndicator();
    }

    private void OnStateChanged(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Patrolling:
                patrolBehavior.StartPatrol();
                break;
            case EnemyState.Chasing:
                patrolBehavior.StopPatrol();
                break;
            case EnemyState.Searching:
                patrolBehavior.StopPatrol();
                // Start search coroutine
                break;
        }
    }

    private void UpdateStateIndicator()
    {
        if (stateIndicator != null)
        {
            switch (currentState)
            {
                case EnemyState.Patrolling:
                    stateIndicator.color = Color.green;
                    break;
                case EnemyState.Chasing:
                    stateIndicator.color = Color.red;
                    break;
                case EnemyState.Searching:
                    stateIndicator.color = Color.yellow;
                    break;
                case EnemyState.Idle:
                    stateIndicator.color = Color.blue;
                    break;
            }
        }
    }

    // For debugging in Inspector
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw patrol path
        Gizmos.color = Color.green;
        for (int i = 0; i < patrolWaypoints.Count; i++)
        {
            Vector3 worldPos = MapManager.Instance.map[patrolWaypoints[i]].transform.position;
            Gizmos.DrawWireCube(worldPos, Vector3.one * 0.3f);
            if (i < patrolWaypoints.Count - 1)
            {
                Vector3 nextWorldPos = MapManager.Instance.map[patrolWaypoints[i + 1]].transform.position;
                Gizmos.DrawLine(worldPos, nextWorldPos);
            }
        }
    }
}