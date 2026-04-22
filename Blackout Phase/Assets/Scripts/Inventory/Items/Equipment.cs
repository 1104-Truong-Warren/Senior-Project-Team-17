// From https://youtu.be/d9oLS5hy0zU?si=Fd8khTe9qatyt4LB for equipment items
// Another scriptable object that inherits from Item specifically for equipment
// Ellison
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    //public EquipmentSlot equipSlot;

    public int healthModifier;
    public int attackModifier;

    public override void Use()
    {
        base.Use();
        bool wasEquipped = EquipmentManager.instance.Equip(this);

        if (wasEquipped)
        {
            RemoveFromInventory();
        }
        else
        {
            Debug.Log("Couldn't equip " + itemName);
        }
    }
}

//public enum EquipmentSlot { Head, Arm1, Arm2, Chest, Legs }