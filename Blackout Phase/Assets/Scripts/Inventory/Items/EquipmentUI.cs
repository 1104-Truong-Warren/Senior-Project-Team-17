using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public Transform equipmentParent;
    EquipmentManager equipmentManager;
    EquipmentMenuSlot[] slots;

    public InventoryData data;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // link to instance
        equipmentManager = EquipmentManager.instance;

        // subscribe to event and changes
        data = equipmentManager.data;
        data.onEquipmentChanged += UpdateUI;

        // find slots
        slots = equipmentParent.GetComponentsInChildren<EquipmentMenuSlot>();

        // force UI refresh
        UpdateUI(null, null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateUI(Equipment newItem, Equipment oldItem)
    {
        /*
        for (int i = 0; i < slots.Length; i++)
        {
            if (newItem != null && (int)newItem.equipSlot == i)
            {
                slots[i].EquipItem(newItem);
            }
            else if (oldItem != null && (int)oldItem.equipSlot == i)
            {
                slots[i].UnequipItem();
            }
        }*/

        Equipment[] currentEquipment = data.currentEquipment;

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < currentEquipment.Length && currentEquipment[i] != null)
            {
                slots[i].EquipItem(currentEquipment[i], i);
            }
            else
            {
                slots[i].UnequipItem();
            }
        }
    }

    void OnDestroy()
    {
        if (data != null)
        {
            data.onEquipmentChanged -= UpdateUI;
        }
    }
}
