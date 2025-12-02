using UnityEngine;

public class AccessTest : MonoBehaviour
{
    public Grid grid;
    public EnemyController enemyController;
    public bool isActive = false;
    public bool clearSpheres = false;
    public bool pathfindTest = false;
    public bool testEnemyPatrol = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            //grid.grid[2, 1].accessible = false;
            //grid.grid[5, 6].accessible = false;
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
            pathfindTest = false;
            grid.CallAccessibleTiles(0, 0, 3);
            grid.TestEnemyPath(0, 0, 2, 0, 3);
        }

        if (testEnemyPatrol)
        {
            //Debug.Log(grid.path.Count);
            testEnemyPatrol = false;
            enemyController.TakeTurn();

        }
    }


}
