// These are based on this channel on YouTube: https://www.youtube.com/@lawlessgames3844
// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// Weijun, Ellison

using System.Collections.Generic; // for the List<T> and dictionary <T, T> for pathfinding
using System.Linq; // filter numbers that are greater than 10, x=> x.F is using it, ordering etc...
using System.Runtime.InteropServices;
using Unity.VisualScripting; // not sure what this is just shows up built in feature for visual graphs, state graphs
using UnityEngine; // default
using UnityEngine.EventSystems; // for stuff like 2d/3d raycasting, for mouse to find the correct tiles
using UnityEngine.UI; // for the UI stuff, menu and such

public class MouseController1 : MonoBehaviour
{
    [Header("Settings for some stuff")]
    [SerializeField] private LayerMask enemyLayer; // enemy's layer for detection
    [SerializeField] private PlayerAction currentAction; // access what kind of skill player is on
    [SerializeField] private GameObject cursor; // for our curosr
    [SerializeField] private GameObject characterPrefab; // object for the character prefab
    [SerializeField] private CharacterInfo1 characterInfo; // stores the characgter info
    [SerializeField] private float speed; // move speed for character

    private OverlayTile1 previouslySelectedTile; // previous tile

    private PathFinder1 pathFinder; // access the pathfinder

    private List<OverlayTile1> path;


    // Ellison - Added bool to enable or disable movement (disabled by default)
    public bool movementEnabled = false;

    // Ellison - Reference to Action Menu's Animator for opening and closing
    public Animator menuAnimator;

    // Ellison - Reference to collapse button to disable it when movement is enabled
    public Button collapseButton;

