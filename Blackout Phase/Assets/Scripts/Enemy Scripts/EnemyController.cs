using UnityEngine;

public class EnemyController : MonoBehaviour
{

    //array for holding all enemies that are in the scene
    //enemy count for tracking how many enemies are currently active
    public BasicEnemy[] enemies;
    public int enemyCount;

    //used for state tracking
    //patrol indicates that the enemy is moving between patrol points
    //alert indicates that the enemy has seen the player and is moving to engage
    //evasion indicates that the enemy has lost vision and is searching last known location
    string state;

    Tile lastSeenPlayerTile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemies = FindObjectsByType<BasicEnemy>(FindObjectsSortMode.None);


        enemyCount = enemies.Length;
        Debug.Log("Enemy Count: " + enemyCount);

        //always default to patrol on starting a scene
        state = "patrol";
    }


    //iterates through every enemy on the map and has them take their turn
    //enemy behavior changes based on the current state
    //state is updated before each enemy's turn
    public void TakeTurn()
    {
        Debug.Log("Enemy Controller Taking Turn");
        bool playerSpotted = false;

        for (int i = 0; i < enemyCount; i++)
        {
            for (int j = 0; j < enemyCount; j++)
            {
                enemies[j].UpdateVision();
                playerSpotted = enemies[j].CanSeePlayer();
            }

            //sets the state to alert if the player is spotted
            if (playerSpotted)
            {
                state = "alert";
            }
            else
            {
                //if the player is not spotted and the enemy was previously alert, change to evasion
                if (state == "alert")
                {
                    state = "evasion";
                }
                //need to set the condition for returning to patrol here, working on it
            }

            switch(state)
            {
                case "patrol":
                    enemies[i].Patrol();
                    break;
                case "alert":
                    //enemies[i].EngagePlayer();
                    break;
                case "evasion":
                    //enemies[i].SearchLastKnownLocation(lastSeenPlayerTile);
                    break;
                default:
                    Debug.Log("Error: Invalid enemy state");
                    break;
            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
