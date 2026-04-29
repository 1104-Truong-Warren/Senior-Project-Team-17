// From https://youtu.be/HQNl3Ff2Lpo?si=Tt6dFsqvx4NA5JgL for inventory system
// Ellison
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public InventoryData data;

    void Awake()
    {
        instance = this;
    }



    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public int maxItems = 6;

    public List<Item> items = new List<Item>();

    [Header("God Mode Items")]
    public Item godModeItem1;
    public Item godModeItem2;
    public Item godModeItem3;

    void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.G))
        {
            GetGodModeItems();
        }
    }
    public void Add(Item item)
    {
        data.Add(item);
    }

    public void Remove (Item item)
    {
        data.Remove(item);
    }

    public void GetGodModeItems()
    {
        Add(godModeItem1);
        Add(godModeItem2);
        Add(godModeItem3);
        Debug.Log("God Mode Items added to inventory.");
    }
}
