using UnityEngine;
using UnityEngine.UI;  

public class ActionMenuManager : MonoBehaviour
{
    public Button collapseButton;
    private Animator menuAnimator;

    public Button inventoryButton;
    public GameObject inventoryScreen;

    public bool actionMenuOpen = true;
    public bool movementEnabled = false;

    public GameObject moveMessagePanel;

    public bool inventoryOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuAnimator = GetComponent<Animator>();

        if (collapseButton == null)
        {
            Debug.LogError("Collapse Button is not assigned in the inspector.");
        }

        if (inventoryButton == null)
        {
            Debug.LogError("Inventory Button is not assigned in the inspector.");
        }

        if (inventoryScreen == null)
        {
            Debug.LogError("Inventory Screen is not assigned in the inspector.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!inventoryOpen)
            {
                ToggleMovementFromKey();
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryScreen();
        }

    }


    public void ToggleMenu()
    {
        if (menuAnimator != null)
        {
            if (menuAnimator.GetBool("isCollapsed"))
            {
                menuAnimator.SetBool("isCollapsed", false);
            }
            else
            {
                menuAnimator.SetBool("isCollapsed", true);
            }
        }
    }

    public void CloseMenu()
    {
        if (menuAnimator != null)
        {
            menuAnimator.SetBool("isCollapsed", true);
        }
    }


    public void OpenInventoryScreen()
    {
        inventoryScreen.SetActive(true);

        // If Action Menu open, close it
        if (!menuAnimator.GetBool("isCollapsed"))
        {
            menuAnimator.SetBool("isCollapsed", true);
        }

        inventoryOpen = true;
    }

    public void CloseInventoryScreen()
    {
        inventoryScreen.SetActive(false);

        // If Action Menu closed, open it
        if (menuAnimator.GetBool("isCollapsed"))
        {
            menuAnimator.SetBool("isCollapsed", false);
        }

        inventoryOpen = false;
    }

    public void ToggleInventoryScreen()
    {
        if (inventoryScreen.activeSelf)
        {
            CloseInventoryScreen();
        }
        else
        {
            OpenInventoryScreen();
        }
    }

    public void EnableMovementFull()
    {
        movementEnabled = true;
        moveMessagePanel.SetActive(true);
        menuAnimator.SetBool("isCollapsed", true);
    }

    public void DisableMovementFull()
    {
        movementEnabled = false;
        moveMessagePanel.SetActive(false);
        menuAnimator.SetBool("isCollapsed", false);
    }

    public void DisableMovementPartial()
    {
        movementEnabled = false;
        moveMessagePanel.SetActive(false);
    }

    public void BeginMovementProcess()
    {
        moveMessagePanel.SetActive(false);
        collapseButton.interactable = false;
    }

    public void EndMovementProcess()
    {
        collapseButton.interactable = true;
        movementEnabled = false;
        menuAnimator.SetBool("isCollapsed", false);
    }

    public void ToggleMovementFromKey()
    {
        if (!movementEnabled)
        {
            EnableMovementFull();
        }
        else
        {
            DisableMovementFull();
        }
    }
}
