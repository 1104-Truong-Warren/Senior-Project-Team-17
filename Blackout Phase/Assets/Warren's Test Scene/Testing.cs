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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    private Pathfinding pathfinding;

    private void Start()
    {
        pathfinding = new Pathfinding(10, 10);
    }

    private void Update()
{
    if(Input.GetMouseButtonDown(0))
    {
        Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
        List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
        
        if(path != null)
        {
            for(int i = 0; i < path.Count - 1; i++)
            {
                // Use grid's actual world positions
                Vector3 startPos = pathfinding.GetGrid().GetWorldPosition(path[i].x, path[i].y);
                Vector3 endPos = pathfinding.GetGrid().GetWorldPosition(path[i+1].x, path[i+1].y);
                
                // Center in cells
                float cellSize = pathfinding.GetGrid().GetCellSize();
                Vector3 centerOffset = new Vector3(cellSize * 0.5f, cellSize * 0.5f);
                
                Debug.DrawLine(startPos + centerOffset, endPos + centerOffset, Color.green, 5f);
            }
        }
    }

    if(Input.GetMouseButtonDown(1))
    {
        Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
        pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
        pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
    }
}
}