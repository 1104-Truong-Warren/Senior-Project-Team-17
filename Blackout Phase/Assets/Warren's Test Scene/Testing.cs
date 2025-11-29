// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using CodeMonkey.Utils;

// public class Testing : MonoBehaviour
// {
//     private NewGrid<bool> grid;
//     private void Start()
//     {
//         grid = new NewGrid<bool>(11,5, 2f, new Vector3(-11, -5));
//     }

//     private void Update()
//     {
//         if(Input.GetMouseButtonDown(0))
//         {
//             grid.SetValue(UtilsClass.GetMouseWorldPosition(), false);
//         }

//         if(Input.GetMouseButtonDown(1))
//         {
//             Debug.Log(grid.GetValue(UtilsClass.GetMouseWorldPosition()));
//         }
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using CodeMonkey.Utils;

// public class Testing : MonoBehaviour
// {
//     private Pathfinding pathfinding;

//     private void Start()
//     {
//         pathfinding = new Pathfinding(10, 10);
//     }

//     private void Update()
// {
//     if(Input.GetMouseButtonDown(0))
//     {
//         Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
//         pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
//         List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
        
//         if(path != null)
//         {
//             for(int i = 0; i < path.Count - 1; i++)
//             {
//                 // Use grid's actual world positions
//                 Vector3 startPos = pathfinding.GetGrid().GetWorldPosition(path[i].x, path[i].y);
//                 Vector3 endPos = pathfinding.GetGrid().GetWorldPosition(path[i+1].x, path[i+1].y);
                
//                 // Center in cells
//                 float cellSize = pathfinding.GetGrid().GetCellSize();
//                 Vector3 centerOffset = new Vector3(cellSize * 0.5f, cellSize * 0.5f);
                
//                 Debug.DrawLine(startPos + centerOffset, endPos + centerOffset, Color.green, 5f);
//             }
//         }
//     }

//     if(Input.GetMouseButtonDown(1))
//     {
//         Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
//         pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
//         pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
//     }
// }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    private Pathfinding pathfinding;
    private CharacterMovement character;
    [SerializeField] private GameObject characterPrefab;

    // Store which cells are black (blocked)
    private HashSet<Vector2Int> blackCells = new HashSet<Vector2Int>();

    private void Start()
    {
        pathfinding = new Pathfinding(10, 10);
        CreateCharacter();
    }

    private void CreateCharacter()
    {
        if (characterPrefab != null)
        {
            GameObject characterObj = Instantiate(characterPrefab);
            character = characterObj.GetComponent<CharacterMovement>();
            
            if (character != null)
            {
                character.SetPathfinding(pathfinding);
                Vector3 startPos = pathfinding.GetGrid().GetWorldPosition(0, 0);
                float cellSize = pathfinding.GetGrid().GetCellSize();
                startPos += new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
                characterObj.transform.position = startPos;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (character != null)
            {
                character.SetMoveToMousePosition();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            PathNode node = pathfinding.GetNode(x, y);
            
            if (node != null)
            {
                // Toggle walkable state
                node.SetIsWalkable(!node.isWalkable);
                
                // Toggle black cell visual
                Vector2Int cellPos = new Vector2Int(x, y);
                if (blackCells.Contains(cellPos))
                {
                    blackCells.Remove(cellPos); // Remove from black cells
                }
                else
                {
                    blackCells.Add(cellPos); // Add to black cells
                }
            }
        }
    }

    // Draw black cells
    private void OnDrawGizmos()
    {
        if (pathfinding == null) return;

        NewGrid<PathNode> grid = pathfinding.GetGrid();
        float cellSize = grid.GetCellSize();

        // Draw black cells
        Gizmos.color = Color.black;
        foreach (Vector2Int cellPos in blackCells)
        {
            Vector3 center = grid.GetWorldPosition(cellPos.x, cellPos.y) + new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
            Vector3 size = new Vector3(cellSize, cellSize, 0.1f);
            Gizmos.DrawCube(center, size);
        }
    }

    public Pathfinding GetPathfinding()
    {
        return pathfinding;
    }
}