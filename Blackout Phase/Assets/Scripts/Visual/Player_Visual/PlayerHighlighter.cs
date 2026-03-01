using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHighlighter : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private MapManager1 map; // defineds the map for use

    private readonly List<OverlayTile1> highlights = new List<OverlayTile1>(); // a List of OverlayTile for displaying the highlights

    private void Awake()
    {
        // if MapManager is not set copy it
        if (map == null)
            map = MapManager1.Instance;
    }

    public void ShowPlayerMovementTiles(OverlayTile1 centerTile, int tileRange, bool tileOccupied = true)
    {
        ClearHighlights(); // clear the previous highlights

        // player tile not found, map is not found or the dictionary is empty get out
        if (centerTile == null || map == null || map.map == null) return;

        // foreach loop go through all the keyValues for player on the map
        foreach (var keyValue in map.map)
        {
            OverlayTile1 tile = keyValue.Value; // saves the tile as key value

            // ignore the null tiles
            if (tile == null) continue;

            int distance = Manhattan(centerTile.gridLocation, tile.gridLocation); // calculates the tile distance from center tile to key value tiles

            // if the distance is within the player's movement range
            if (distance <= tileRange)
            {
                // ignores the blocked tiles 
                if (tile.isBlocked) continue;

                // ignores the tiles that are being used and non-centerTile (player's center)
                if (tileOccupied && tile.Occupied && tile != centerTile) continue;

                tile.ShowPlayerMoveRangeTile(); // display the tile color highlight

                highlights.Add(tile); // add tile to highlight
            }
        }
    }

    public void ShowPlayerAttackRangeTiles(OverlayTile1 centerTile, int attkRange)
    {
        ClearHighlights(); // clear the previous highlights

        // player tile not found, map is not found or the dictionary is empty get out
        if (centerTile == null || map?.map == null) return;

        // foreach loop go through all the keyValues for player on the map
        foreach (var tile in map.map.Values)
        {
            // ignore the null tiles
            if (tile == null) continue;

            int distance = Manhattan(centerTile.gridLocation, tile.gridLocation); // calculates the tile distance from center tile to key value tiles

            // if the distance is within the player's movement range
            if (distance <= attkRange)
            {
                tile.ShowPlayerAttackRangeTile(); // display the tile color highlight

                highlights.Add(tile); // add tile to highlight
            }
        }

        centerTile.ShowPlayerTile(); // keep player tile
    }

    public void SingleTileHighlight(OverlayTile1 tile, bool enemy = false)
    {
        // the tile doesn't exist get out
        if (tile == null) return;

        ClearHighlights(); // clear the previous hightlights

        // if the tile 
        if (enemy)
            tile.ShowEnemyTile(); 
        else
            tile.ShowEnemyTile();

        highlights.Add(tile); // add tile to the list
    }

    public void ClearHighlights()
    {
        // foreach loop to clear all the highlighted tiles
        foreach (var tile in highlights)
        {
            // if tile found make it not visiable 
            if (tile != null)
                tile.HideTile();     
        }

        highlights.Clear(); //cleears the list
    }

    private int Manhattan(Vector3Int a, Vector3Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // returns the calculation for the tiles around the player/enemy
}
