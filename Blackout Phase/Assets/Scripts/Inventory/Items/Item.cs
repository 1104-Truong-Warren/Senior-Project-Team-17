// From https://youtu.be/HQNl3Ff2Lpo?si=5hqesTzYmGC-hFAd for inventory items
// Scriptable object for inventory items
// Ellison
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon  = null;
    public string type = "Default Type";
    public string description = "Default Description";
    public string flavorText = "Default Flavor Text";
    public bool isDefaultItem = false;

    // virtual function so that derived classes can override it
    public virtual void Use()
    {
        // Use the item
        // Something may happen
        Debug.Log("Using " + itemName);
    }

    public void RemoveFromInventory()
    {
        Inventory.instance.Remove(this);
    }
}
