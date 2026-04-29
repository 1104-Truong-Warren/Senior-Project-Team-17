// Inspired by Inventory Slot and reference video but not directly from it
// Ellison
using UnityEngine;
using UnityEngine.UI;

public class EquipmentMenuSlot : MonoBehaviour
{
    public Image icon;

    Item item;

    public int slotIndex;

    new public string name;
    public string type;
    public string description;
    public string flavorText;

    public bool hasItem = false;

    public void EquipItem(Item newItem, int index)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;

        slotIndex = index;

        name = item.itemName;
        type = item.type;
        description = item.description;
        flavorText = item.flavorText;

        hasItem = true;
    }

    public void UnequipItem()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;

        name = "";
        type = "";
        description = "";
        flavorText = "";

        hasItem = false;
    }

    public void OnUnequip()
    {
        if (item != null)
        {
            EquipmentManager.instance.Unequip(slotIndex);
        }
    }
}
