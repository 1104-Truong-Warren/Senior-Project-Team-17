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

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using CodeMonkey.Utils;

// public class Testing : MonoBehaviour
// {
//     private Pathfinding pathfinding;
//     private CharacterMovement character;
//     [SerializeField] private GameObject characterPrefab;

//     // Store which cells are black (blocked)
//     private HashSet<Vector2Int> blackCells = new HashSet<Vector2Int>();

//     private void Start()
//     {
//         pathfinding = new Pathfinding(10, 10);
//         CreateCharacter();
//     }

//     private void CreateCharacter()
//     {
//         if (characterPrefab != null)
//         {
//             GameObject characterObj = Instantiate(characterPrefab);
//             character = characterObj.GetComponent<CharacterMovement>();
            
//             if (character != null)
//             {
//                 character.SetPathfinding(pathfinding);
//                 Vector3 startPos = pathfinding.GetGrid().GetWorldPosition(0, 0);
//                 float cellSize = pathfinding.GetGrid().GetCellSize();
//                 startPos += new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
//                 characterObj.transform.position = startPos;
//             }
//         }
//     }

//     private void Update()
//     {
//         if (Input.GetMouseButtonDown(0))
//         {
//             if (character != null)
//             {
//                 character.SetMoveToMousePosition();
//             }
//         }

//         if (Input.GetMouseButtonDown(1))
//         {
//             Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
//             pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
//             PathNode node = pathfinding.GetNode(x, y);
            
//             if (node != null)
//             {
//                 // Toggle walkable state
//                 node.SetIsWalkable(!node.isWalkable);
                
//                 // Toggle black cell visual
//                 Vector2Int cellPos = new Vector2Int(x, y);
//                 if (blackCells.Contains(cellPos))
//                 {
//                     blackCells.Remove(cellPos); // Remove from black cells
//                 }
//                 else
//                 {
//                     blackCells.Add(cellPos); // Add to black cells
//                 }
//             }
//         }
//     }

//     // Draw black cells
//     private void OnDrawGizmos()
//     {
//         if (pathfinding == null) return;

//         NewGrid<PathNode> grid = pathfinding.GetGrid();
//         float cellSize = grid.GetCellSize();

//         // Draw black cells
//         Gizmos.color = Color.black;
//         foreach (Vector2Int cellPos in blackCells)
//         {
//             Vector3 center = grid.GetWorldPosition(cellPos.x, cellPos.y) + new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
//             Vector3 size = new Vector3(cellSize, cellSize, 0.1f);
//             Gizmos.DrawCube(center, size);
//         }
//     }

//     public Pathfinding GetPathfinding()
//     {
//         return pathfinding;
//     }
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
    [SerializeField] private GameObject enemyPrefab;
    
    private HealthSystem playerHealth;
    private List<EnemyController> enemies = new List<EnemyController>();

    // Store which cells are black (blocked)
    private HashSet<Vector2Int> blackCells = new HashSet<Vector2Int>();

    private void Start()
    {
        pathfinding = new Pathfinding(10, 10);
        CreateCharacter();
        
        // Wait one frame to ensure character is created, then spawn enemies
        StartCoroutine(DelayedEnemySpawn());
    }

    private IEnumerator DelayedEnemySpawn()
    {
        yield return null; // Wait one frame
        SpawnEnemies();
    }

    private void CreateCharacter()
    {
        if (characterPrefab != null)
        {
            GameObject characterObj = Instantiate(characterPrefab);
            character = characterObj.GetComponent<CharacterMovement>();
            playerHealth = characterObj.GetComponent<HealthSystem>();
            
            if (character != null && playerHealth != null)
            {
                character.SetPathfinding(pathfinding);
                Vector3 startPos = pathfinding.GetGrid().GetWorldPosition(0, 0);
                float cellSize = pathfinding.GetGrid().GetCellSize();
                startPos += new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
                characterObj.transform.position = startPos;
                
                playerHealth.OnDeath += OnPlayerDeath;
                Debug.Log("Character created successfully!");
            }
        }
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab != null && character != null)
        {
            Vector2Int[] patrol1 = new Vector2Int[] { 
                new Vector2Int(2, 2), 
                new Vector2Int(5, 2), 
                new Vector2Int(5, 5), 
                new Vector2Int(2, 5) 
            };
            
            // Vector2Int[] patrol2 = new Vector2Int[] { 
            //     new Vector2Int(7, 7), 
            //     new Vector2Int(7, 3), 
            //     new Vector2Int(3, 3) 
            // };
            
            SpawnEnemyAt(patrol1[0], patrol1);
            // SpawnEnemyAt(patrol2[0], patrol2);
        }
    }
    
    private void SpawnEnemyAt(Vector2Int gridPos, Vector2Int[] patrolRoute)
    {
        GameObject enemyObj = Instantiate(enemyPrefab);
        Vector3 worldPos = pathfinding.GetGrid().GetWorldPosition(gridPos.x, gridPos.y);
        float cellSize = pathfinding.GetGrid().GetCellSize();
        worldPos += new Vector3(cellSize * 0.5f, cellSize * 0.5f, 0);
        enemyObj.transform.position = worldPos;
        
        EnemyController enemy = enemyObj.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.Initialize(pathfinding, character.transform);
            enemies.Add(enemy);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click - move character
        {
            if (character != null && playerHealth != null && !playerHealth.IsDead)
            {
                character.SetMoveToMousePosition();
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right click - toggle walkable
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
                
                Debug.Log($"Tile ({x}, {y}) is now {(node.isWalkable ? "walkable" : "BLOCKED")}");
            }
        }

        // Health testing
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(25);
            }
        }
        
        // Test enemy damage
        if (Input.GetKeyDown(KeyCode.U) && enemies.Count > 0)
        {
            HealthSystem enemyHealth = enemies[0].GetComponent<HealthSystem>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(10);
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

    private void OnPlayerDeath()
    {
        Debug.Log("GAME OVER - Player Died!");
        foreach (EnemyController enemy in enemies)
        {
            enemy.StopAllCoroutines();
        }
    }

    public Pathfinding GetPathfinding()
    {
        return pathfinding;
    }
}