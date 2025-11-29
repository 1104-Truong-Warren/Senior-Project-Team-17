using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Pathfinding pathfinding;
    
    private List<PathNode> currentPath;
    private int currentPathIndex;
    private bool isMoving = false;
    private Vector3 targetPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetMoveToMousePosition();
        }

        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    public void SetMoveToMousePosition()
    {
        if (pathfinding == null || pathfinding.GetGrid() == null)
        {
            Debug.LogError("Pathfinding or Grid is null!");
            return;
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int targetX, out int targetY);
        
        // Get character's current grid position
        pathfinding.GetGrid().GetXY(transform.position, out int startX, out int startY);
        
        // Find path from current position to target
        currentPath = pathfinding.FindPath(startX, startY, targetX, targetY);
        
        if (currentPath != null && currentPath.Count > 0)
        {
            currentPathIndex = 0;
            isMoving = true;
            SetNextTargetPosition();
        }
        else
        {
            Debug.Log("No path found!");
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    private void SetNextTargetPosition()
    {
        if (currentPathIndex < currentPath.Count)
        {
            PathNode node = currentPath[currentPathIndex];
            float cellSize = pathfinding.GetGrid().GetCellSize();
            
            // Get world position and center in cell
            targetPosition = pathfinding.GetGrid().GetWorldPosition(node.x, node.y);
            targetPosition += new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
            
            currentPathIndex++;
        }
        else
        {
            // Reached destination
            isMoving = false;
            Debug.Log("Reached destination!");
        }
    }

    private void MoveAlongPath()
    {
        if (pathfinding == null) return;

        // Move towards current target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
        // Check if reached current target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNextTargetPosition();
        }
    }

    public void SetPathfinding(Pathfinding pf)
    {
        pathfinding = pf;
    }

    // Visualize current path in Scene view
    private void OnDrawGizmosSelected()
    {
        if (currentPath != null && currentPath.Count > 0 && pathfinding != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Vector3 startPos = pathfinding.GetGrid().GetWorldPosition(currentPath[i].x, currentPath[i].y);
                Vector3 endPos = pathfinding.GetGrid().GetWorldPosition(currentPath[i+1].x, currentPath[i+1].y);
                
                float cellSize = pathfinding.GetGrid().GetCellSize();
                Vector3 centerOffset = new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
                
                Gizmos.DrawLine(startPos + centerOffset, endPos + centerOffset);
                Gizmos.DrawSphere(startPos + centerOffset, 0.1f);
            }
        }
    }
}