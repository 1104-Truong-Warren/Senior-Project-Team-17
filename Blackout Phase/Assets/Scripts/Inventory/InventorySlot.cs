// From https://youtu.be/YLhj7SfaxSE?si=Wm-SfEMXYx61skpm for Inventory slots
// Ellison
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;
    public GameObject confirmDeletePanel;

    private Button confirmButton;
    private Button cancelButton;

    Item item;

    new public string name;
    public string type;
    public string description;
    public string flavorText;

    public bool hasItem = false;

    private void Start()
    {
        // Set up the remove button listener
        if (removeButton != null)
        {
            removeButton.onClick.AddListener(OnRemoveButton);
        }
    }

    public void SetConfirmationButtons(Button confirm, Button cancel)
    {
        confirmButton = confirm;
        cancelButton = cancel;
    }

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;

        name = item.itemName;
        type = item.type;
        description = item.description;
        flavorText = item.flavorText;

        hasItem = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;

        name = "";
        type = "";
        description = "";
        flavorText = "";

        hasItem = false;
    }

    public void OnRemoveButton()
    {
        if (confirmDeletePanel != null)
        {
            confirmDeletePanel.SetActive(true);
            
            // Clear previous listeners and add current slot's methods
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(OnConfirmDelete);
            }
            
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(OnCancelDelete);
            }
        }
    }

    public void OnConfirmDelete()
    {
        Inventory.instance.Remove(item);
        hasItem = false;
        if (confirmDeletePanel != null)
        {
            confirmDeletePanel.SetActive(false);
        }
    }

    public void OnCancelDelete()
    {
        if (confirmDeletePanel != null)
        {
            confirmDeletePanel.SetActive(false);
        }
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
            Debug.Log("Used item: " + item.itemName);
        }
    }
}
