using UnityEngine;

public class Tile
{
    //tracks the x and y position of the tile in the grid
    public int x;
    public int y;


    //pathfinding variables
    //movement cost to enter this tile
    //is the tile accessible
    //whether the tile has been visited in pathfinding, should be reset to false after each pathfinding operation
    public int movementCost;
    public bool accessible;
    public bool visited;

    public GameObject tileObject;



    public Tile(int x, int y, int movementCost, bool accessible)
    {
        this.x = x;
        this.y = y;
        this.movementCost = movementCost;
        this.accessible = accessible;
        this.visited = false;
        this.tileObject = null;

    }

}
