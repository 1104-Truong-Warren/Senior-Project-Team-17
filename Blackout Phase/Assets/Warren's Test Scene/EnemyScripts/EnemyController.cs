using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Vector2Int[] patrolPoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;
    
    [Header("Detection Settings")]
    [SerializeField] private int detectionRange = 3;
    [SerializeField] private int attackRange = 1;
    [SerializeField] private int attackDamage = 20;
    
    private Pathfinding pathfinding;
    private HealthSystem healthSystem;
    private Transform player;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    
    public void Initialize(Pathfinding pf, Transform playerTransform)
    {
        pathfinding = pf;
        player = playerTransform;
        healthSystem = GetComponent<HealthSystem>();
        
        // Start patrolling
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            StartCoroutine(PatrolRoutine());
        }
    }
    
    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (isChasing)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            
            Vector2Int targetPoint = patrolPoints[currentPatrolIndex];
            yield return StartCoroutine(MoveToPosition(targetPoint));
            
            yield return new WaitForSeconds(waitTime);
            
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }
    
    private IEnumerator MoveToPosition(Vector2Int targetGridPos)
    {
        pathfinding.GetGrid().GetXY(transform.position, out int currentX, out int currentY);
        List<PathNode> path = pathfinding.FindPath(currentX, currentY, targetGridPos.x, targetGridPos.y);
        
        if (path != null && path.Count > 0)
        {
            foreach (PathNode node in path)
            {
                Vector3 worldPos = pathfinding.GetGrid().GetWorldPosition(node.x, node.y);
                worldPos += new Vector3(1f, 1f, 0); // Center in cell
                
                while (Vector3.Distance(transform.position, worldPos) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, worldPos, moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }
    }
    
    private void Update()
    {
        if (player == null) return;
        
        CheckForPlayer();
    }
    
    private void CheckForPlayer()
    {
        pathfinding.GetGrid().GetXY(transform.position, out int enemyX, out int enemyY);
        pathfinding.GetGrid().GetXY(player.position, out int playerX, out int playerY);
        
        int distance = Mathf.Abs(enemyX - playerX) + Mathf.Abs(enemyY - playerY);
        
        if (distance <= detectionRange)
        {
            if (!isChasing)
            {
                Debug.Log("Enemy detected player! Starting chase!");
                isChasing = true;
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }
        }
        else if (isChasing)
        {
            Debug.Log("Player escaped! Returning to patrol.");
            isChasing = false;
            StopAllCoroutines();
            StartCoroutine(PatrolRoutine());
        }
    }
    
    private IEnumerator ChasePlayer()
    {
        while (isChasing && player != null)
        {
            pathfinding.GetGrid().GetXY(player.position, out int playerX, out int playerY);
            pathfinding.GetGrid().GetXY(transform.position, out int enemyX, out int enemyY);
            
            int distance = Mathf.Abs(enemyX - playerX) + Mathf.Abs(enemyY - playerY);
            
            if (distance <= attackRange)
            {
                // Attack player
                HealthSystem playerHealth = player.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log("Enemy attacked player!");
                }
                yield return new WaitForSeconds(1f); // Attack cooldown
            }
            else
            {
                // Move toward player
                yield return StartCoroutine(MoveToPosition(new Vector2Int(playerX, playerY)));
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    // Visualize patrol and detection in editor
    private void OnDrawGizmosSelected()
    {
        // Draw patrol points
        if (patrolPoints != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Vector3 worldPos = new Vector3(patrolPoints[i].x * 2f + 1f, patrolPoints[i].y * 2f + 1f, 0);
                Gizmos.DrawWireCube(worldPos, new Vector3(1.8f, 1.8f, 0.1f));
                
                if (i < patrolPoints.Length - 1)
                {
                    Vector3 nextPos = new Vector3(patrolPoints[i + 1].x * 2f + 1f, patrolPoints[i + 1].y * 2f + 1f, 0);
                    Gizmos.DrawLine(worldPos, nextPos);
                }
            }
        }
        
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange * 2f);
    }
}