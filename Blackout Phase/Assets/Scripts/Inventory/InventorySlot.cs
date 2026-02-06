// From https://youtu.be/YLhj7SfaxSE?si=Wm-SfEMXYx61skpm for Inventory slots
// Ellison
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;

    Item item;

    new public string name;
    public string type;
    public string description;
    public string flavorText;

    public bool hasItem = false;

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
        Inventory.instance.Remove(item);
        hasItem = false;
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }
}
