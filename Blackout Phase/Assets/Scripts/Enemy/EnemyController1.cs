using UnityEngine; // default
using System.Collections; // for the array list we have also IEnumerator for delay funciton calls yield returns. loading map first then do something else
using System.Collections.Generic; // for the List<T> and dictionary <T, T> for pathfinding
using System.Linq; // filter numbers that are greater than 10, x=> x.F is using it, ordering etc...
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

// enemy changes state depending on what player is doing
public enum EnemyState
{
    Patrol,
    Alter,
    Attack
}

public class EnemyController1 : MonoBehaviour
{
    //[SerializeField] private Transform player; // get the player object

    [Header("Patrol points")]
    [SerializeField] private List<Vector2Int> patrolPoints = new List<Vector2Int>(); // patrol points

    private int patrolIndex = 0; // where at array
    private int alterCounter = 0; // how many turns enemy stays altered

    private EnemyInfo enemyInfo; // enmey's status
    private EnemyPathFinder pathFinder; // finding the enemy path
    private EnemyMovement movement; // how it moves
    private EnemyAttackCore enemyAttk; // access the enemy attk script
    private EnemyState enemyState; // state control for Enemy AI
    //private EnemyTileScanner scanner; // scanns the neighbour tiles for pathfinding

    private CharacterInfo1 player; // access player info

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

        enemyAttk = GetComponentInChildren<EnemyAttackCore>(); // set up enemy attack

        // check to see if the set up is working
        if (enemyAttk == null)
            Debug.Log($"{name} No EnemyAttackCore found! cannont attack!"); // debug msg

