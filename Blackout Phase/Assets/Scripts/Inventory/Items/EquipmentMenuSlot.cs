// Inspired by Inventory Slot and reference video but not directly from it
// Ellison
using UnityEngine;
using UnityEngine.UI;

public class EquipmentMenuSlot : MonoBehaviour
{
    public Image icon;

    Item item;

    public void EquipItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void UnequipItem()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnUnequip()
    {
        if (item != null && item is Equipment equipmentItem)
        {
            EquipmentManager.instance.Unequip((int)equipmentItem.equipSlot);
        }
    }
}
