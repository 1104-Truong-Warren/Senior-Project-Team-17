// Modified version of MouseController1.cs with isolated functionality for tutorial use
// Copies a lot of logic from EnemySpawner.cs too since player is treated like an enemy for parts of initialization
// Ellison
using System.Collections;
using System.Collections.Generic; // for the List<T> and dictionary <T, T> for pathfinding
using System.Linq; // filter numbers that are greater than 10, x=> x.F is using it, ordering etc...
using UnityEngine; // default
using UnityEngine.EventSystems; // for stuff like 2d/3d raycasting, for mouse to find the correct tiles
using UnityEngine.UI; // for the UI stuff, menu and such

public class TutorialMouseController : MonoBehaviour
{
    [Header("Settings for some stuff")]
    [SerializeField] private LayerMask enemyLayer; // enemy's layer for detection
    [SerializeField] private PlayerAction currentAction; // access what kind of skill player is on
    [SerializeField] private GameObject cursor; // for our curosr
    [SerializeField] private GameObject characterPrefab; // object for the character prefab
    [SerializeField] private CharacterInfo1 characterInfo; // stores the characgter info
    [SerializeField] private float speed; // move speed for character
    [SerializeField] public Vector2Int spawnGridPosition; // predetermined player spawn point

    [SerializeField] public LayerMask tileLayer;

    private OverlayTile1 previouslySelectedTile; // previous tile
    private PathFinder1 pathFinder; // access the pathfinder
    private List<OverlayTile1> path;
    public bool movementEnabled = false;
    //public Animator menuAnimator;
    //public Button collapseButton;

    public TutorialActionMenu actionMenu;
    public TutorialManager tutorialManager;

    [Header("Tutorial Milestone Tile Positions")]
    [SerializeField] public Vector2Int spot1aTilePosition;
    [SerializeField] public Vector2Int spot1bTilePosition; 
    [SerializeField] public Vector2Int spot2aTilePosition;
    [SerializeField] public Vector2Int spot2bTilePosition;
    [SerializeField] public Vector2Int spot3aTilePosition;
    [SerializeField] public Vector2Int spot3bTilePosition;
    [SerializeField] public Vector2Int combat1spotTilePosition;
    [SerializeField] public Vector2Int transition1aTilePosition;
    [SerializeField] public Vector2Int transition1bTilePosition;
    [SerializeField] public Vector2Int transition2aTilePosition;
    [SerializeField] public Vector2Int transition2bTilePosition;
    [SerializeField] public Vector2Int transition3aTilePosition;
    [SerializeField] public Vector2Int transition3bTilePosition;

    public float blinkSpeed = 0.5f;
    public OverlayTile1 currentMilestoneTile;
    private Coroutine blinkCoroutine;

    public bool movedSpot1a = false;
    public bool movedSpot1b = false;
    public bool movedSpot2a = false;
    public bool movedSpot2b = false;
    public bool movedSpot3a = false;
    public bool movedSpot3b = false;

    private bool isMoving = false; // track if movement is in progress

    [Header("Combat1 Step")]
    public Vector2Int enemySpot1Position;
    public bool movedCombat1Spot = false;
    public bool attackCombat1Prepare = false;
    public bool confirmedCombat1Attack = false;

    [Header("MoveToNext Step")]
    public bool movedTransition1a = false;
    public bool movedTransition1b = false;
    public bool movedTransition2a = false;
    public bool movedTransition2b = false;
    public bool movedTransition3a = false;
    public bool movedTransition3b = false;

    private IEnumerator Start()
    {
        pathFinder = new PathFinder1(); // create it
        path = new List<OverlayTile1>();

        // TUTORIAL SPECIFIC:
        // Wait until the map is fully setup before spawning the player
        yield return new WaitUntil(() => MapManager1.Instance != null && 
            MapManager1.Instance.map != null &&
            MapManager1.Instance.map.Count > 0);

        SpawnCharacterAtTile();

        // Start blinking for the first milestone after setup
        //yield return new WaitForSeconds(1f);
        //StartMilestoneBlinking(spot1aTilePosition);
    }