        //player = CharacterInfo1.Instance; // cpoies player info
    }

    private void Start()
    {
        Debug.Log("EnemyController start - currentTile = " + enemyInfo.currentTile); // debug

        enemyState = EnemyState.Patrol; // start off by patrolling
    }

    // using finite states to control the enemy ai
    public IEnumerator TakeTurn()
    {
        SetPlayerInfo(); // set up player info

        // before moving check if player is in range if so attack (attack state)
        //if (PlayerInAttRangeCheck())
        //{
        //    Debug.Log($"In enemy range! {name} attacks the player!"); // debug

        //    //AttackPlayer(); // if player in range attack them
        //    yield break; // get out, not moving this turn
        //}

        Debug.Log($"{name} " +
            $"EnemyAttkscript null:{(enemyAttk == null)} " +
            $"attkEnemyInfo null:{(enemyInfo == null)} " +
            $"enemyTile null:{(enemyInfo.currentTile == null)} " +
            $"player null:{(player == null)} " +
            $"playerTile null:{(player.CurrentTile == null)}"); // debug msg

        // check if player is in enemy attack range if so, attack
        if (enemyAttk != null && enemyAttk.CanAttackPlayer(player))
        {
            //var player = GetPlayer(); // set up player

            if (player != null || player.CurrentTile != null)
            {
                Debug.Log($"Player In Range:{name} attck player!"); // debug msg

                //enemyAttk.AttackPlayer(player);

                AttackQeue(player);
                yield break;
            }
        }

        // player detected ! change state (detect state)
        if (PlayerDetectRange())
        {
            // only reset if enemy is not in alter state
            if (enemyState != EnemyState.Alter)
            {
                enemyState = EnemyState.Alter; // player detect now shift to alter mode

                alterCounter = 0; // resets the alterCounts, if it reaches 2 back to patrol
            }
            else
            {
                yield return MoveTowardPlayer(); // calls moving twoards player function
                yield break; // get out
            }
        }

        // after player detected change to alter (alter state)
        if (enemyState == EnemyState.Alter)
        {
            alterCounter++; // add one to the counter

            // if alter counter is 2 or greater back to patrol, lost player track in 2 turns
            if (alterCounter >= 2)
                enemyState = EnemyState.Patrol; // back to patrol 
            else
            {
                yield return MoveTowardPlayer(); // keep chasing the player, alter mode
                yield break; // still in alter mode but don't move get out
            }
        }

        //// enemy is alter mode and player not found (alter mode 2)
        //else if (enemyState == EnemyState.Alter && !PlayerDetectRange())
        //{
        //    Debug.Log($"{name}: Player got away and not deteccted, back to patrol"); // debug

        //    enemyState = EnemyState.Alter; // change to patrol
        //}

        // player not found or player escaped back to patrol (patrol state)
        else if (enemyState == EnemyState.Patrol)
        {
            yield return PatrolEnemyMovement(); // back to the patrol movment/continues the patrol
            yield break;
        }
    }

    private void UpdateIndex()
    {
        if (movingForward) // flag to check which way enemy is going A->B->C or C->B->A
        {
            patrolIndex++; // move to next indext patrol position

            if (patrolIndex >= patrolPoints.Count) // check to see if enemy is doing this C->B->A reversing patrol
            {
                patrolIndex = patrolPoints.Count - 2; // if index goes over or equal,example 3 patrol points and we counted total of 3 counts, 3 - 2, we are back to p[1] , C->B

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

    private bool PlayerDetectRange()
{
    CharacterInfo1 player = CharacterInfo1.Instance; // setup the prefab character

    if (player == null || player.CurrentTile == null) return false; // if player or the tile is not found return false

    int distance = Mathf.Abs(enemyInfo.currentTile.gridLocation.x - player.CurrentTile.gridLocation.x) // mahattant math to see how close is the player 
                + Mathf.Abs(enemyInfo.currentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

    return distance <= enemyInfo.EnemyDetect; // return the detection range
}

private IEnumerator MoveTowardPlayer()
    {
        //OverlayTile playerTile = CharacterInfo.Instance.CurrentTile; // get the player tile info

        OverlayTile1 chaseTargetPlayer = GetClosestAdjacentTileToPlayer(); // find the closest adjacent tile but not player's tile

        // no adjacent tile found
        if (chaseTargetPlayer == null)
        {
            Debug.Log($"{name}: No valid adjacent tiles towards player, skipping movement!"); // debug

            enemyState = EnemyState.Alter; // stays in alter state
            yield break; // tile not found get out
        }

        // if adjacent tile is equal to the enemy tile
        if (chaseTargetPlayer == enemyInfo.currentTile)
        {
            Debug.Log($"{name}: Already at the adjacent tile, keep on chasing next turn."); // debug

            enemyState = EnemyState.Alter; // stays in alert mode
            yield break;
        }

        List<OverlayTile1> findPath = pathFinder.FindPath(enemyInfo.currentTile, chaseTargetPlayer); // find the adjacent path bewteen player/enemy

        if (findPath.Count <= 1)  // skip if it's one because player is next to nemey
        {
            Debug.Log($"{name}: Path less than attackRange, don't chase attack!"); // debug

            enemyState = EnemyState.Alter; // stays in alter mode
            yield break;
        }

        List<OverlayTile1> moveSteps = findPath.Skip(1).Take(enemyInfo.moveRange).ToList(); // if it matches current tile, go to next tile

        yield return movement.MoveAlong(moveSteps); // move correct steps to moveAlong and distance

        enemyState = EnemyState.Alter; // still in alter mode after move
    }

    private IEnumerator PatrolEnemyMovement()
    {
        // if enemy tile is null display a debug
        if (enemyInfo.currentTile == null)
        {
            Debug.LogError($" {name} has no currentTile, skipping turn.");
            yield break;
        }

        //Debug.Log("Enemy current tile = " + enemyInfo.currentTile.gridLocation); debug

        // if patrolPoints doesn't exisit display debug msg
        else if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning($"{name} has no patrol points, skipping turn");
            yield break;
        }

        Vector2Int targetGrid = patrolPoints[patrolIndex]; // find the next patrol point

        Debug.Log($"{name} Patrol target = {targetGrid}"); // debug msg

        OverlayTile1 targetTile = MapManager1.Instance.GetTile(targetGrid); // get the tile from map Manager

        // if target tile and enemy current tile both are found and they are equal update the index
        if (targetTile != null && enemyInfo.currentTile != null && enemyInfo.currentTile.gridLocation == targetTile.gridLocation)
        {
            UpdateIndex(); // index update
            yield break;
        }

        if (targetTile == null) // if next tile not found display error
        {
            Debug.LogError($"{name} No tile at Patrol target {targetGrid}"); // debug msg
            yield break;
        }

        Debug.Log($"{name} current tile = {enemyInfo.currentTile.gridLocation}"); // debug msg
        Debug.Log("Calling pathfinder..."); // debug msg

        if (targetTile.hasPlayer)
        {
            Debug.Log($"{name}: Player is in the way of patrol point -> switch to alter state!"); // debug

            enemyState = EnemyState.Alter; // stay in alter state if the next targeTile has player

            alterCounter = 0; // reset counter

            //yield return MoveTowardPlayer(); // chase the player
            yield break;
        }

        List<OverlayTile1> path = pathFinder.FindPath(enemyInfo.currentTile, targetTile); // current tile to next tile

        if (path.Count < 2)
        {
            Debug.LogWarning("Path empty, enemy not moving this turn"); // debug msg
            yield break;
        }

        // current skip 0 -> start from 1
        yield return movement.MoveAlong(path.Skip(1).ToList()); // if path count > 0 delay return, got through the patrol points, skips whole path

        if (enemyInfo.currentTile.gridLocation == targetTile.gridLocation) // if we reach the tile update it
            UpdateIndex(); // updates the index
    }

    private OverlayTile1 GetClosestAdjacentTileToPlayer()
    {
        CharacterInfo1 player = CharacterInfo1.Instance; // get player info

        // if player || the player tile not found retuns null
        if (player == null || player.CurrentTile == null)
            return null;

        Vector2Int playerPositions = new Vector2Int(player.CurrentTile.gridLocation.x, // only use vectect2Int not Vector3Int convert to x,y
            player.CurrentTile.gridLocation.y);

        Vector2Int[] adjacentOffsets = new Vector2Int[] // the four adjacent directions
        {
            new Vector2Int(1, 0), // x
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1), // y
            new Vector2Int(0, -1),
        };

        OverlayTile1 bestTile = null; // to store the best tile to stand

        float bestDistance = float.MaxValue; // no limits for now 

        // use loop to run through the offsets to find the best one
        foreach (var offsets in adjacentOffsets)
        {
            Vector2Int gridPosition = playerPositions + offsets; // add the offset to the player position

            OverlayTile1 tile = MapManager1.Instance.GetTile(gridPosition); // access the tile from map

            if (tile == null) continue; // keeps going even if the tile not found skip

            else if (tile.isBlocked) continue; // keeps going if the tile isBlocked (has enemy/player) skip

            else if (tile.hasEnemy) continue; // keeps going if enemy is using the tile skip

            else if (tile.hasPlayer) continue; // keeps going if player using the tile skip

            //return tile; // returns the first valid adjacent tile

            float distance = Vector2.Distance(tile.transform.position, enemyInfo.currentTile.transform.position); // store that variable 

            // if the new distance better than the distance we have replace it
            if (distance < bestDistance)
            {
                bestDistance = distance; // replace the position

                bestTile = tile; // store that tile's position
            }
        }

        return bestTile; // returns the closest adjacent tile  
    }

    public void SetPatrolPoints(List<Vector2Int> pPoints, int startIndex = 0)
    {
        patrolPoints = new List<Vector2Int>(pPoints); // clone the patrol points for individual enemy

        patrolIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, patrolPoints.Count - 1)); // the patrol range is bewteen 0 - max count

        movingForward = true; // direction of movement

        // enemyInfo and enemy current tile exist and patrolPoints is vaild, change the starting tile to the next index
        if (enemyInfo != null && enemyInfo.currentTile != null && patrolPoints.Count > 0)
        {
            Vector2Int currentTile = new Vector2Int(enemyInfo.currentTile.gridLocation.x, enemyInfo.currentTile.gridLocation.y); // set current tile based on enemy current position x,y

            // current patrol point is equal to the tile and patrol points are more than one go to next point
            if (patrolPoints[patrolIndex] == currentTile && patrolPoints.Count > 1)
                patrolIndex = (patrolIndex + 1) % patrolPoints.Count; // update the indext to next patrol point
        }
    }

    private void SetPlayerInfo()
    {
        // if player is null set up
        if (player == null)
            player = CharacterInfo1.Instance; // copies the playerInfo

        // if player still null try findthe name tag
        if (player == null)
        {
            // ======= if playerInfo still null ========
            var playerObj = GameObject.FindGameObjectWithTag("Player1"); // find it using game object tag

            // if found set up the basic stats for player1
            if (playerObj != null)
                player = playerObj.GetComponent<CharacterInfo1>(); // set up the playerInfo using object
        }

        Debug.Log($"player null?{player == null} playerTile null?{(player != null ? player.CurrentTile == null : true)}"); // debug msg
    }

    //private CharacterInfo1 GetPlayer()
    //{
    //    // finds the player info when needed 
    //    return CharacterInfo1.Instance != null ? CharacterInfo1.Instance : GameObject.FindGameObjectWithTag("Player1")?.GetComponent<CharacterInfo1>(); 
    //}

    private void AttackQeue(CharacterInfo1 player)
    {
        // if player null get out
        if (player == null) return;

        Debug.Log($"[Enemy]:{name} Attack Qeue! player:{player?.name} dmg:{enemyInfo.EnemyDmg}"); // debug msg

        //// states for calculation
        //int enemyHitRate = enemyInfo.EnemyHitRate; // set up the enemy hit rate

        //int dmg = enemyInfo.EnemyDmg; // set up enemy dmg

        //int playerEvasion = player.BaseEvasion; // set up the player evasion rate

        int hitChance = HitRollCheck.FinalHitChanceCal(enemyInfo.EnemyHitRate, 0, player.BaseEvasion);

        TurnManager.Instance.StartPlayerReaction(enemyInfo, enemyInfo.EnemyDmg, hitChance); // copies over the enemy/player data
    }

    private void OnDestroy()
    {
        // deltes the destoryed enemy
        if (TurnManager.Instance != null)
            TurnManager.Instance.DeleteEnmey(this);

        // Added by Warren, stops all running coroutines when enemies are destroyed, bug should be fixed.
        StopAllCoroutines();
    }
    
}

