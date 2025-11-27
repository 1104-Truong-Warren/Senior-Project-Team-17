using UnityEngine;

public class GridDebugger : MonoBehaviour
{
    [ContextMenu("Debug Grid Information")]
    public void DebugGridInfo()
    {
        Debug.Log("=== GRID DEBUG INFORMATION ===");
        
        // Check if we have a Grid component
        Grid grid = FindObjectOfType<Grid>();
        if (grid != null)
        {
            Debug.Log($"Grid found: {grid.name}");
            Debug.Log($"Cell Size: {grid.cellSize}");
            Debug.Log($"Cell Layout: {grid.cellLayout}");
            Debug.Log($"Cell Swizzle: {grid.cellSwizzle}");
        }
        else
        {
            Debug.LogWarning("No Grid component found in scene!");
        }

        // Check MapManager
        if (MapManager.Instance != null)
        {
            Debug.Log($"MapManager found with {MapManager.Instance.map?.Count ?? 0} tiles");
            
            if (MapManager.Instance.map != null && MapManager.Instance.map.Count > 0)
            {
                Debug.Log("First 5 tile coordinates:");
                int count = 0;
                foreach (var entry in MapManager.Instance.map)
                {
                    if (count >= 5) break;
                    Debug.Log($"  {entry.Key} -> World Position: {entry.Value.transform.position}");
                    count++;
                }
            }
        }

        // Check what coordinates exist
        Debug.Log("=== CHECKING COORDINATE RANGE ===");
        CheckCoordinateRange(-5, 5, -5, 5);
    }

    private void CheckCoordinateRange(int minX, int maxX, int minY, int maxY)
    {
        if (MapManager.Instance == null || MapManager.Instance.map == null) return;

        bool foundAny = false;
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int coord = new Vector2Int(x, y);
                if (MapManager.Instance.map.ContainsKey(coord))
                {
                    Debug.Log($"FOUND TILE at {coord} - World Position: {MapManager.Instance.map[coord].transform.position}");
                    foundAny = true;
                }
            }
        }

        if (!foundAny)
        {
            Debug.LogWarning($"No tiles found in range ({minX},{minY}) to ({maxX},{maxY})");
        }
    }
}