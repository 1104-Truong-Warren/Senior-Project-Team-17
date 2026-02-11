// From https://youtu.be/d9oLS5hy0zU?si=Fd8khTe9qatyt4LB for equipment items
// Another scriptable object that inherits from Item specifically for equipment
// Ellison
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipSlot;

    public int healthModifier;
    public int attackModifier;

    public override void Use()
    {
        base.Use();
        EquipmentManager.instance.Equip(this);
        RemoveFromInventory();
    }
}

public enum EquipmentSlot { Head, Arm1, Arm2, Chest, Legs }