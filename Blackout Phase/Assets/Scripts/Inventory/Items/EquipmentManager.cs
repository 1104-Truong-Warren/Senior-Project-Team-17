// From https://youtu.be/d9oLS5hy0zU?si=aRchPZDA7vTQ6ELb for Equipment Manager
// Ellison

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager instance;

    public int numSlots = 3;

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



    public Equipment[] currentEquipment;

    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    Inventory inventory;

    void Start()
    {
        inventory = Inventory.instance;

        //int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    public bool Equip(Equipment newItem)
    {
        //int slotIndex = (int)newItem.equipSlot;

        // find leftmost empty slot
        int slotIndex = -1;
        for (int i = 0; i < numSlots; i++)
        {
            if (currentEquipment[i] == null)
            {
                slotIndex = i;
                break;
            }
        }

        Equipment oldItem = null;

        // return early if full
        if (slotIndex == -1)
        {
            Debug.LogWarning("No more equipment slots available!");
            return false;
        }


        /*if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            Unequip(slotIndex);
            inventory.Add(oldItem);
        }*/

        // equip new item
        currentEquipment[slotIndex] = newItem;

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        RecalculateTotalModifiers();
        ApplyModifiersToPlayer();
        Debug.Log("Equipped " + newItem.name + " in slot " + slotIndex);
        return true;
    }

    public void Unequip(int slotIndex)
    {
        /*
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
        }*/

        // sliding unequip
        if (slotIndex >= 0 && slotIndex < currentEquipment.Length && currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);

            // shift items to the left
            for (int i = slotIndex; i < currentEquipment.Length - 1; i++)
            {
                currentEquipment[i] = currentEquipment[i + 1];
            }
            currentEquipment[currentEquipment.Length - 1] = null;

            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
            RecalculateTotalModifiers();
            ApplyModifiersToPlayer();
            Debug.Log("Unequipped " + oldItem.name + " from slot " + slotIndex);
        }
    }

    public void UnequipAll()
    {
        for (int i = currentEquipment.Length - 1; i >= 0; i--)
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
