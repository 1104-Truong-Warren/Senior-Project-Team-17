using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private GameObject cursor; // for our curosr

    [SerializeField] private GameObject characterPrefab; // object for the character prefab

    [SerializeField] private CharacterInfo characterInfo; // stores the characgter info

    [SerializeField] private float speed; // move speed for character

    private OverlayTile previouslySelectedTile; // previous tile

    private PathFinder pathFinder; // access the pathfinder

    private List<OverlayTile> path;

    private void Start()
    {
        pathFinder = new PathFinder(); // create it

        path = new List<OverlayTile>(); 
    }

    // Update is called once per frame
    private void LateUpdate()
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

                    MapManager.Instance.ResetAllTiles(); // before showing tiles reset all

                    tile.ShowPlayerTile();

                    previouslySelectedTile = tile; // shows current tile and save it

                    //Debug.Log($"Clicked on tile: {overlayTile.name}");

                    if (characterInfo == null)
                    {
                        //characterInfo = new CharacterInfo(); // declare it again 

                        characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo>(); // get the prefab assign

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
                        path = pathFinder.FindPath(characterInfo.CurrentTile, tile); //(characterInfo.standingOnTile, overlayTile.GetComponent<OverlayTile>());

                        //tile.gameObject.GetComponent<OverlayTile>().HideTile(); // hides the tile

                        //Show the path tiles (highlight)
                        foreach (var t in path)
                            t.ShowPlayerTile();
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

    private void MoveAlongPath()
    {
        var step = speed * Time.deltaTime; // how fast character moves

        float zIndex = path[0].transform.position.z + 0.01f;

        characterInfo.transform.position = Vector2.MoveTowards(characterInfo.transform.position, // move to
            path[0].transform.position, step);

        characterInfo.transform.position = new Vector3(characterInfo.transform.position.x, // from
            characterInfo.transform.position.y, zIndex);

        if (Vector2.Distance(characterInfo.transform.position, path[0].transform.position) < 0.00001f)
        {
            PositionCharacterOnLine(path[0]); // calculate postition on line

            path.RemoveAt(0); // remove after
        }
    }

    private void PositionCharacterOnLine(OverlayTile tile)
    {
        if (characterInfo == null) // characterInfo not found go through the characterPrefab and use that info
        {
            characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo>();

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
}
