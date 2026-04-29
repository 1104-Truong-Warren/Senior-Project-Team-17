// From https://youtu.be/d9oLS5hy0zU?si=aRchPZDA7vTQ6ELb for Equipment Manager
// Ellison

using System;
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

    public InventoryData data;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        data.onEquipmentChanged += OnEquipmentChanged;
    }

    public bool Equip(int inventoryIndex)
    {
        return data.Equip(inventoryIndex);
    }

    public void Unequip(int slotIndex)
    {
        data.Unequip(slotIndex);
    }

    public void UnequipAll()
    {
        for (int i = data.currentEquipment.Length - 1; i >= 0; i--)
        {
            Unequip(i);
        }
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        RecalculateTotalModifiers();
        ApplyModifiersToPlayer();
    }

    // function to recalculate total modifiers based on currently equipped items, called after equipping or unequipping
    public void RecalculateTotalModifiers()
    {
        previousTotalHealthModifier = currentTotalHealthModifier;
        previousTotalAttackModifier = currentTotalAttackModifier;

        currentTotalHealthModifier = 0;
        currentTotalAttackModifier = 0;

        foreach (Equipment equipment in data.currentEquipment)
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
