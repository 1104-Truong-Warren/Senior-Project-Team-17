using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder1 
{
   public List<OverlayTile1> FindPath(OverlayTile1 start, OverlayTile1 end)
    {

        List<OverlayTile1> openList = new List<OverlayTile1>(); // add to the list

        List<OverlayTile1> closedList = new List<OverlayTile1>(); // once finished checking put them here

        openList.Add(start);

        // loop through everything
        while (openList.Count > 0)
        {
            OverlayTile1 currentOverlayTile = openList.OrderBy(x => x.F).First(); // overlay tile of the lowest overlay

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

        return new List<OverlayTile1>();
    }

    private List<OverlayTile1> GetFinishedList(OverlayTile1 start, OverlayTile1 end)
    {
        List<OverlayTile1> finishedList = new List<OverlayTile1>(); // new list

        OverlayTile1 currentTile = end; // from end

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
    public int GetManhattenDistance(OverlayTile1 start, OverlayTile1 neighbour)
    {
        return Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x) + 
            Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);
    }

    private List<OverlayTile1> GetNeighbourTiles(OverlayTile1 currentOverlayTile)
    {
        var map = MapManager1.Instance.map; // from the mapManager

        List<OverlayTile1> neighbours = new List<OverlayTile1>();
        
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
