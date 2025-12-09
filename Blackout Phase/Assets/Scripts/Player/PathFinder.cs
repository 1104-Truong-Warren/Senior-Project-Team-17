using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder 
{
   public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end)
    {

        List<OverlayTile> openList = new List<OverlayTile>(); // add to the list

        List<OverlayTile> closedList = new List<OverlayTile>(); // once finished checking put them here

        openList.Add(start);

        // loop through everything
        while (openList.Count > 0)
        {
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First(); // overlay tile of the lowest overlay

            openList.Remove(currentOverlayTile);

            closedList.Add(currentOverlayTile); // store to closed

            if (currentOverlayTile == end)
            {
                //finalize the path
                return GetFinishedList(start, end);
            }

            var neighbourTiles = GetNeighbourTiles(currentOverlayTile);

            foreach (var neighbour in neighbourTiles)
            {
                // 1 = jump hight, check if tile is blocked, in the colosedList, matching the z-axis
                if (neighbour.isBlocked || closedList.Contains(neighbour)) //|| 
                    //Mathf.Abs(currentOverlayTile.gridLocation.y - neighbour.gridLocation.y) > 1)
                {
                    continue;
                }

                neighbour.G = GetManhattenDistance(start, neighbour); // save it to Overlay

                neighbour.H = GetManhattenDistance(end, neighbour);

                neighbour.previousTile = currentOverlayTile; // store it to previouse tile

                // can't open the list
                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }

        }

        return new List<OverlayTile>();
    }

    private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
    {
        List<OverlayTile> finishedList = new List<OverlayTile>(); // new list

        OverlayTile currentTile = end; // from end

        // read in everything but backwards
        while (currentTile != start)
        {
            finishedList.Add(currentTile);

            currentTile.ShowPlayerTile(); // shows the tile 

            currentTile = currentTile.previousTile;
        }

        finishedList.Reverse(); // reverse the list to sort it

        return finishedList;
    }

    // for find the square distance
    public int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + 
            Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

    private List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile)
    {
        var map = MapManager.Instance.map; // from the mapManager

        List<OverlayTile> neighbours = new List<OverlayTile>();
        
        // Top
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, 
            currentOverlayTile.gridLocation.y + 1
        );

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        // Bottom
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x,
           currentOverlayTile.gridLocation.y - 1
        );

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        // Right
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1,
           currentOverlayTile.gridLocation.y
        );

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        // Left
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1,
           currentOverlayTile.gridLocation.y
        );

        if (map.ContainsKey(locationToCheck))
        {
            neighbours.Add(map[locationToCheck]);
        }

        return neighbours; 
    }
}
