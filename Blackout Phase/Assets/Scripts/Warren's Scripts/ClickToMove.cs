using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ClickToMove : MonoBehaviour
{
    // Reference to the Grid class
    public Grid grid;


    // Reference to Unity's built-in component that hands the position, rotation, and scale (of the player)
    public Transform player;
   
    // Reference to Unity's built-in component that stores x and y values, it also starts the player at grid position (5,5)
    // Resource: https://docs.unity3d.com/ScriptReference/Vector2Int.html
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
        // Checks if the player object exists, then it will position the player at (5,5,-1)
        if (player != null)
        {
            player.position = new Vector3(5, 5, -1);
        }
    }
   
    void Update()
    {
        // When you press space, it will clear all of the visual markers of reachable tiles, and you press it again, it will show call all of the accessible tiles with a range limit of 3
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearSpheres();
            grid.CallAccessibleTiles(playerGridPos.x, playerGridPos.y, 3);
        }
       
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject() && movementEnabled)
            {
                Vector3 mousePos = Input.mousePosition; // Coordinates on where the mouse presses
                mousePos.z = 10; // Distance from the camera
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos); // Game world coordinates


                // Green visual marker for debugging purposes, checks to see if the player will not move out of bounds
                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube); // Creates a cube object at runtime
                marker.transform.position = new Vector3(worldPos.x, worldPos.y, -1);
                marker.transform.localScale = Vector3.one * 0.3f;
                marker.GetComponent<Renderer>().material.color = Color.green;
                Destroy(marker, 3f); // Will be automatically removed in 3 seconds


                // Rounds the world coordinates to the nearest whole number
                // Resource: https://docs.unity3d.com/ScriptReference/Mathf.RoundToInt.html
                int targetX = Mathf.RoundToInt(worldPos.x);
                int targetY = Mathf.RoundToInt(worldPos.y);


                // Checks if the the clicked position is within the grid bounderies
                if (targetX >= 0 && targetX < grid.width && targetY >= 0 && targetY < grid.height)
                {
                    // Checks if the tile is visited
                    if (grid.grid[targetX, targetY].visited)
                    {
                        MovePlayerTo(targetX, targetY); // Moves the player to target position


                        menuAnimator.SetBool("isCollapsed", false);
                        movementEnabled = false;
                    }
                }
            }
           
        }
       
        // When you press Key C, it will clear the spheres if you no longer want visualization
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearSpheres();
        }


    }
   
    // Moves the player to the specific grid position
    void MovePlayerTo(int x, int y)
    {
        playerGridPos = new Vector2Int(x, y); // Updates the player's grid position
        player.position = new Vector3(x, y, -1); // Moves the player object to the new position
       
        ClearSpheres(); // Removes visual markers
        grid.TestPathDisplay(5, 5, x, y, 3); // Movement path, displays a path from the starting point (5,5)
    }
   
    // Removes existing visual markers
    void ClearSpheres()
    {
        GameObject[] spheres = GameObject.FindGameObjectsWithTag("AccessibleTileDisplay"); // This finds all of the marker objects with the tag "AccessibleTileDisplay"


        // Loops through each of the marker and removes it from the scene
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


    // Enables movement
    public void EnableMovement()
    {
        // Checks to see if movement is enabled, only runs if the movement is currently off
        if (!movementEnabled)
        {
            movementEnabled = true;
            ClearSpheres();
            grid.CallAccessibleTiles(playerGridPos.x, playerGridPos.y, 3); // Shows the reachable tiles
        }
    }


    // Disables movement
    public void DisableMovement()
    {
        // Checks to see if movement is enabled, only runs if the movement is currently on
        if (movementEnabled)
        {
            movementEnabled = false;
            ClearSpheres();
        }
    }


}

