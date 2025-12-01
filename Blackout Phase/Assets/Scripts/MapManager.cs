using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } } // can access but can't change it 

    [Header("File attach")]
    [SerializeField] public OverlayTile overlayTilePrefab; // to access the overlay prefab
    [SerializeField] public GameObject overlayContainer;  // to access the gameobject
    [SerializeField] private Tilemap groundTileMap; // for ground only

    public Dictionary<Vector2Int, OverlayTile> map;   //
    private bool ignoreBottomTiles;      // flag 

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

        map = new Dictionary<Vector2Int, OverlayTile>();

        BoundsInt bounds = tileMap.cellBounds; // find the edges of the map

        // loop through all the tiles
        for (int z = bounds.max.z; z >= bounds.min.z; z--) // hights to lowest 
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                for (int x = bounds.min.x; x < bounds.max.x; x++)
                {
                    if (z == 0 && ignoreBottomTiles)
                        yield break;

                    Vector3Int tileLocation = new Vector3Int(x, y, z); // new vection3 location

                    Vector2Int tileKey = new Vector2Int(x, y);  // new vection2 location

                    if (tileMap.HasTile(tileLocation) && tileMap.GetTile(tileLocation) != null &&
                        !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);

                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = cellWorldPosition + new Vector3(0, 0, -0.05f);
                        //      cellWorldPosition.z - 0.1f); // one higher than the actual map

                        //overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;

                        overlayTile.gridLocation = new Vector3Int(x, y, 0); //tileLocation; // save the gridlocation

                        // Consider ONLY the ground tilemap:
                        overlayTile.isBlocked = false; //tileMap.GetTile(tileLocation) == null;

                        overlayTile.HideTile(); // hides the tile

                        SpriteRenderer spriteRenderer = overlayTile.GetComponent<SpriteRenderer>(); // setup sprite render

                        TilemapRenderer tilemapRenderer = tileMap.GetComponent<TilemapRenderer>(); // setup tilemap render

                        //overlayTile.GetComponent<SpriteRenderer>().color = Color.yellow; // display yellow

                        if (spriteRenderer != null && tileMap != null)//!map.ContainsKey(tileKey) || overlayTile.gridLocation.z > map[tileKey].gridLocation.z) // only keep the highest z
                        {
                            spriteRenderer.sortingOrder = tilemapRenderer.sortingOrder + 1;
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

    public OverlayTile GetTile(Vector2Int gridPosition)
    {
        if (map.ContainsKey(gridPosition))
            return map[gridPosition]; // if exisit return it

        return null; // if not return nothing
    }
}

////Only x,y no z

////using System;
////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;
////using UnityEngine.Tilemaps;

////public class MapManager : MonoBehaviour
////{
////    private static MapManager _instance; // privite to save data

////    public static MapManager Instance => _instance; // public so other script can use it

////    [Header("Files attach")]
////    [SerializeField] public OverlayTile overlayTilePrefab; // the prefab  for overlay
////    [SerializeField] public GameObject overlayContainer; // the overlay object
////    [SerializeField] private Tilemap groundTileMap; // for the groundTile

////    public Dictionary<Vector2Int, OverlayTile> map; // dictionary for the map

////    public static event Action OnMapFinished; // events on map

////    private void Awake()
////    {
////        //  if the map is not set up distory it
////        if (_instance != null && _instance != this) 
////        {
////            Destroy(gameObject);
////            return;
////        }

////        _instance = this; // set it to this
////    }

////    private void Start()
////    {
////        StartCoroutine(GenerateMap()); // generates the map takes time
////    }

////    private IEnumerator GenerateMap()
////    {
////        // error msg if the groundTile not found
////        if (groundTileMap == null) 
////        {
////            Debug.LogError("MapManager: groundTile is not assigned!");
////            yield break;
////        }

////        map = new Dictionary<Vector2Int, OverlayTile>(); // creates the new object for map

////        BoundsInt bounds = groundTileMap.cellBounds; // the boundaries of the tile

////        Debug.Log("Bounds: " + bounds);
////        Debug.Log("Min: " + bounds.min + "  Max: " + bounds.max);

////        Debug.Log("CellBounds = " + groundTileMap.cellBounds);
////        Debug.Log("GroundTilemap has tile at (0,0)? " + groundTileMap.HasTile(new Vector3Int(0, 0, 0)));

////        // uses nested loops to run through the x, y coordinates 
////        for (int y = bounds.min.y; y < bounds.max.y; y++)
////        {
////            for (int x = bounds.min.x; x < bounds.max.x; x++)
////            {
////                Vector3Int cell = new Vector3Int(x, y, 0); // only use x,y, z = 0

////                // check if the groundTiles is not working keep going
////                if (!groundTileMap.HasTile(cell))
////                    continue;

////                Vector2Int key = new Vector2Int(x, y); // this is the x, y checking if map matches

////                // if matches keep on going
////                if (map.ContainsKey(key))
////                    continue;

////                OverlayTile overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform); // what the overlay has, a prefab&thecontainer

////                Vector3 worldPosition = groundTileMap.GetCellCenterWorld(cell); // set up the world position for cells

////                overlayTile.transform.position = new Vector3(worldPosition.x, worldPosition.y, worldPosition.z + 1f); // above the groud overlay

////                overlayTile.gridLocation = cell; // 0 for z

////                overlayTile.isBlocked = false;   // all ttile can be access

////                overlayTile.HideTile();  // hides the tiles

////                var collider = overlayTile.GetComponent<BoxCollider2D>(); // set up the collider box2d

////                // if collider is found add it from gameobject
////                if (collider == null)
////                {
////                    collider = overlayTile.gameObject.AddComponent<BoxCollider2D>();

////                }

////                overlayTile.gameObject.layer = LayerMask.NameToLayer("OverlayTile"); // set up the layer mask 

////                map.Add(key, overlayTile); // add the correct tiles to max
////            }
////        }

////        Debug.Log("Tiles created: " + map.Count); // debug to show how many tiles created

////        OnMapFinished?.Invoke(); // wait untile map is finished
////        yield return null; // delay return
////    }

////    public void ResetAllTiles()
////    {
////        // empty map get out
////        if (map == null) return;

////        // reset all the tiles in map
////        foreach (var tile in map.Values)
////            tile.ResetTiles();
////    }

////    public OverlayTile GetTile(Vector2Int gridPosition)
////    {
////        // if tile is found get the gridPosition and return it
////        if (map != null && map.TryGetValue(gridPosition, out var tile))
////            return tile;
////        else
////            return null; // return nothing if not found

////    }
////}
///

