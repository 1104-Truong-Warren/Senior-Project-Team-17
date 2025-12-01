using UnityEngine;

public class TileDebugger : MonoBehaviour
{
    private Camera cam; // camera 

    private void Start()
    {
        cam = Camera.main; // set up the camera
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // mouse is pressed
        {
            Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition); // world position = input by mouse

            Vector2 mouse2D = new Vector2(worldPosition.x, worldPosition.y); // saves the position to mouse2D

            RaycastHit2D hit = Physics2D.Raycast(mouse2D, Vector2.zero); // using raycast to find the correct position

            if (hit.collider != null)
            {
                OverlayTile tile = hit.collider.GetComponent<OverlayTile>(); // saves the tile if found

                if (tile != null)
                {
                    Debug.Log($"Clicked tile at: ({tile.gridLocation.x}, {tile.gridLocation.y})"); // display x,y when click on map

                    tile.debugSelected = !tile.debugSelected; // toggles hightlight
                }
            }
        }
    }
}
