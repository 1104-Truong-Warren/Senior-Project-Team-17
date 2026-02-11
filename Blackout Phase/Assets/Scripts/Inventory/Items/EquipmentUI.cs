using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public Transform equipmentParent;
    EquipmentManager equipmentManager;
    EquipmentMenuSlot[] slots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        equipmentManager = EquipmentManager.instance;
        equipmentManager.onEquipmentChanged += UpdateUI;

        slots = equipmentParent.GetComponentsInChildren<EquipmentMenuSlot>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateUI(Equipment newItem, Equipment oldItem)
    {
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
        }
    }
}