    private void SpawnCharacterAtTile()
    {
        // Instantiate player at set position and put on line
        characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo1>(); // copy character info from character1
        characterInfo.GetComponent<PlayerTargetSelect>().enabled = false; // disable the target select script on the player for tutorial purposes
        OverlayTile1 tile = MapManager1.Instance.GetTile(spawnGridPosition); // get the tile info for the spawn point
        if (tile != null)
        {
            PositionCharacterOnLine(tile);
        }
        else
        {
            Debug.LogError("Spawn tile not found at position: " + spawnGridPosition);
        }
    }

    private void LateUpdate()
    {
        // Combat Tutorial Logic (Simulated)
        if (tutorialManager.currentStep == TutorialStep.Combat1)
        {
            // Get the tile where the enemy is standing
            OverlayTile1 enemyTile = MapManager1.Instance.GetTile(enemySpot1Position);

            if (enemyTile != null)
            {
                // 1. Mimic 'A' to Lock-On (Turn Tile Red)
                if (Input.GetKeyDown(KeyCode.A))
                {
                    enemyTile.ShowEnemyTile(); // This turns the tile red/orange
                    //if (cursor != null) cursor.SetActive(false); // Hide mouse cursor to mimic lock-on
                    Debug.Log("Tutorial: Simulated Lock-on");
                    attackCombat1Prepare = true;
                }

                // 2. Mimic 'S' to Cancel (Hide Tile)
                if (Input.GetKeyDown(KeyCode.S))
                {
                    enemyTile.HideTile();
                    //if (cursor != null) cursor.SetActive(true); // Bring mouse cursor back
                    Debug.Log("Tutorial: Simulated Cancel");
                    attackCombat1Prepare = false;
                }

                // 3. Mimic 'F' to Confirm Attack
                if (Input.GetKeyDown(KeyCode.F))
                {
                    // Only count the attack if the tile is currently "highlighted" (mimicking a lock-on)
                    // You can check the sprite color alpha or just assume if they press F they meant it
                    enemyTile.HideTile();
                    confirmedCombat1Attack = true;
                    //if (cursor != null) cursor.SetActive(true);
                    Debug.Log("Tutorial: Simulated Attack Confirmed");
                }
            }

            // If we are "mimicking" targeting, we should block the normal mouse logic
            // We can check if the enemy tile is visible to determine this
            if (enemyTile != null && enemyTile.GetComponent<SpriteRenderer>().color.a > 0)
            {
                return;
            }
        }

        if (!IsPointerOverUIObject())
        {
            var hit = GetFocusedOnTile(); // reference
            if (hit.HasValue)
            {
                if (cursor != null)
                {
                    OverlayTile1 tile = hit.Value.collider.gameObject.GetComponent<OverlayTile1>(); // which tile to spawn                                                                        // get out if tile not found
                    if (tile == null)
                        return;
                    cursor.transform.position = tile.transform.position; // set cursor location to the overlay
                    cursor.GetComponent<SpriteRenderer>().sortingOrder = 9999;


                    if (Input.GetMouseButtonDown(0) && !isMoving) // only allow new movement input if not currently moving
                    {
                        Vector2 world = Camera.main.ScreenToWorldPoint(Input.mousePosition); // get the input position of mouse
                        Debug.Log("Pointer is over UI: " + MouseController1.IsPointerOverUIObject()); // debug msg

                        // Clear previous path highlight
                        foreach (var t in path)
                            t.HideTile();

                        path.Clear();

                        if (previouslySelectedTile != null)  // hides the previous selected tiles
                            previouslySelectedTile.HideTile();
                        MapManager1.Instance.ResetAllTiles(); // before showing tiles reset all
                        tile.ShowPlayerTile();
                        previouslySelectedTile = tile; // shows current tile and save it

                        // if the character movement is enabled
                        if (actionMenu.movementEnabled)
                        {
                            if (characterInfo == null)
                            {
                                //characterInfo = new CharacterInfo(); // declare it again 

                                characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo1>(); // get the prefab assign
                                PositionCharacterOnLine(tile);

                                //PositionCharacterOnLine(overlayTile.GetComponent<OverlayTile>()); // spawn the character

                                characterInfo.PlayerSetTile(tile);
                            }
                            else
                            {
                                if (tile.isBlocked || tile.hasEnemy || tile.hasPlayer) // if tile has enemy/player/blocked get out
                                {
                                    Debug.Log("Tile is being used!"); // debug

                                    return;
                                }

                                // Check if the clicked tile is the current milestone target
                                if (!IsTargetMilestone(tile))
                                {
                                    Debug.Log("Tutorial: Can only move to the milestone tile!");
                                    return;
                                }

                                int playerMoveteps = characterInfo.GetMoveRange(); // set the move range for player 

                                int distance = pathFinder.GetManhattenDistance(characterInfo.CurrentTile, tile); // find the distance between our current and target tile

                                // path steps > actually movement that's not in the range
                                if (distance > playerMoveteps)
                                {
                                    Debug.Log("Out of bound! Moved Too far or Not moving!"); // debug
                                    return;
                                }

                                path = pathFinder.FindPath(characterInfo.CurrentTile, tile); //(characterInfo.standingOnTile, overlayTile.GetComponent<OverlayTile>()); // if is not out of range player can go there

                                //tile.gameObject.GetComponent<OverlayTile>().HideTile(); // hides the tile

                                //Show the path tiles (highlight)
                                foreach (var t in path)
                                    t.ShowPlayerTile();

                                // Only start movement if path is valid
                                if (path.Count > 0)
                                {
                                    isMoving = true;
                                    actionMenu.BeginMovementProcess(); // Call once when movement starts
                                }
                            }
                        }

                    }
                }
            }
            /*else
            {
                if (cursor != null)
                {
                    cursor.SetActive(false); // hide cursor if not hovering over a tile
                }
            }*/

            
        }

        // Move along path if there are tiles to move to
        if (path.Count > 0 && isMoving)
        {
            MoveAlongPath();
        }

    }

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime; // how fast character moves