    private void Start()
    {
        pathFinder = new PathFinder1(); // create it

        path = new List<OverlayTile1>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (TurnManager.Instance.State != TurnState.PlayerAction && TurnManager.Instance.State != TurnState.PlayerSpawn) return; // preventing player moving before other things are setup

        //// mouse test for enemy collider
        //Vector3 mPosition = Input.mousePosition; // set up the mousse position

        // Debugs for display
        //mPosition.z = Mathf.Abs(Camera.main.transform.position.z); // make sure z is not affected to get the correct input

        //Vector3 MPWorld = Camera.main.ScreenToWorldPoint(mPosition); // mouse position in the game world

        //Debug.DrawLine(MPWorld + Vector3.left * 0.1f, MPWorld + Vector3.right * 0.1f, Color.green, 0.1f); // debug msg

        //Debug.DrawLine(MPWorld + Vector3.up * 0.1f, MPWorld + Vector3.down * 0.1f, Color.green, 0.1f); // debug msg

        // Ellison - moved everything into a check for movement being enabled AND cursor not being over a UI element
        if (!IsPointerOverUIObject())
        {
            //Debug.Log($"Cursor Z: {cursor.transform.position.z}");

            //Debug.Log($"Camera Z: {Camera.main.transform.position.z}");

            //RaycastHit2D? hit = GetFocusedOnTile();

            var hit = GetFocusedOnTile(); // reference

            // found something then save it to a gameobj
            if (hit.HasValue)
            {

                //GameObject overlayTile = hit.Value.collider.gameObject;

                //Debug.Log($"Cursor moving to {overlayTile.transform.position}");

                if (cursor != null)
                //if (Input.GetMouseButtonDown(0))
                {
                    //Vector3 targetPosition = overlayTile.transform.position;

                    //targetPosition.z -= 0.01f; // tiny offsets to z

                    OverlayTile1 tile = hit.Value.collider.gameObject.GetComponent<OverlayTile1>(); // which tile to spawn

                    // get out if tile not found
                    if (tile == null)
                        return;

                    cursor.transform.position = tile.transform.position; // set cursor location to the overlay

                    cursor.GetComponent<SpriteRenderer>().sortingOrder = 9999;

                    // spawn player before Turn starts
                    if (TurnManager.Instance.State == TurnState.PlayerSpawn && Input.GetMouseButtonDown(0))
                    {
                        // tile not found get out
                        if (tile == null) return;

                        // if tile is being used get out
                        if (tile.isBlocked || tile.hasEnemy || tile.hasPlayer) return;

                        characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo1>(); // copy character info from character1

                        PositionCharacterOnLine(tile); // where to spawn

                        characterInfo.PlayerSetTile(tile); // set up the player tile

                        TurnManager.Instance.SetTurnState(TurnState.PlayerStart); // after respawn starts turn

                        PlayerCombatCheck.Instance?.PlayerSetUp(); // set up player status
                        return;
                    }

                    // check for attack input if key press is A
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        currentAction = PlayerAction.Attack;
                        Debug.Log("Attack Mode on!");
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        //tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1); // changes the selected color

                        //tile.ShowTile(); // get the color

                        Vector2 world = Camera.main.ScreenToWorldPoint(Input.mousePosition); // get the input position of mouse

                        DebugMouseClickHits(world); // function call to check

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

                        //Debug.Log($"Clicked on tile: {overlayTile.name}");

                        // Attack mode on
                        if (currentAction == PlayerAction.Attack && Input.GetMouseButtonDown(0))
                        {
                            //Vector2 worldP = Camera.main.ScreenToWorldPoint(Input.mousePosition); // get the input position of mouse

                            EnemyInfo enemy = GetEnemyMouseClick(); // check for enemy status

                            //EnemyInfo enemy = GetEnemyUnderMouse(worldP); // find the enemyInfo using world position

                            Debug.Log(enemy == null ? "No enemy under mouse click" : $"Enemy clicked: {enemy.name}"); // display enemy if clicked works

                            // enemy exist attack
                            if (enemy != null)
                                PlayerCombatCheck.Instance.PlayerAttackCheck(enemy); // passes the enemy over to finalize the attack

                            currentAction = PlayerAction.None; // set the player action to none
                            return;
                        }

                        // if the character movement is enabled
                        if (movementEnabled)
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
                            }
                        }

                    }
                    //overlayTile.GetComponent<SpriteRenderer>().sortingOrder; // also matches the sprite render

                    //cursor.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
                    //cursor.GetComponent<SpriteRenderer>().sortingOrder = 9999;
                    //cursor.GetComponent<SpriteRenderer>().color = Color.red; // bright color to check visibility

                }

                //transform.position = overlayTile.transform.position; // position = overlay position

