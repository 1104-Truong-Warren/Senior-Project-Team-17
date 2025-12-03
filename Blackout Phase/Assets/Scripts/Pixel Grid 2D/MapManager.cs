using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance { get { return _instance; } } // can access but can't change it 

    [Header ("File attach")]
    [SerializeField] public OverlayTile overlayTilePrefab; // to access the overlay prefab
    [SerializeField] public GameObject overlayContainer;  // to access the gameobject
    [SerializeField] private Tilemap groundTileMap; // for ground only

    public Dictionary<Vector2Int, OverlayTile> map;   //
    private bool ignoreBottomTiles;      // flag 

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
    void Start()
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
                        return;

                    var tileLocation = new Vector3Int(x, y, z); // new vection3 location

                    var tileKey = new Vector2Int(x, y);  // new vection2 location

                    if (tileMap.HasTile(tileLocation) && tileMap.GetTile(tileLocation) != null &&
                        !map.ContainsKey(tileKey))
                    {
                        var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);

                        var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                        overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y,
                              cellWorldPosition.z + 1
                        ); // one higher than the actual map

                        overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;

                        overlayTile.gridLocation = tileLocation; // save the gridlocation

                        // Consider ONLY the ground tilemap:
                        overlayTile.isBlocked = tileMap.GetTile(tileLocation) == null;

                        overlayTile.HideTile(); // hides the tile

                        map.Add(tileKey, overlayTile);

                        Debug.Log($"Overlay created at {tileKey} : Tilemap.HasTile = {tileMap.HasTile(tileLocation)}");
                    }

                }
            }
        }
    }

    public void ResetAllTiles()
    {
        foreach (var tile in map.Values) // calls the overlay tile reset for all objects
            tile.ResetTiles();
    }
}