// not in use anymore 
//public void AttackPlayer()
//{
//    CharacterInfo1 playerInfo = CharacterInfo1.Instance; // setup the player, now it can access all the info

//    // if player not found get out 
//    if (playerInfo == null)
//    {
//        Debug.LogError("playerInfo not found!"); // debug
//        return;
//    }
//    else //if (playerInfo != null) // if player found damage them 
//        playerInfo.PlayerTakeDamage(enemyInfo.EnemyDmg);

//    Debug.Log($"{name} attacked the player for {enemyInfo.EnemyDmg} damage! Player current HP: {playerInfo.CurrentHP}"); // debug
//}

// replaced by enemy attack scripts
//private bool PlayerInAttRangeCheck()
//{
//    //OverlayTile playerTile = CharacterInfo.Instance.CurrentTile; // get the player's tile

//    CharacterInfo1 player = CharacterInfo1.Instance; // setup the prefab character

//    //OverlayTile enemyTile = enemyInfo.currentTile;

//    //if (playerTile == null || enemyTile == null) return; // if player or enemy not tile not found get out

//    if (player == null || player.CurrentTile == null) return false; // if player or the tile is not found return false

//    int distance = Mathf.Abs(enemyInfo.currentTile.gridLocation.x - player.CurrentTile.gridLocation.x) // mahattant math to see how close is the player 
//                + Mathf.Abs(enemyInfo.currentTile.gridLocation.y - player.CurrentTile.gridLocation.y);

