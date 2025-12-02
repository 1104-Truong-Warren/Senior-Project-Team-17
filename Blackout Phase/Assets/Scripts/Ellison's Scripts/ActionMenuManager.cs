using UnityEngine;
using UnityEngine.UI;  

public class ActionMenuManager : MonoBehaviour
{
    public Button collapseButton;
    private Animator menuAnimator;

    public Button inventoryButton;
    public GameObject inventoryScreen;

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


    public void OpenInventoryScreen()
    {
        inventoryScreen.SetActive(true);

        bool tempBool = menuAnimator.GetBool("isCollapsed");
        tempBool = !tempBool;

        if (tempBool)
        {
            menuAnimator.SetBool("isCollapsed", true);
        }

        
    }

    public void CloseInventoryScreen()
    {
        inventoryScreen.SetActive(false);

        if (menuAnimator.GetBool("isCollapsed"))
        {
            menuAnimator.SetBool("isCollapsed", false);
        }
    }
}
