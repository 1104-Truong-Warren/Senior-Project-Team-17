// Inspired by Inventory Slot and reference video but not directly from it
// Ellison
using UnityEngine;
using UnityEngine.UI;

public class EquipmentMenuSlot : MonoBehaviour
{
    public Image icon;

    Item item;

    public int slotIndex;

    public void EquipItem(Item newItem, int index)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;

        slotIndex = index;
    }

    public void UnequipItem()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnUnequip()
    {
        if (item != null)
        {
            EquipmentManager.instance.Unequip(slotIndex);
        }
    }
}