//    Debug.Log($"Enemy distance to play = {distance}"); // debug

//    // if player is in range attack
//    //if (distance <= enemyInfo.attackRange)
//    //{
//    //    Debug.Log($"In enemy range! {name} attacks the player!"); // debug

//    //    AttackPlayer(); // if player in range attack them
//    //}

//    return distance <= enemyInfo.attackRange; // if palyer exsit return the distance not T/F
//}

// Old enemy movement control version 
//    // if enemy tile is null display a debug
//    if (enemyInfo.currentTile == null)
//    {
//        Debug.LogError($" {name} has no currentTile, skipping turn.");
//        yield break;
//    }

//    //Debug.Log("Enemy current tile = " + enemyInfo.currentTile.gridLocation); debug

//    // if patrolPoints doesn't exisit display debug msg
//    if (patrolPoints == null || patrolPoints.Count == 0)
//    {
//        Debug.LogWarning($"{name} has no patrol points, skipping turn");
//        yield break;
//    }

//    Vector2Int targetGrid = patrolPoints[patrolIndex]; // find the next patrol point

//    //patrolIndex = (patrolIndex + 1) % patrolPoints.Count; // index calculation for example 0, 1 % patrolPoints = 1, 2%3 = 2, 3%3 = 0; 0 -> 1 ->2

