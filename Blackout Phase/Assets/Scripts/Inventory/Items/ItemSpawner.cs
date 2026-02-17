// Attached to a hierarchy item that is responsible for spawning one item in the game world
// Inspired by Weijun's EnemySpawner script to keep consistency
// - Ellison

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ScriptableObject item; // item to spawn
    [SerializeField] private GameObject itemObject; // item object to spawn in the world
    [SerializeField] private Vector2Int spawnGridPosition; // where to spawn the item


    private IEnumerator Start()
    {
        yield return new WaitUntil(() => MapManager1.Instance != null &&
            MapManager1.Instance.map != null &&
            MapManager1.Instance.map.Count > 0); // wait until the map is fully setup before anything else

        SpawnAfterMapReady(); // delay call 
    }


    private void SpawnAfterMapReady()
    {
        // wait until map is spawned and the map count > 0
        //yield return new WaitUntil(() => MapManager.Instance.map != null && MapManager.Instance.map.Count > 0);

        OverlayTile1 tile = MapManager1.Instance.GetTile(spawnGridPosition); // get the spawn tile

        if (tile == null)
        {
            Debug.LogError($"Spawn failed No tile found at {spawnGridPosition}"); // nothing found
            return; // get out
        }

        GameObject worldItem = Instantiate(itemObject, tile.transform.position, Quaternion.identity); // setup the item through prefab
        WorldItemInfo worldItemInfo = worldItem.GetComponent<WorldItemInfo>();

        worldItemInfo.ItemSetTile(tile); // setup the tile through script

        tile.hasItem = true; // setup the tile through script

        worldItem.GetComponent<WorldItemInfo>().item = item as Item; // setup the item through scriptable object
        worldItem.GetComponent<SpriteRenderer>().sprite = (item as Item).icon; // setup the item sprite through scriptable object

        //worldItem.layer = LayerMask.NameToLayer("Ignore Raycast");

        // NEW: Register item with tile system (matching enemy behavior)
        worldItem.transform.position = new Vector3(tile.transform.position.x,
            tile.transform.position.y + 0.25f, // y offset
            tile.transform.position.z + 0.02f);
    }
}
