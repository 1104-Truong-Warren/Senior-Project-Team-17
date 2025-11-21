using UnityEngine;

public class AccessTest : MonoBehaviour
{
    public Grid grid;
    public bool isActive = false;
    public bool clearSpheres = false;
    public bool pathfindTest = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            grid.grid[2,1].accessible = false;
            grid.grid[5,6].accessible = false;
            isActive = false;
            grid.CallAccessibleTiles(5, 5, 3);
        }

        if (clearSpheres)
        {
            clearSpheres = false;
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("AccessibleTileDisplay"))
            {
                Destroy(obj);
            }
        }

        if (pathfindTest)
        {
            grid.TestEnemyPath(5,5,0,0,3);
        }

    }


}
