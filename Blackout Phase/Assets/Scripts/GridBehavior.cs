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


    //when activated, generate the grid based on the GenerateGrid function
    //if the script is not properly attached to the gridPrefab, log an error
    void Start()
    {
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
            }
        }
    }

}
