using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickToMove : MonoBehaviour
{
    public Grid grid;
    public Transform player;
    
    private Vector2Int playerGridPos = new Vector2Int(5, 5);

    // Added by Ellison - to track if movement is allowed
    bool movementEnabled = false;

    // reference to Action Menu's animator to open and close menu for movement
    public Animator menuAnimator;

    // reference to Move Button and Menu Open/Collapse Button
    public Button moveButton;
    public Button menuToggleButton;

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
            if (!IsPointerOverUIObject() && movementEnabled)
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

                        menuAnimator.SetBool("isCollapsed", false);
                        movementEnabled = false;
                    }
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


    // Added by Ellison - function to detect if cursor over UI element
    // from https://discussions.unity.com/t/detect-if-pointer-is-over-any-ui-element/138619
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    
    public void EnableMovement()
    {
        if (!movementEnabled)
        {
            movementEnabled = true;
            ClearSpheres();
            grid.CallAccessibleTiles(playerGridPos.x, playerGridPos.y, 3);
        }
    }

    public void DisableMovement()
    {
        if (movementEnabled)
        {
            movementEnabled = false;
            ClearSpheres();
        }
    }

}