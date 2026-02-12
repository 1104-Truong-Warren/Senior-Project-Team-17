// From https://youtu.be/d9oLS5hy0zU?si=aRchPZDA7vTQ6ELb for Equipment Manager
// Ellison

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;


    public int previousTotalHealthModifier = 0;
    public int previousTotalAttackModifier = 0;
    public int currentTotalHealthModifier = 0;
    public int currentTotalAttackModifier = 0;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EquipmentManager found!");
            return;
        }
        instance = this;
    }



    Equipment[] currentEquipment;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    Inventory inventory;

    void Start()
    {
        inventory = Inventory.instance;

        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;

        Equipment oldItem = null;

        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
        }

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        currentEquipment[slotIndex] = newItem;
        RecalculateTotalModifiers();
        ApplyModifiersToPlayer();
        Debug.Log("Equipped " + newItem.name + " in slot " + newItem.equipSlot);
    }

    public void Unequip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
            currentEquipment[slotIndex] = null;

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }

            RecalculateTotalModifiers();
            ApplyModifiersToPlayer();
            Debug.Log("Unequipped " + oldItem.name + " from slot " + oldItem.equipSlot);
        }
    }

    public void UnequipAll()
    {
        for (int i = 0; i < currentEquipment.Length; i++)
        {
            Unequip(i);
        }
    }

    // function to recalculate total modifiers based on currently equipped items, called after equipping or unequipping
    public void RecalculateTotalModifiers()
    {
        previousTotalHealthModifier = currentTotalHealthModifier;
        previousTotalAttackModifier = currentTotalAttackModifier;

        currentTotalHealthModifier = 0;
        currentTotalAttackModifier = 0;

        foreach (Equipment equipment in currentEquipment)
        {
            if (equipment != null)
            {
                currentTotalHealthModifier += equipment.healthModifier;
                currentTotalAttackModifier += equipment.attackModifier;
            }
        }
    }

    // another function kept separate from recalculating modifiers to reapply modifiers to player
    // best way I could think of doing this was subtracting the previous modifiers then adding the new ones
    // seems better than trying to go relative to player base health since that can change from other sources
    public void ApplyModifiersToPlayer()
    {
        CharacterInfo1 player = CharacterInfo1.Instance;
        if (player != null)
        {
            player.DecreaseMaxHP(previousTotalHealthModifier);
            player.DecreaseAttack(previousTotalAttackModifier);

            player.IncreaseMaxHP(currentTotalHealthModifier);
            player.IncreaseAttack(currentTotalAttackModifier);
        }
    }

}
