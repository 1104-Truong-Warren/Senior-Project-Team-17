// Controls behavior for when the mouse pointer is over a specific UI element
// Ellison
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInfoPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int mouseOnCount = 0;

    public GameObject inventorySlot;
    public GameObject UIObject;

    public ItemInfoUI itemInfoUI;

    public Sprite currentSlotSprite;
    public string currentSlotName;
    public string currentSlotType;
    public string currentSlotDescription;
    public string currentSlotFlavorText;

    public delegate void OnMouseEnter(Image image, string type, string description, string flavorText);
    public OnMouseEnter onMouseEnterCallback;

    public void Awake()
    {
        UIObject = GameObject.Find("ItemInfoPopup");
        itemInfoUI = GameObject.Find("ItemInfoPopup").GetComponent<ItemInfoUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOnCount = mouseOnCount + 1;

        if (inventorySlot.GetComponent<InventorySlot>().hasItem)
        {
            UIObject.SetActive(true);

            currentSlotSprite = inventorySlot.GetComponent<InventorySlot>().icon.sprite;
            currentSlotName = inventorySlot.GetComponent<InventorySlot>().name;
            currentSlotType = inventorySlot.GetComponent<InventorySlot>().type;
            currentSlotDescription = inventorySlot.GetComponent<InventorySlot>().description;
            currentSlotFlavorText = inventorySlot.GetComponent<InventorySlot>().flavorText;

            //onMouseEnterCallback.Invoke(currentSlotImage, currentSlotType, currentSlotDescription, currentSlotFlavorText);
            itemInfoUI.UpdateUI(currentSlotSprite, currentSlotName, currentSlotType, currentSlotDescription, currentSlotFlavorText);
        }
        
        Debug.Log(mouseOnCount);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UIObject.SetActive(false);
    }
}
