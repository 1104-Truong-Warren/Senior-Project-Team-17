// From https://youtu.be/HQNl3Ff2Lpo?si=5hqesTzYmGC-hFAd for inventory items
// Scriptable object for inventory items
// Ellison
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon  = null;
    public bool isDefaultItem = false;
}
