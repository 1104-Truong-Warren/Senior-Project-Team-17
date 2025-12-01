using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public Grid grid;
    public Transform player;
    
    private Vector2Int playerGridPos = new Vector2Int(5, 5);
    
    void Start()
    {
        if (player != null)
            player.position = new Vector3(5, 5, -1);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearSpheres();
            grid.CallAccessibleTiles(playerGridPos.x, playerGridPos.y, 3);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            
            // CREATE A VISUAL MARKER AT CLICK POSITION
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.transform.position = new Vector3(worldPos.x, worldPos.y, -1);
            marker.transform.localScale = Vector3.one * 0.3f;
            marker.GetComponent<Renderer>().material.color = Color.green;
            Destroy(marker, 3f); // Remove after 3 seconds
            
            int targetX = Mathf.RoundToInt(worldPos.x);
            int targetY = Mathf.RoundToInt(worldPos.y);
            
            if (targetX >= 0 && targetX < grid.width && 
                targetY >= 0 && targetY < grid.height)
            {
                if (grid.grid[targetX, targetY].visited)
                {
                    MovePlayerTo(targetX, targetY);
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearSpheres();
        }
    }
    
    void MovePlayerTo(int x, int y)
    {
        playerGridPos = new Vector2Int(x, y);
        player.position = new Vector3(x, y, -1);
        
        ClearSpheres();
        grid.TestPathDisplay(5, 5, x, y, 3);
    }
    
    void ClearSpheres()
    {
        GameObject[] spheres = GameObject.FindGameObjectsWithTag("AccessibleTileDisplay");
        foreach (GameObject sphere in spheres)
        {
            Destroy(sphere);
        }
    }
}