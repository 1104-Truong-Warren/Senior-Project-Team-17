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

    private OverlayTile1 previouslySelectedTile; // previous tile
    private PathFinder1 pathFinder; // access the pathfinder
    private List<OverlayTile1> path;
    public bool movementEnabled = false;
    public Animator menuAnimator;
    public Button collapseButton;

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
    }

    private void SpawnCharacterAtTile()
    {
        // Instantiate player at set position and put on line
        characterInfo = Instantiate(characterPrefab).GetComponent<CharacterInfo1>(); // copy character info from character1
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

        RaycastHit2D[] hit = Physics2D.RaycastAll(mousePosition2d, Vector2.zero); // raycast is like a imaginary line to find what's infront 

        //Debug.DrawRay(mousePosition2d, Vector2.zero, Color.red, 0.2f);
        Debug.Log($"All Hits: {hit.Length}");

        if (hit.Length > 0)
        {
            return hit.OrderByDescending(i => i.collider.transform.position.z).First(); // return whatever hits first
        }

        return null;
    }
}
