using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //stores the display objects for accessible tiles so they can be removed later
    Stack<Tile> path = new Stack<Tile>();

    public int width;
    public int height;

    public Tile[,] grid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        grid = new Tile[width, height];

        //ideally we would load things like movecost off of a file or something, but for now we can just load
        //default values
        //this will need to be adjusted later
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new Tile(x, y, 1, true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignObjectToTile(int xpos, int ypos, GameObject obj)
    {
        if (xpos < 0 || xpos >= width || ypos < 0 || ypos >= height)
        {
            Debug.LogError("AssignObjectToTile: Position out of bounds");
            return;
        }
        grid[xpos, ypos].tileObject = obj;
    }


    //calls the accessible tiles function after resetting visited flags
    public void CallAccessibleTiles(int startx, int starty, int budgetRemaining)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].visited = false;
            }
        }

        AccessibleTiles(startx, starty, budgetRemaining);
        grid[startx, starty].visited = false; //don't count the starting tile as accessible
        DisplayAccessibleTiles();

    }
    //given a starting position, find the tiles that are reachable within the given budget
    //this function should be called from another one that resets the visited flags on all tiles first
    private void AccessibleTiles(int startx, int starty, int budgetRemaining)
    {
        //check each adjacent tile to see if it is acceptable

        //go in order of up, right, down, left
        if (starty + 1 < height && grid[startx,starty+1].accessible)
        {
            if (budgetRemaining - grid[startx, starty + 1].movementCost >= 0)
            {
                grid[startx, starty + 1].visited = true;
                AccessibleTiles(startx, starty + 1, budgetRemaining - grid[startx, starty + 1].movementCost);
            }
            
        }

        if (startx + 1 < width && grid[startx + 1, starty].accessible)
        {
            if (budgetRemaining - grid[startx + 1, starty].movementCost >= 0)
            {
                grid[startx + 1, starty].visited = true;
                AccessibleTiles(startx + 1, starty, budgetRemaining - grid[startx + 1, starty].movementCost);
            }

        }

        if (starty - 1 >= 0 && grid[startx, starty - 1].accessible)
        {
            if (budgetRemaining - grid[startx, starty - 1].movementCost >= 0)
            {
                grid[startx, starty - 1].visited = true;
                AccessibleTiles(startx, starty - 1, budgetRemaining - grid[startx, starty - 1].movementCost);
            }
        }

        if (startx - 1 >= 0 && grid[startx - 1, starty].accessible)
        {
            if (budgetRemaining - grid[startx - 1, starty].movementCost >= 0)
            {
                grid[startx - 1, starty].visited = true;
                AccessibleTiles(startx - 1, starty, budgetRemaining - grid[startx - 1, starty].movementCost);
            }
        }

        

        return;
    }

    private void DisplayAccessibleTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].visited)
                {
                    GameObject accessObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    accessObject.tag = "AccessibleTileDisplay";
                    accessObject.transform.position = new Vector3(x , y, -1);

                }
            }
            
        }
    }

    //player variant of pathfinding function, always assumes the destination tile is reachable
    //uses a bool return type to show if a given tile was on the path to target or not
    public bool GetPlayerPathToDestination(int startx, int starty, int endx, int endy, int budgetRemaining)
    {

        if (startx == endx && starty == endy)
        {
            path.Push(grid[startx, starty]);
            return true;
        }

        bool pathToDest = false;

        //same idea as Accessible tiles, but instead of just marking tiles visited, find
        //path to destination

        //check in the same order as AccessibleTiles
        //this will result in slow execution if the destination is down and left, but that should be
        //ok since this is a turn-based game anyway


        if (grid[startx,starty+1].visited && (budgetRemaining -  grid[startx,starty+1].movementCost) >= 0)
        {
            pathToDest = GetPlayerPathToDestination(startx, starty + 1, endx, endy, budgetRemaining - grid[startx, starty + 1].movementCost);
            if (pathToDest)
            {
                path.Push(grid[startx, starty]);
                return true;
            }
        }

        if (grid[startx + 1, starty].visited && (budgetRemaining - grid[startx + 1, starty].movementCost) >= 0)
        {
            pathToDest = GetPlayerPathToDestination(startx + 1, starty, endx, endy, budgetRemaining - grid[startx + 1, starty].movementCost);
            if (pathToDest)
            {
                path.Push(grid[startx, starty]);
                return true;
            }
        }

        if (grid[startx, starty - 1].visited && (budgetRemaining - grid[startx, starty - 1].movementCost) >= 0)
        {
            pathToDest = GetPlayerPathToDestination(startx, starty - 1, endx, endy, budgetRemaining - grid[startx, starty - 1].movementCost);
            if (pathToDest)
            {
                path.Push(grid[startx, starty]);
                return true;
            }
        }

        if (grid[startx - 1, starty].visited && (budgetRemaining - grid[startx - 1, starty].movementCost) >= 0)
        {
            pathToDest = GetPlayerPathToDestination(startx - 1, starty, endx, endy, budgetRemaining - grid[startx - 1, starty].movementCost);
            if (pathToDest)
            {
                path.Push(grid[startx, starty]);
                return true;
            }
        }

        return false;
    }

    public void EnemyPathToDestination(int startx, int starty, int endx, int endy, int budgetRemaining)
    {
        if (grid[endx, endy].visited == true)
        {

            //if the destination tile is reachable, just use the player pathfind function
            GetPlayerPathToDestination(startx, starty, endx, endy, budgetRemaining);
        } else
        {
            //if the destination tile is not reachable, find the closest reachable tile to it
            Tile closestTile = null;
            float closestDistance = float.MaxValue;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y].visited)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(endx, endy));
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestTile = grid[x, y];
                        }
                    }
                }
            }
            if (closestTile != null)
            {
                GetPlayerPathToDestination(startx, starty, closestTile.x, closestTile.y, budgetRemaining);
            }
        }
    }

    public void TestPathDisplay(int startx, int starty, int endx, int endy, int budgetRemaining)
    {
        bool foundPath = GetPlayerPathToDestination(startx, starty, endx, endy, budgetRemaining);

        if (foundPath)
        {

            while (path.Count > 0)
            {
                Tile currentTile = path.Pop();

                GameObject pathObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                pathObject.tag = "AccessibleTileDisplay";
                pathObject.transform.position = new Vector3(currentTile.x, currentTile.y, -1);
            }
            
        }
    }

    public void TestEnemyPath(int startx, int starty, int endx, int endy, int budgetRemaining)
    {
        EnemyPathToDestination(startx, starty, endx, endy, budgetRemaining);
        while (path.Count > 0)
        {
            Tile currentTile = path.Pop();
            GameObject pathObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pathObject.tag = "AccessibleTileDisplay";
            pathObject.transform.position = new Vector3(currentTile.x, currentTile.y, -1);
        }
    }

}
