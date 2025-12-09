using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour
{
    [SerializeField] private GameObject cursor; // for our curosr

    [SerializeField] private GameObject characterPrefab; // object for the character prefab

    [SerializeField] private CharacterInfo1 characterInfo; // stores the characgter info

    [SerializeField] private float speed; // move speed for character

    private OverlayTile previouslySelectedTile; // previous tile

    private PathFinder pathFinder; // access the pathfinder

    private List<OverlayTile> path;


    // Ellison - Added bool to enable or disable movement (disabled by default)
    public bool movementEnabled = false;

    // Ellison - Reference to Action Menu's Animator for opening and closing
    public Animator menuAnimator;

    // Ellison - Reference to collapse button to disable it when movement is enabled
    public Button collapseButton;

    private void Start()
    {
        pathFinder = new PathFinder(); // create it

        path = new List<OverlayTile>(); 
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (TurnManager.Instance.State != TurnState.PlayerAction) return; // preventing player moving before other things are setup

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

                    OverlayTile tile = hit.Value.collider.gameObject.GetComponent<OverlayTile>();

                    // get out if tile not found
                    if (tile == null)
                        return;

                    cursor.transform.position = tile.transform.position; // set cursor location to the overlay

                    cursor.GetComponent<SpriteRenderer>().sortingOrder = 9999;

                    if (Input.GetMouseButtonDown(0))
                    {
                        //tile.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1); // changes the selected color

                        //tile.ShowTile(); // get the color

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

    private void PositionCharacterOnLine(OverlayTile tile)
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

        Debug.DrawRay(mousePosition2d, Vector2.zero, Color.red, 0.2f);
        Debug.Log($"Hits: {hit.Length}");

        if (hit.Length > 0)
        {
            return hit.OrderByDescending(i => i.collider.transform.position.z).First(); // return whatever hits first
        }

        return null;
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
