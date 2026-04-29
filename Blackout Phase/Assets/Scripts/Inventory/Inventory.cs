// From https://youtu.be/HQNl3Ff2Lpo?si=Tt6dFsqvx4NA5JgL for inventory system
// Ellison
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found!");
            return;
        }
        
        instance = this;
    }



    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int maxItems = 6;

    public List<Item> items = new List<Item>();

    public void Add(Item item)
    {
        if (items.Count >= maxItems)
        {
            Debug.Log("Inventory is full. Cannot add " + item.itemName);
            return;
        }
        else
        {
            items.Add(item);
            Debug.Log("Added " + item.itemName + " to inventory.");
            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
            return;
        }
    }

    public void Remove (Item item)
    {
        items.Remove(item);
        Debug.Log("Removed " + item.itemName + " from inventory.");
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}
