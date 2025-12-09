using UnityEngine;
using System.Collections.Generic;

public class EnemyTileScanner
{
    //  for reading in the directions
    private static readonly Vector2Int[] Directions = 
        { Vector2Int.up, Vector2Int.down, 
          Vector2Int.left, Vector2Int.right
        };

    public List<OverlayTile> GetNeighbours(OverlayTile tile)
    {
        //Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) }; //Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }; // setup the vection directions

        //int tileLayerMask = LayerMask.GetMask("OverlayTile"); // get the layermask 

        List<OverlayTile> result = new List<OverlayTile>();// new list for tile scanner

        // if everything is not found returns the empty result
        if (tile == null || MapManager1.Instance == null || MapManager1.Instance.map == null)
            return result;

        var map = MapManager1.Instance.map; // get map directly

        Vector2Int position = new(tile.gridLocation.x, tile.gridLocation.y); // get current grid position

        foreach (var direction in Directions) // search every single direction neighbour tiles
        {
            //RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, dir, 1f, tileLayerMask);  // use raycast instead of know where to go

            Vector2Int neighbourKey = position + direction; // what position and direction is the neighbour tile

            if (map.TryGetValue(neighbourKey, out OverlayTile neighbour))// collider not null found something 
            {
                //Debug.Log($"Scanner hit: {hit.collider.name} at {hit.collider.transform.position}"); // debug

                //OverlayTile t = hit.collider.GetComponent<OverlayTile>(); // setup overlay

                // only allows if tile not blocked and no enemies on it
                if (!neighbour.isBlocked && !neighbour.hasEnemy)
                    result.Add(neighbour);

                //if (t == null)
                //{
                //    Debug.Log("Hit something NOT an overlay tile!"); //debug
                //    continue;
                //}

                //if (!t.isBlocked && t != null) // if tile is not blocked added to neighbor
                //    result.Add(t);
                //else
                //    Debug.Log($"Scanner found NOthing in direct {dir}"); //debug
            }
        }

        return result; // returns the founds
    }
}
