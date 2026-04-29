// Ellison
// saves inventory and equipment data
using System.Collections.Generic;
using UnityEngine;
using static EquipmentManager;

[CreateAssetMenu(fileName = "NewInventoryData", menuName = "Inventory/Data Container")]
public class InventoryData : ScriptableObject
{
    [Header("Inventory Data")]
    public List<Item> items = new List<Item>();
    public int maxItems = 9;

    [Header("Equipment Data")]
    public Equipment[] currentEquipment = new Equipment[3];

    public System.Action onItemChanged;
    public System.Action<Equipment, Equipment> onEquipmentChanged;

    // inventory
    public void Add(Item item)
    {
        if (items.Count >= maxItems)
        {
            Debug.Log("Inventory is full.");
            return;
        }
        items.Add(item);
        onItemChanged?.Invoke();
    }

    public void Remove(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            onItemChanged?.Invoke();
        }
    }

    public void RemoveAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);
            onItemChanged?.Invoke();
        }
    }

    // equipment

    public bool Equip(int inventoryIndex)
    {
        // fetch item
        Equipment newItem = items[inventoryIndex] as Equipment;

        // find first open slot
        int slotIndex = -1;
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            if (currentEquipment[i] == null)
            {
                slotIndex = i;
                break;
            }
        }

        if (slotIndex == -1) return false;

        // Remove from inventory list first
        items.RemoveAt(inventoryIndex);

        // put in current equipment
        currentEquipment[slotIndex] = newItem;

        // refresh
        onItemChanged?.Invoke();
        onEquipmentChanged?.Invoke(newItem, null);
        return true;
    }

    public void Unequip(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < currentEquipment.Length && currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];

            // Shift items left
            for (int i = slotIndex; i < currentEquipment.Length - 1; i++)
            {
                currentEquipment[i] = currentEquipment[i + 1];
            }
            currentEquipment[currentEquipment.Length - 1] = null;

            // Add back to inventory
            Add(oldItem);
            onEquipmentChanged?.Invoke(null, oldItem);
        }
    }


}
