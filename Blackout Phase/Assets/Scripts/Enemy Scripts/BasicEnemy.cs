using UnityEngine;


//working with the assumption that enemies should have an update method for
//animations, movement is handled by the enemy controller
public class BasicEnemy : MonoBehaviour
{
    

    //set all these variables when you assign the script to an enemy
    public int health;
    public int visionRange;
    public int currentX;
    public int currentY;
    public int movementBudget;

    //important for deciding which direction the enemy will check for the player
    public char directionFacing; //N, S, E, W

    //array for tracking visible tiles (just a 1d array since for now we assume vision to be a line)

    public Tile[] visibleTiles;

    //array for all the locations the enemy will patrol between. size should be set by starting script
    //enemy will move to each location in order, then loop back to start
    //index stored in currentPatrolIndex
    public Tile[] patrolLocations;
    public int currentPatrolIndex = 0;

    public Grid grid;

    public Rifle weapon;

    void Start()
    {
        //setting the size of the visible tiles array to be equal to vision range
        visibleTiles = new Tile[visionRange];
        
        for (int i = 0; i < visionRange; i++)
        {
            visibleTiles[i] = null;
        }

        directionFacing = 'N'; //default to north
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //checks all cardinal directions up to vision range
    //stops checking in a direction if an inaccessible tile is found (which would indicate a wall or obstacle)
    public void UpdateVision()
    {

        for (int i = 0; i < visionRange; i++)
        {
            visibleTiles[i] = null;
        }

        switch (directionFacing) 
        {
            case 'N':
                for (int i = 1; i <= visionRange; i++)
                {
                    if (grid.grid[currentX,currentY + i].accessible)
                    {
                        visibleTiles[i - 1] = grid.grid[currentX, currentY + i];
                    } else
                    {
                        break;
                    }
                }
                break;
            case 'S':
                for (int i = 1; i <= visionRange; i++)
                {
                    if (grid.grid[currentX, currentY - i].accessible)
                    {
                        visibleTiles[i - 1] = grid.grid[currentX, currentY - i];
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case 'E':
                for (int i = 1; i <= visionRange; i++)
                {
                    if (grid.grid[currentX + i, currentY].accessible)
                    {
                        visibleTiles[i - 1] = grid.grid[currentX + i, currentY];
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case 'W':
                for (int i = 1; i <= visionRange; i++)
                {
                    if (grid.grid[currentX - i, currentY].accessible)
                    {
                        visibleTiles[i - 1] = grid.grid[currentX - i, currentY];
                    }
                    else
                    {
                        break;
                    }
                }
                break;

        }
    }

    public bool CanSeePlayer()
    {
        foreach (Tile tile in visibleTiles)
        {
            if (tile != null && tile.playerOnTile)
            {
                return true;
            }
        }
        return false;
    }

}
