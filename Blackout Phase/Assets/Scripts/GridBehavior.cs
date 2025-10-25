using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

//used for reading CSV files
using System.IO;
using JetBrains.Annotations;

public class GridBehavior : MonoBehaviour
{
    // Ellison
    // likely not necessary for functionality, just set here and toggled using Unity inspector to trigger distance finding on and off (see Update function)
    public bool findDistance = false;

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

    public string weightInput; //name of the CSV file to read from for setting tile weights

    


    // Ellison
    // This path is the running list for the most optimal route to the destination
    public List<GameObject> path = new List<GameObject>();




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

    // Ellison | added code to update
    // Update is called once per frame
    void Update()
    {
        if (findDistance)
        {
            SetDistance();
            SetPath();
            findDistance = false;
        }
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

        SetWeight();
    }


    //Jason
    //Sets the weight of each tile in the grid by reading from a provided csv file.
    //Might change this later to also allow for setting textures of the tiles.

    //This function assumes that 0,0 is the top left corner of the grid, as does the csv
    void SetWeight()
    {

        StreamReader dataInput = new StreamReader(weightInput);
        //verifying that the dataInput file has been provided
        if (dataInput == null)
        {
            Debug.LogError("No CSV file provided for SetWeight function in GridBehavior script attached to " + gameObject.name);
            return;
        } else
        {
            for (int i = 0; i < columns; i++)
            {
                //reading a line from the CSV file
                string dataLine = dataInput.ReadLine();

                //splitting the line into individual string values based on the comma delimiter
                string[] dataValues = dataLine.Split(',');

                for (int j = 0; j < rows; j++)
                {
                    //parsing the string value into an integer weight
                    int tileWeight = int.Parse(dataValues[j]);
                    //assigning the weight to the corresponding tile in the grid
                    gridArray[i, j].GetComponent<GridStat>().weight = tileWeight;
                }
            }
        }


    }

    // Ellison
    void SetDistance()
    {
        InitialSetup();
        int x = startX;
        int y = startY;
        int[] testArray = new int[rows * columns]; // creates an array larger than any possible movement
        for (int step = 1; step < rows * columns; step++)
        {
            foreach (GameObject obj in gridArray) // for each tile in the grid array
            {
                if (obj && obj.GetComponent<GridStat>().visited == step - 1) // if current tile's visited status is equal to step - 1
                {
                    TestFourDirections(obj.GetComponent<GridStat>().x, obj.GetComponent<GridStat>().y, step); // then set valid adjacent tiles' visited to step
                }
            }
        }
    }

    // Ellison
    void SetPath()
    {
        int step;
        int x = endX;
        int y = endY;
        List<GameObject> tempList = new List<GameObject>();
        path.Clear();

        if (gridArray[endX, endY] && gridArray[endX, endY].GetComponent<GridStat>().visited > 0)
        {
            path.Add(gridArray[x, y]);
            step = gridArray[x, y].GetComponent<GridStat>().visited - 1; // traversing backwards
        }
        else
        {
            print("Can't reach the desired location.");
            return;
        }

        for (int i = step; step > -1; step--) // start at last spot and go down to 0
        {
            if (TestDirection(x, y, step, 1)) // testing up
            {
                tempList.Add(gridArray[x, y + 1]);
            }
            if (TestDirection(x, y, step, 2)) // testing right
            {
                tempList.Add(gridArray[x + 1, y]);
            }
            if (TestDirection(x, y, step, 3)) // testing down
            {
                tempList.Add(gridArray[x, y - 1]);
            }
            if (TestDirection(x, y, step, 4)) // testing left
            {
                tempList.Add(gridArray[x - 1, y]);
            }

            GameObject tempObj = FindClosest(gridArray[endX, endY].transform, tempList); // gets closest object
            path.Add(tempObj); // adds it to path
            x = tempObj.GetComponent<GridStat>().x; // updates x and y to that object's coordinates
            y = tempObj.GetComponent<GridStat>().y;
            tempList.Clear(); // clear tempList for next iteration
        }
    }

    // Warren | function that sets up the grid so we could label the points.
    void InitialSetup()
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
                if (x - 1 > -1 && gridArray[x - 1, y] && gridArray[x - 1, y].GetComponent<GridStat>().visited == step)
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


    // Ellison
    // function tests all four directions for valid movement and sets values accordingly
    void TestFourDirections(int x, int y, int step)
    {
        if (TestDirection(x, y, -1, 1)) // -1 step in 1 direction (up)
        {
            SetVisited(x, y + 1, step);
        }
        if (TestDirection(x, y, -1, 2)) // -1 step in 2 direction (right)
        {
            SetVisited(x + 1, y, step);
        }
        if (TestDirection(x, y, -1, 3)) // -1 step in 3 direction (down)
        {
            SetVisited(x, y - 1, step);
        }
        if (TestDirection(x, y, -1, 4)) // -1 step in 4 direction (left)
        {
            SetVisited(x - 1, y, step);
        }
    }

    // Ellison
    // function sets "visited" value of a tile at coordinates (x, y) to step parameter
    void SetVisited(int x, int y, int step)
    {
        if (gridArray[x, y])
        {
            gridArray[x, y].GetComponent<GridStat>().visited = step;
        }
    }

    // Ellison
    // function finds the shortest distance from targetLocation to any object in the list and returns that object
    GameObject FindClosest(Transform targetLocation, List<GameObject> list)
    {
        float currentDistance = scale * rows * columns; // arbitrarily large distance
        int indexNumber = 0;
        for (int i = 0; i < list.Count; i++) // for each object in list
        {
            if (Vector3.Distance(targetLocation.position, list[i].transform.position) < currentDistance) // if shorter distance found
            {
                currentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position); // replace with new shortest distance
                indexNumber = i; // update indexNumber to this current one
            }
        }
        return list[indexNumber];
    }
}
