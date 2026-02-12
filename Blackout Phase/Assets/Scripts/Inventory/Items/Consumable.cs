// Consumable class that inherits from Item
// Ellison
using System.Collections;   
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
    public int healthChangeAmount;

    public override void Use()
    {
        base.Use();
        CharacterInfo1.Instance.RestoreHP(healthChangeAmount);
        RemoveFromInventory();
    }
}