//    Debug.Log($"{name} Patrol target = {targetGrid}"); // debug msg

//    OverlayTile targetTile = MapManager.Instance.GetTile(targetGrid); // get the tile from map Manager

//    if (targetTile == null) // if next tile not found display error
//    {
//        Debug.LogError($"{name} No tile at Patrol target {targetGrid}"); // debug msg
//        yield break;
//    }

//    Debug.Log($"{name} current tile = {enemyInfo.currentTile.gridLocation}"); // debug msg
//    Debug.Log("Calling pathfinder..."); // debug msg

//    List<OverlayTile> path = pathFinder.FindPath(enemyInfo.currentTile, targetTile); // current tile to next tile

//    if (path.Count <= 1)
//    {
//        Debug.LogWarning("Path empty, enemy not moving this turn"); // debug msg
//        yield break;
//    }

//    // if the path is not empty display it
//    //if (path.Count > 0)

//    //List<OverlayTile> trimmedPath = path.GetRange(1, path.Count - 1); // only go from 1 -> next

//    // current skip 0 -> start from 1
//    yield return movement.MoveAlong(path.Skip(1).ToList()); // if path count > 0 delay return, got through the patrol points, skips whole path

//    // after moving check if player in range if so attack
//    if (PlayerInAttRangeCheck())
//    {
//        Debug.Log($"In enemy range! {name} attacks the player!"); // debug

//        AttackPlayer(); // if player in range attack them
//        yield break; // get out, not moving this turn
//    }

//    if (enemyInfo.currentTile == targetTile) // if we reach the tile update it
//        UpdateIndex(); // updates the index

//    //if (!isReady)
//    //{
//    //    Debug.LogWarning("Enemy not ready yet - skipping turn");

//    //    yield break;
//    //}

//    //Debug.Log($"{name} is taking its turn.");

//    ////yield return new WaitForSeconds(0.5f); // wait a little before returns

//    //if (enemyPatrol != null) // if enemyPatrol exist do the patrol
//    //{
//    //    yield return StartCoroutine(enemyPatrol.EnemyPatrolStep());
//    //}
//    ////yield return null;

//    ////TurnManager.Instance.EndEnemyTurn(); // end enemy's Turn
//}

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