        float zIndex = path[0].transform.position.z + 0.01f; // z position

        characterInfo.transform.position = Vector2.MoveTowards(characterInfo.transform.position, // move to
            path[0].transform.position, step);

        characterInfo.transform.position = new Vector3(characterInfo.transform.position.x, // from
            characterInfo.transform.position.y, zIndex);

        // movement finished
        if (Vector2.Distance(characterInfo.transform.position, path[0].transform.position) < 0.00001f)
        {
            PositionCharacterOnLine(path[0]); // calculate postition on line

            if (path[0].hasItem) // if current tile has item trigger the pickup
            {
                path[0].PickUpItem(); // call the pickup function
                Debug.Log("Player picked up an item!"); // debug msg
            }

            path.RemoveAt(0); // remove after

            // finishes moving Turn starts
            if (path.Count == 0)
            {
                //characterInfo.ApUsed(1); // used 1 AP after moved

                //TurnManager.Instance.PlayerSpendAP(1); // checks for the AP each turn, 1 AP per movement

                // Ellison - disable movement after moving, allow action menu to pop back up, re-enable collapse button
                isMoving = false; // mark movement as complete
                actionMenu.EndMovementProcess();
            }
        }
    }

    private void PositionCharacterOnLine(OverlayTile1 tile)
    {
        if (characterInfo == null) // characterInfo not found go through the characterPrefab and use that info
        {
            characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo1>();
            PositionCharacterOnLine(tile); // current position
            return;
        }

        else if (characterInfo.CurrentTile != null) // reset the old tile, reset flag
            characterInfo.CurrentTile.hasPlayer = false;

        tile.hasPlayer = true; // triggers the flag has player
        characterInfo.PlayerSetTile(tile); // update player's tile info

        // Check if milestone was reached
        CheckMilestoneReached(tile);

        // offset the y-axis a little bit
        characterInfo.transform.position = new Vector3(tile.transform.position.x,
            tile.transform.position.y + 0.1f,
            tile.transform.position.z
        );  // store the postion

        characterInfo.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
    }

    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 mousePosition2d = new Vector2(mousePosition.x, mousePosition.y); // get the 2d position

        RaycastHit2D[] hit = Physics2D.RaycastAll(mousePosition2d, Vector2.zero, 0f, tileLayer); // raycast is like a imaginary line to find what's infront 

        //Debug.DrawRay(mousePosition2d, Vector2.zero, Color.red, 0.2f);
        //Debug.Log($"All Hits: {hit.Length}");

        if (hit.Length > 0)
        {
            return hit.OrderByDescending(i => i.collider.transform.position.z).First(); // return whatever hits first 
        }

        return null;
    }

    // from https://discussions.unity.com/t/detect-if-pointer-is-over-any-ui-element/138619
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void CheckMilestoneReached(OverlayTile1 tile)
    {
        // Check if player reached spot 1a
        if (!movedSpot1a && tile.gridLocation.x == spot1aTilePosition.x &&
            tile.gridLocation.y == spot1aTilePosition.y)
        {
            movedSpot1a = true;
            Debug.Log("Player reached Spot 1A!");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(spot1bTilePosition);
        }

        // Check if player reached spot 1b
        if (!movedSpot1b && tile.gridLocation.x == spot1bTilePosition.x &&
            tile.gridLocation.y == spot1bTilePosition.y)
        {
            movedSpot1b = true;
            Debug.Log("Player reached Spot 1B!");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(spot2aTilePosition);
        }

        // Check if player reached spot 2a
        if (!movedSpot2a && tile.gridLocation.x == spot2aTilePosition.x &&
            tile.gridLocation.y == spot2aTilePosition.y)
        {
            movedSpot2a = true;
            Debug.Log("Player reached Spot 2A!");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(spot2bTilePosition);
        }

        // Check if player reached spot 2b
        if (!movedSpot2b && tile.gridLocation.x == spot2bTilePosition.x &&
            tile.gridLocation.y == spot2bTilePosition.y)
        {
            movedSpot2b = true;
            Debug.Log("Player reached Spot 2B!");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(spot3aTilePosition);
        }

        // Check if player reached spot 3a
        if (!movedSpot3a && tile.gridLocation.x == spot3aTilePosition.x &&
            tile.gridLocation.y == spot3aTilePosition.y)
        {
            movedSpot3a = true;
            Debug.Log("Player reached Spot 3A!");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(spot3bTilePosition);
        }

        // Check if player reached spot 3b
        if (!movedSpot3b && tile.gridLocation.x == spot3bTilePosition.x &&
            tile.gridLocation.y == spot3bTilePosition.y)
        {
            movedSpot3b = true;
            Debug.Log("Player reached Spot 3B! Tutorial movement complete!");
            StopMilestoneBlinking();
            tile.HideTile();
            // no more blinking
        }


        // Later, check if player reached combat1 spot
        if (!movedCombat1Spot && tile.gridLocation.x == combat1spotTilePosition.x &&
            tile.gridLocation.y == combat1spotTilePosition.y)
        {
            movedCombat1Spot = true;
            Debug.Log("Player reached Combat 1 Spot! Tutorial combat movement complete!");
            StopMilestoneBlinking();
            tile.HideTile();
            // no more blinking
        }

        
        // later, in transition step
        if (!movedTransition1a && tile.gridLocation.x == transition1aTilePosition.x &&
            tile.gridLocation.y == transition1aTilePosition.y && movedCombat1Spot)
        {
            movedTransition1a = true;
            Debug.Log("Player reached Transition1a.");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(transition1bTilePosition);
        }
        if (!movedTransition1b && tile.gridLocation.x == transition1bTilePosition.x &&
            tile.gridLocation.y == transition1bTilePosition.y)
        {
            movedTransition1b = true;
            Debug.Log("Player reached Transition1b.");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(transition2aTilePosition);
        }
        if (!movedTransition2a && tile.gridLocation.x == transition2aTilePosition.x &&
            tile.gridLocation.y == transition2aTilePosition.y)
        {
            movedTransition2a = true;
            Debug.Log("Player reached Transition2a.");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(transition2bTilePosition);
        }
        if (!movedTransition2b && tile.gridLocation.x == transition2bTilePosition.x &&
            tile.gridLocation.y == transition2bTilePosition.y)
        {
            movedTransition2b = true;
            Debug.Log("Player reached Transition2b.");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(transition3aTilePosition);
        }
        if (!movedTransition3a && tile.gridLocation.x == transition3aTilePosition.x &&
            tile.gridLocation.y == transition3aTilePosition.y)
        {
            movedTransition3a = true;
            Debug.Log("Player reached Transition3a.");
            StopMilestoneBlinking();
            tile.HideTile();
            StartMilestoneBlinking(transition3bTilePosition);
        }
        if (!movedTransition3b && tile.gridLocation.x == transition3bTilePosition.x &&
            tile.gridLocation.y == transition3bTilePosition.y)
        {
            movedTransition3b = true;
            Debug.Log("Player reached Transition3b. Done.");
            StopMilestoneBlinking();
            tile.HideTile();
        }
    }

    public Vector2Int GetCurrentMilestoneTarget()
    {
        if (!movedSpot1a)
            return spot1aTilePosition;
        else if (!movedSpot1b)
            return spot1bTilePosition;
        else if (!movedSpot2a)
            return spot2aTilePosition;
        else if (!movedSpot2b)
            return spot2bTilePosition;
        else if (!movedSpot3a)
            return spot3aTilePosition;
        else if (!movedSpot3b)
            return spot3bTilePosition;
        else if (!movedCombat1Spot)
            return combat1spotTilePosition;
        else if (!movedTransition1a)
            return transition1aTilePosition;
        else if (!movedTransition1b)
            return transition1bTilePosition;
        else if (!movedTransition2a)
            return transition2aTilePosition;
        else if (!movedTransition2b)
            return transition2bTilePosition;
        else if (!movedTransition3a)
            return transition3aTilePosition;
        else if (!movedTransition3b)
            return transition3bTilePosition;

            // all milestones reached, no valid target
            return Vector2Int.zero;
    }

   
    private bool IsTargetMilestone(OverlayTile1 tile)
    {
        Vector2Int targetMilestone = GetCurrentMilestoneTarget();

        Debug.Log($"Clicked: {tile.gridLocation} | Target: {targetMilestone}");

        // If no valid milestone target, allow movement (tutorial may be complete)
        if (targetMilestone == Vector2Int.zero)
            return true;

        // Check if the clicked tile matches the target milestone
        return tile.gridLocation.x == targetMilestone.x && 
               tile.gridLocation.y == targetMilestone.y;
    }

    // meant to be called from tutorial manager once at player movement step
    public void BeginBlinkingSequence()
    {
        StartMilestoneBlinking(spot1aTilePosition);
    }

    public void BeginSecondBlinkingSequence()
    {
        StartMilestoneBlinking(transition1aTilePosition);
    }

    public void StartMilestoneBlinking(Vector2Int tilePosition)
    {
        // Get the tile at the position
        OverlayTile1 tile = MapManager1.Instance.GetTile(tilePosition);
        
        if (tile == null)
        {
            Debug.LogWarning($"Milestone tile not found at position: {tilePosition}");
            return;
        }

        // Stop any existing blink coroutine
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }

        currentMilestoneTile = tile;
        blinkCoroutine = StartCoroutine(BlinkTileCoroutine(tile));
    }

    private void StopMilestoneBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (currentMilestoneTile != null)
        {
            currentMilestoneTile.HideTile();
            currentMilestoneTile = null;
        }
    }

    private IEnumerator BlinkTileCoroutine(OverlayTile1 tile)
    {
        while (tile != null)
        {
            // Show the tile
            tile.ShowPlayerTile();
            yield return new WaitForSeconds(blinkSpeed);

            // Hide the tile
            tile.HideTile();
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
}
