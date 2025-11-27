using UnityEngine;

public class EnemyDebugger : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== ENEMY DEBUGGER STARTED ===");
        StartCoroutine(DebugStepByStep());
    }

    private System.Collections.IEnumerator DebugStepByStep()
    {
        Debug.Log("Step 1: Checking basic GameObject...");
        Debug.Log($"Enemy GameObject exists: {gameObject != null}");
        Debug.Log($"Enemy position: {transform.position}");
        Debug.Log($"Enemy active in hierarchy: {gameObject.activeInHierarchy}");

        yield return new WaitForSeconds(1f);

        Debug.Log("Step 2: Checking components...");
        CharacterInfo charInfo = GetComponent<CharacterInfo>();
        Debug.Log($"CharacterInfo component: {charInfo != null}");
        if (charInfo != null)
        {
            Debug.Log($"Standing on tile: {charInfo.standingOnTile != null}");
        }

        SimplePatrol patrol = GetComponent<SimplePatrol>();
        Debug.Log($"SimplePatrol component: {patrol != null}");

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Debug.Log($"SpriteRenderer: {sprite != null}");
        if (sprite != null)
        {
            Debug.Log($"Sprite: {sprite.sprite != null}");
            Debug.Log($"Sprite color: {sprite.color}");
            Debug.Log($"Sprite enabled: {sprite.enabled}");
        }

        yield return new WaitForSeconds(1f);

        Debug.Log("Step 3: Checking MapManager...");
        while (MapManager.Instance == null)
        {
            Debug.Log("MapManager.Instance is null - waiting...");
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("MapManager.Instance found!");

        while (MapManager.Instance.map == null)
        {
            Debug.Log("MapManager.map is null - waiting...");
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log($"MapManager.map has {MapManager.Instance.map.Count} tiles");

        yield return new WaitForSeconds(1f);

        Debug.Log("Step 4: Checking if enemy is on a valid tile...");
        if (charInfo != null && charInfo.standingOnTile == null)
        {
            Debug.Log("Enemy not on any tile - attempting to find tile...");
            
            // Raycast to find what tile we're on
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero);
            if (hit.collider != null)
            {
                OverlayTile tile = hit.collider.GetComponent<OverlayTile>();
                if (tile != null)
                {
                    charInfo.standingOnTile = tile;
                    Debug.Log($"Found tile at: {tile.gridLocation}");
                }
                else
                {
                    Debug.Log($"Hit object but no OverlayTile: {hit.collider.name}");
                }
            }
            else
            {
                Debug.Log("No tile found at enemy position - enemy might be in wrong position");
                Debug.Log($"Enemy world position: {transform.position}");
            }
        }

        Debug.Log("=== DEBUG COMPLETE ===");
    }
}