                //gameObject.GetComponent<SpriteRenderer>().sortingLayerID = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
            }

            if (path.Count > 0)
            {
                MoveAlongPath();
            }
            //else
            //{
            //Debug.Log("No hit");
            //}
        }
    }

    private void MoveAlongPath()
    {
        // Ellison - added to disable collapse button while moving
        collapseButton.interactable = false;

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

                TurnManager.Instance.PlayerSpendAP(1); // checks for the AP each turn, 1 AP per movement

                // Ellison - disable movement after moving, allow action menu to pop back up, re-enable collapse button
                DisableMovement();
                menuAnimator.SetBool("isCollapsed", false);
                collapseButton.interactable = true;
            }
        }
    }

    private void PositionCharacterOnLine(OverlayTile1 tile)
    {
        if (characterInfo == null) // characterInfo not found go through the characterPrefab and use that info
        {
            characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo1>();

            PositionCharacterOnLine(tile); // current position

            //characterInfo.PlayerSetTile(tile); // update the tile 

            return;
        }

        else if (characterInfo.CurrentTile != null) // reset the old tile, reset flag
            characterInfo.CurrentTile.hasPlayer = false;

        tile.hasPlayer = true; // triggers the flag has player

        characterInfo.PlayerSetTile(tile); // update player's tile info

        // offset the y-axis a little bit
        characterInfo.transform.position = new Vector3(tile.transform.position.x,
            tile.transform.position.y + 0.0001f,
            tile.transform.position.z
        );  // store the postion

        characterInfo.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;
    }

    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 mousePosition2d = new Vector2(mousePosition.x, mousePosition.y); // get the 2d position

        RaycastHit2D[] hit = Physics2D.RaycastAll(mousePosition2d, Vector2.zero); // raycast is like a imaginary line to find what's infront 

        //Debug.DrawRay(mousePosition2d, Vector2.zero, Color.red, 0.2f);
        Debug.Log($"All Hits: {hit.Length}");

        if (hit.Length > 0)
        {
            return hit.OrderByDescending(i => i.collider.transform.position.z).First(); // return whatever hits first
        }

        return null;
    }

    private void DebugMouseClickHits(Vector2 worldPosition)
    {
        var hits = Physics2D.OverlapPointAll(worldPosition); // what the world position the mouse hit

        Debug.LogWarning($"Click:{worldPosition} hit {hits.Length} colliders:"); // debug msg

        // loop to find what it hit
        foreach (var hit in hits)
            Debug.LogWarning($" - {hit.name} layer:{LayerMask.LayerToName(hit.gameObject.layer)} z:{hit.transform.position.z}"); // debug msg
    }

    private EnemyInfo GetEnemyMouseClick()
    {
        // camera not found return null
        if (Camera.main == null) return null;

        //Vector3 mPosition = Input.mousePosition; // mouse input position set up

        //mPosition.z = -Camera.main.transform.position.z; // set up the correct z position

        //Vector3 mousePositionWorld = Camera.main.ScreenToWorldPoint(mPosition); // get mouse position acorrding to mian cam

        //Vector2 mousePosition2d = mousePositionWorld; // new Vector2(mousePositionWorld.x, mousePositionWorld.y ); // get the 2d position x,y

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPosition); //(mousePosition2d); // layer doesn't work, enemyLayer); // reading it from 2d x,y and layer

        Debug.Log($"Mouse world position: {worldPosition}, enemy hits: {hits.Length}"); // debug msg {mousePosition2d}

        foreach (var h in hits)
        {
            // accept Enemy Layer only
            if (h.gameObject.layer != LayerMask.NameToLayer("Enemy"))
                continue;

            Debug.Log($"Hit collider: {h.name} layer = {LayerMask.LayerToName(h.gameObject.layer)}"); // debug msg

            EnemyInfo enemy = h.GetComponentInParent<EnemyInfo>(); // get enemyinfo from hit

            // if enemy exist returns the enemy info
            if (enemy != null) return enemy; 
        }

        return null; // elsee return null
    }


    // Ellison - added function to toggle movement on and off
    public void ToggleMovement()
    {
        movementEnabled = !movementEnabled;
    }


    // Added by Ellison - function to detect if cursor over UI element
    // from https://discussions.unity.com/t/detect-if-pointer-is-over-any-ui-element/138619
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    // Enables movement
    public void EnableMovement()
    {
        // Checks to see if movement is enabled, only runs if the movement is currently off
        if (!movementEnabled)
        {
            EnemyInfo enemy = GetEnemyMouseClick(); // raycast check

            if (enemy != null)
            {
                PlayerCombatCheck.Instance.PlayerAttackCheck(enemy);
                return;
            }

            movementEnabled = true;
        }
    }


    // Disables movement
    public void DisableMovement()
    {
        // Checks to see if movement is enabled, only runs if the movement is currently on
        if (movementEnabled)
        {
            movementEnabled = false;
        }
    }
}

// doesn't work
//private EnemyInfo GetEnemyUnderMouse(Vector2 worldPos)
//{
//    var hits = Physics2D.OverlapPointAll(worldPos); // find the overlap points

//    // loop to find enemyinfo if hit
//    foreach (var hit in hits)
//    {
//        var enemy = hit.GetComponentInParent<EnemyInfo>(); // get the hit.enemyInfo

//        if (enemy != null) return enemy; // if enemy exit return it
//    }

//    return null;
//}

