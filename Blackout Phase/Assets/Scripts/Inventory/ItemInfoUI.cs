// Updates the item pop up UI when the player hovers over an item in the inventory
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUI : MonoBehaviour
{
    public GameObject UIObject;

    public Image icon;
    new public TextMeshProUGUI name;
    public TextMeshProUGUI type;
    public TextMeshProUGUI description;
    public TextMeshProUGUI flavorText;

    Item item;

    /*public void Awake()
    {
        UIObject = GameObject.Find("ItemInfoPopup");
        UIObject.SetActive(false);
    }*/

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //itemInfoPopup.onMouseEnterCallback += UpdateUI;
        UIObject = GameObject.Find("ItemInfoPopup");
        UIObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI(Sprite slotIcon, string slotName, string slotType, string slotDescription, string slotFlavorText)
    {
        icon.enabled = true;
        icon.sprite = slotIcon;
        name.text = slotName;
        type.text = slotType;
        description.text = slotDescription;
        flavorText.text = slotFlavorText;
    }
}
