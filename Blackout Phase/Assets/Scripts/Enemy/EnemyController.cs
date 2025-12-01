using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    //[SerializeField] private Transform player; // get the player object

    [Header("Patrol points")]
    [SerializeField] private List<Vector2Int> patrolPoints = new List<Vector2Int>(); // patrol points

    private int patrolIndex = 1; // where at array

    private EnemyInfo enemyInfo; // enmey's status
    private EnemyPathFinder pathFinder; // finding the enemy path
    private EnemyMovement movement; // how it moves
    //private EnemyTileScanner scanner; // scanns the neighbour tiles for pathfinding

    public bool Initialized { get; set; } = false; // for initialize flag
    private bool movingForward = true;

    //private PathFinder pathFinder; // access the pathfinder

    //private EnemyPatrol enemyPatrol;

    //private bool isReady = false; // for enemy

    private void Awake()
    {
        enemyInfo = GetComponent<EnemyInfo>(); // setup enemyinfo

        EnemyTileScanner scanner = new EnemyTileScanner(); // helper clss 

        movement = GetComponent<EnemyMovement>(); // setup movement

        pathFinder = new EnemyPathFinder(scanner); // setup scanner
    }

    private void Start()
    {
        Debug.Log("EnemyController start - currentTile = " + enemyInfo.currentTile); // debug
    }

    //public void SetInitialized()
    //{
    //    Initialized = true;
    //}

    //private IEnumerator Start()
    //{
    //    yield return new WaitUntil(() => MapManager.Instance != null); // wait until Map is initialized 

    //    yield return new WaitUntil(() => enemyInfo.currentTile != null); // wait until TurnManager is set up

    //    yield return new WaitUntil(() => MapManager.Instance.map.Count > 0); // wait until map finshed load

    //    scanner = GetComponent<EnemyTileScanner>();

    //    pathFinder = new EnemyPathFinder(scanner); // setup path finding

    //    Debug.Log("Scanner from Get Component = " + scanner); // debug

    //    Debug.Log("Scanner inside pathfinder = " + pathFinder.TestScanner()); // debug


    //    //StartCoroutine(TestPatrol());
    //}

    //private IEnumerator Start()
    //{
    //    yield return new WaitUntil(() => MapManager.Instance != null); // wait until Map is initialized 

    //    yield return new WaitUntil(() => TurnManager.Instance != null); // wait until TurnManager is set up

    //    enemyInfo = GetComponent<EnemyInfo>(); // setup enmey's status

    //    //pathFinder = new PathFinder(); // setup pathfinder

    //    enemyPatrol = GetComponent<EnemyPatrol>(); // setup enemy patrol

    //    //if (player != null) // player not found? check for the tag "Player" and set that to our game object
    //    //    player = GameObject.FindWithTag("Player").transform;
    //    //else
    //    //    Debug.LogWarning("Player not found. Make sure the player has the Player tag.");
    //    //player = GameObject.FindWithTag("Player").transform; // setup the player object


    //    //if (player == null) // if player not found find the object with tag "Player"
    //    //{
    //    //    Debug.LogWarning("Player not assigned to EnemyController!");

    //    //    GameObject playerObj = GameObject.FindWithTag("Player");

    //    //    if (playerObj != null) // if found set that to the player obj
    //    //        player = playerObj.transform;

    //    //}

    //    yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds

    //    isReady = true; // now player is ready

    //    Debug.Log($"{name} is fully initialized and ready"); // debug test

    //    //StartCoroutine(FindPlayer()); // starts the turn for enemy
    //}

    //private IEnumerator FindPlayer()
    //{
    //    yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null); // wait until the player tag is found

    //    player = GameObject.FindWithTag("Player").transform; // setup the player object

    //    //StartCoroutine(TakeTurn()); // calls for TakeTurn
    //    yield return null; // wait for 1 frame

    //    TurnManager.Instance.EndEnemyTurn(); // ends enemy turn

    //    isReady = true; // if player found toggle flag
    //}

    public IEnumerator TakeTurn()
    {
        // if enemy tile is null display a debug
        if (enemyInfo.currentTile == null)
        {
            Debug.LogError($" {name} has no currentTile, skipping turn.");
            yield break;
        }

        //Debug.Log("Enemy current tile = " + enemyInfo.currentTile.gridLocation); debug

        // if patrolPoints doesn't exisit display debug msg
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning($"{name} has no patrol points, skipping turn");
            yield break;
        }

        // before moving check if player is in range
        if (PlayerDistanceCheck())
        {
            Debug.Log($"In enemy range! {name} attacks the player!"); // debug

            AttackPlayer(); // if player in range attack them
            yield break; // get out, not moving this turn
        }

        Vector2Int targetGrid = patrolPoints[patrolIndex]; // find the next patrol point

        //patrolIndex = (patrolIndex + 1) % patrolPoints.Count; // index calculation for example 0, 1 % patrolPoints = 1, 2%3 = 2, 3%3 = 0; 0 -> 1 ->2

        Debug.Log($"{name} Patrol target = {targetGrid}"); // debug msg

        OverlayTile targetTile = MapManager.Instance.GetTile(targetGrid); // get the tile from map Manager

        if (targetTile == null) // if next tile not found display error
        {
            Debug.LogError($"{name} No tile at Patrol target {targetGrid}"); // debug msg
            yield break;
        }

        Debug.Log($"{name} current tile = {enemyInfo.currentTile.gridLocation}"); // debug msg
        Debug.Log("Calling pathfinder..."); // debug msg

        List<OverlayTile> path = pathFinder.FindPath(enemyInfo.currentTile, targetTile); // current tile to next tile

        if (path.Count <= 1)
        {
            Debug.LogWarning("Path empty, enemy not moving this turn"); // debug msg
            yield break;
        }

        // if the path is not empty display it
        //if (path.Count > 0)

        //List<OverlayTile> trimmedPath = path.GetRange(1, path.Count - 1); // only go from 1 -> next

        yield return movement.MoveAlong(path.Skip(1).ToList()); // if path count > 0 delay return, got through the patrol points, skips whole path
        
        // after moving check if player in range if so attack
        if (PlayerDistanceCheck())
        {
            Debug.Log($"In enemy range! {name} attacks the player!"); // debug

            AttackPlayer(); // if player in range attack them
            yield break; // get out, not moving this turn
        }

        if (enemyInfo.currentTile == targetTile) // if we reach the tile update it
            UpdateIndex(); // updates the index

        //if (!isReady)
        //{
        //    Debug.LogWarning("Enemy not ready yet - skipping turn");

        //    yield break;
        //}

        //Debug.Log($"{name} is taking its turn.");

        ////yield return new WaitForSeconds(0.5f); // wait a little before returns

        //if (enemyPatrol != null) // if enemyPatrol exist do the patrol
        //{
        //    yield return StartCoroutine(enemyPatrol.EnemyPatrolStep());
        //}
        ////yield return null;

        ////TurnManager.Instance.EndEnemyTurn(); // end enemy's Turn
    }

    private void UpdateIndex()
    {
        if (movingForward) // flag to check which way enemy is going A->B->C or C->B->A
        {
            patrolIndex++; // move to next indext patrol position

            if (patrolIndex >= patrolPoints.Count) // check to see if enemy is doing this C->B->A reversing patrol
            {
                patrolIndex = patrolPoints.Count - 2; // if index goes over 3 or equal, and we counted total of 3 counts, 3 - 2, we are back to p[1] , C->B

                movingForward = false; // toggles flag

                //Index--; // go backwards
                //if (patrolPoints.Count > 1) // only do it when it's bigger than 1 wrong never goes back to index[0]
                //{
                //    reversing = true; // toggles flag

                //    Index = patrolPoints.Count - 2; // if index goes over 3, and we counted total of 3 counts, 3 - 2, we are back to p[1] , C->B
                //}
            }
        }
        else
        {
            patrolIndex--; // if reversing goes back 1

            if (patrolIndex < 0) // if currentIndex is < 0 means we reach A, reset reversing to F, and reset index A->B
            {
                patrolIndex = 1; // goes back from start;

                movingForward = true;

                //Index++; // move forward
            }
        }
    }

    private bool PlayerDistanceCheck()
    {
        //OverlayTile playerTile = CharacterInfo.Instance.CurrentTile; // get the player's tile

        CharacterInfo player = CharacterInfo.Instance; // setup the prefab character

        //OverlayTile enemyTile = enemyInfo.currentTile;

        //if (playerTile == null || enemyTile == null) return; // if player or enemy not tile not found get out

        if (player == null || player.CurrentTile == null) return false; // if player or the tile is not found return false
        
        int distance = Mathf.Abs(enemyInfo.currentTile.gridLocation.x - player.CurrentTile.gridLocation.x) // mahattant math to see how close is the player 
                    + Mathf.Abs(enemyInfo.currentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

        Debug.Log($"Enemy distance to play = {distance}"); // debug

        // if player is in range attack
        //if (distance <= enemyInfo.attackRange)
        //{
        //    Debug.Log($"In enemy range! {name} attacks the player!"); // debug

        //    AttackPlayer(); // if player in range attack them
        //}

        return distance <= enemyInfo.attackRange; // if palyer exsit return the distance not T/F
    }

    public void AttackPlayer()
    {
        CharacterInfo playerInfo = CharacterInfo.Instance; // setup the player, now it can access all the info

        // if player not found get out 
        if (playerInfo == null)
        {
            Debug.LogError("playerInfo not found!"); // debug
            return;
        }
        else //if (playerInfo != null) // if player found damage them 
            playerInfo.PlayerTakeDamage(enemyInfo.EnemyDmg);

        Debug.Log($"{name} attacked the player for {enemyInfo.EnemyDmg} damage! Player current HP: {playerInfo.hp}"); // debug
    }

        //private IEnumerator TestPatrol()
        //{
        //    while (true)
        //    {
        //        yield return TakeTurn();
        //        yield return new WaitForSeconds(0.5f);
        //    }
        //}

    }
