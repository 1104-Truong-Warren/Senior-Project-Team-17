using UnityEngine;

public class GridBehavior : MonoBehaviour
{

    //setting variables for grid size, should be set using the in-engine inspector
    //this will necesitate that all maps are technically rectangular, but we can mark out map boundaries with inaccessible tiles
    public int rows;
    public int columns;
    public int scale = 1;

    //prefab for the grid tile
    public GameObject gridPrefab;

    //setting the origin position of the grid to be at 0,0,0
    public Vector3 originPosition = new Vector3(0, 0, 0);

    // Warren | represents the map grid, [,] means it has two dimensions such as rows and columns.
    public GameObject[,] gridArray; 
    // Warren | the starting coordinates of the grid
    public int startX = 0; 
    public int startY = 0; 
    // Warren | the ending coordinates of the grid
    public int endX = 2; 
    public int endY = 2; 


    //when activated, generate the grid based on the GenerateGrid function
    //if the script is not properly attached to the gridPrefab, log an error
    void Start()
    {
        // Warren | creates an empty 2D grid where each cell can store a reference to a GameObject later.
        gridArray = new GameObject[columns, rows]; 
        if (gridPrefab)
        {
            GenerateGrid();            
        } else
        {
            Debug.LogError("Grid Prefab not assigned in GridBehavior script attached to " + gameObject.name);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }



    void GenerateGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                //instantiating the grid tile prefab at the correct position based on the origin position and scale, uses z for the j scale since in unity y is up
                GameObject gridTile = Instantiate(gridPrefab, new Vector3(originPosition.x + scale * i, originPosition.y, originPosition.z + scale * j), Quaternion.identity);

                //assigning a coordinate to each tile based on its position in the grid
                gridTile.transform.SetParent(gameObject.transform);
                gridTile.GetComponent<GridStat>().x = i;
                gridTile.GetComponent<GridStat>().y = j;
                gridArray[i, j] = gridTile; // Warren | assigns objects into the grid
            }
        }
    }
    // Warren | function that sets up the grid so we could label the points.
    void InitalSetup()
    {
        // Warren | goes through every GameObject in the grid and resets its visited status to -1, and 0 is the starting point.
        foreach (GameObject obj in gridArray)
        {
            obj.GetComponent<GridStat>().visited = -1;
        }
        gridArray[startX, startY].GetComponent<GridStat>().visited = 0;
    }

    // Warren
    bool TestDirection(int x, int y, int step, int direction)
    {
        // Warren | int direction: 1 = up, 2 = right, 3 = down, 4 = left
        switch(direction)
        {
            // Warren | checks whether it is moving up
            case 1:
                if (y + 1 < rows && gridArray[x, y + 1] && gridArray[x, y + 1].GetComponent<GridStat>().visited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            // Warren | checks whether it is moving right
            case 2:
                if (x + 1 < columns && gridArray[x + 1, y] && gridArray[x + 1, y].GetComponent<GridStat>().visited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            
            // Warren | checks whether it is moving down
            case 3:
                if (y - 1 > -1 && gridArray[x, y - 1] && gridArray[x, y - 1].GetComponent<GridStat>().visited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            // Warren | checks whether it is moving left
            case 4:
                if (x - 1 > -1 && gridArray[x + 1, y] && gridArray[x - 1, y].GetComponent<GridStat>().visited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            default:
                return false;
        }
    }
}
