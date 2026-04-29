// From https://youtu.be/YLhj7SfaxSE?si=Wm-SfEMXYx61skpm for Inventory UI
// Ellison
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public Button confirmButton;
    public Button cancelButton;
    
    Inventory inventory;
    InventorySlot[] slots;

    public InventoryData data;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data.onItemChanged += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        // Initialize all slots with button references
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetConfirmationButtons(confirmButton, cancelButton);
        }

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < data.items.Count)
            {
                slots[i].AddItem(data.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    void OnDestroy()
    {
        if (data != null)
        {
            data.onItemChanged -= UpdateUI;
        }
    }
}
