// Ellison
// This script manages all other custom keybinds used by the new Unity Input System.
// A lot of pressed keys also share functionality with UI buttons, so this script keeps them all together in one place
// This way the Input System is only called in one place and the functionality isn't scattered across multiple scripts
// Also makes it easier to keep track of if the user becoems allowed to rebind their own controls later

// Any new keybinds should follow the format used for MOVE:
// 1. Create a public InputAction variable for the keybind
// 2. Add references to any scripts needed to call functions when the key is pressed
// 3. In Start(), connect the InputAction variable to the correct action in the Input System (name needs to match exactly what is in the Input Actions asset)
// 4. In Update(), check for key press, then call any functions that should happen when the key is pressed
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindManager : MonoBehaviour
{
    // reference to Input System
    public InputActionAsset InputActions;

    // MOVE
    // user-bound key to toggle movement(default M key)
    public InputAction toggleMovementAction;
    // reference to MouseController1.cs to actually toggle movement
    public MouseController1 mouseController;
    // reference to ActionMenuManager.cs to toggle action menu
    public ActionMenuManager actionMenuManager;

    // INVENTORY
    // user-bound key to open inventory (default I key)
    public InputAction toggleInventoryAction;
    // no other references needed, ActionMenuManager already present

    // ADD OTHER KEYBINDS HERE WITH SAME FORMAT AS NEEDED


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Connect movement action
        toggleMovementAction = InputSystem.actions.FindAction("MoveToggle");
        Debug.Log("Toggle Movement Action set up");

        // Connect inventory action
        toggleInventoryAction = InputSystem.actions.FindAction("InventoryToggle");
        Debug.Log("Toggle Inventory Action set up");

        // ADD OTHER KEYBINDS HERE WITH SAME FORMAT AS NEEDED
    }

    // Update is called once per frame
    void Update()
    {
        // Check for toggle movement key press
        // References MouseController1.cs and ActionMenuManager.cs
        if (toggleMovementAction.WasPressedThisFrame())
        {
            mouseController.ToggleMovement();
            actionMenuManager.ToggleMenu();
            Debug.Log("M key pressed");
        }

        // Check for toggle inventory key press
        // References ActionMenuManager.cs
        if (toggleInventoryAction.WasPressedThisFrame())
        {
            actionMenuManager.ToggleInventoryScreen();
            Debug.Log("I key pressed");
        }

        // ADD OTHER KEYBINDS HERE WITH SAME FORMAT AS NEEDED
    }
}
