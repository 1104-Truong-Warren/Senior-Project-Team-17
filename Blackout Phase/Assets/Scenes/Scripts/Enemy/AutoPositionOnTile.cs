using UnityEngine;

public class AutoPositionOnTile : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(AutoPosition());
    }

    private System.Collections.IEnumerator AutoPosition()
    {
        Debug.Log("Waiting for MapManager...");
        
        // Wait for MapManager
        while (MapManager.Instance == null || MapManager.Instance.map == null)
        {
            yield return null;
        }
        
        Debug.Log($"Map ready with {MapManager.Instance.map.Count} tiles");
        
        // Find first available tile and move there
        foreach (var tile in MapManager.Instance.map.Values)
        {
            if (!tile.isBlocked)
            {
                // Position enemy on this tile
                transform.position = tile.transform.position;
                
                // Set CharacterInfo
                CharacterInfo charInfo = GetComponent<CharacterInfo>();
                if (charInfo != null)
                {
                    charInfo.standingOnTile = tile;
                }
                
                Debug.Log($"Enemy auto-positioned at tile: {tile.gridLocation} at world position: {tile.transform.position}");
                break;
            }
        }
        
        Debug.Log($"Enemy final position: {transform.position}");
    }
}