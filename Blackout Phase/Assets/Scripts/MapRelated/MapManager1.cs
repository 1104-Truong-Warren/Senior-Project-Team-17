// These are based on this channel on YouTube: https://www.youtube.com/@lawlessgames3844
// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// Weijun
using System; // for c# library stuff like int, string etc...
using System.Collections.Generic; // for the List<T> and dictionary <T, T> for pathfinding
using UnityEngine; // default
using UnityEngine.Tilemaps; // for the grid isomatric/ hexagonal tilemaps, the one we have is isometric
using System.Collections; // for the array list we have also IEnumerator for delay funciton calls yield returns. loading map first then do something else

public class MapManager1 : MonoBehaviour
{
    private static MapManager1 _instance; 
    public static MapManager1 Instance { get { return _instance; } } // can access but can't change it 

    [Header("File attach")]
    [SerializeField] public OverlayTile1 overlayTilePrefab; // to access the overlay prefab
    [SerializeField] public GameObject overlayContainer;  // to access the gameobject
    [SerializeField] private Tilemap groundTileMap; // for ground only

    public Dictionary<Vector2Int, OverlayTile1> map;   // map position, using x,y, and overlay using map
    private bool ignoreBottomTiles;      // flag for tiles that are under tiles, z high 

    public static event Action OnMapFinished; // waiting for map before anything else

    private void Awake()
    {
        // if it's not null and not equal to this destory object, else keep it
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        StartCoroutine(GenerateMap()); // calls the delay
    }

    private IEnumerator GenerateMap()
    {

        //var tileMap = gameObject.GetComponentInChildren<Tilemap>(); // get the map

        var tileMap = groundTileMap; // only using the ground layer

        map = new Dictionary<Vector2Int, OverlayTile1>();

        BoundsInt bounds = tileMap.cellBounds; // find the edges of the map

        // loop through all the tiles in order, z from high to low, y,x
        for (int z = bounds.max.z; z >= bounds.min.z; z--) // hights to lowest 
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    if (z == 0 && ignoreBottomTiles)
                        yield break;

                    Vector3Int tileLocation = new Vector3Int(x, y, z); // new vection3 location

                    Vector2Int tileKey = new Vector2Int(x, y);  // new vection2 location, for our path find

                    // tile has tiles, the map is built before loading in
                    if (tileMap.HasTile(tileLocation) && tileMap.GetTile(tileLocation) != null &&
                        !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform); // copy the overlay info over

                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation); // center of a grid cell

                        overlayTile.transform.position = cellWorldPosition + new Vector3(0, 0, -0.05f); // keep the x,y but offset the z a little 
                        //      cellWorldPosition.z - 0.1f); // one higher than the actual map

                        //overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;

                        overlayTile.gridLocation = new Vector3Int(x, y, 0); //tileLocation; // save the gridlocation for overlayTile

                        // Consider ONLY the ground tilemap:
                        overlayTile.isBlocked = false; //tileMap.GetTile(tileLocation) == null;

                        overlayTile.HideTile(); // hides the tile

                        SpriteRenderer spriteRenderer = overlayTile.GetComponent<SpriteRenderer>(); // setup sprite render

                        TilemapRenderer tilemapRenderer = tileMap.GetComponent<TilemapRenderer>(); // setup tilemap render

                        //overlayTile.GetComponent<SpriteRenderer>().color = Color.yellow; // display yellow

                        if (spriteRenderer != null && tileMap != null)//!map.ContainsKey(tileKey) || overlayTile.gridLocation.z > map[tileKey].gridLocation.z) // only keep the highest z
                        {
                            spriteRenderer.sortingOrder = tilemapRenderer.sortingOrder + 1; // keep the sprite higher than tile
                        }

                        map[tileKey] = overlayTile; // if nothing happens stores the overlay 
                        Debug.Log($"Overlay created at {tileKey} : Tilemap.HasTile = {tileMap.HasTile(tileLocation)}"); // debug msg
                    }

                }
            }
        }
        Debug.Log("Tiles created: " + map.Count);

        OnMapFinished?.Invoke(); // make sure it delays

        yield return null; // give it a frame
    }

    public void ResetAllTiles()
    {
        foreach (var tile in map.Values) // calls the overlay tile reset for all objects
            tile.ResetTiles();
    }

    public OverlayTile1 GetTile(Vector2Int gridPosition)
    {
        if (map.ContainsKey(gridPosition))
            return map[gridPosition]; // if exisit return it the map location

        return null; // if not return nothing
    }
}


