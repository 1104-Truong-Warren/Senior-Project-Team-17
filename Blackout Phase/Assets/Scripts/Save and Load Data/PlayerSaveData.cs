// Warren

// The purpose of this script is it defines the data structure for saving and loading the character's information.
// It stores all of the stats and positioning from CharacterInfo.cs
// It can be converted to JSON and saved to a txt file.
// It also loads the scene when where the player saved.

// Source: https://docs.unity3d.com/Manual/JSONSerialization.html - For JsonUtility and serialization
// Source: https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html - For save file location
// Source: https://learn.microsoft.com/en-us/dot/standard/serialization/system-text-json/overview - For JSON data structure

using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlayerSaveData
{
    public string sceneName;

    // Player stats from CharacterInfo1
    public int hp;
    public int maxHP;
    public int en;
    public int maxEN;
    public int baseAttk;
    public int baseAttkRange;
    public int baseHitRate;
    public int baseCriticalRate;
    public int baseCritDamage;
    public int baseEvasion;
    public int level;

    // Position
    public float posX;
    public float posY;
    public float posZ;

    // Ellison - added to also load inventory
    public List<string> inventoryItemNames = new List<string>();
    public string[] equippedItemNames = new string[3];

    // Empty constructor needed for loading
    public PlayerSaveData() { }
    
    // Constructor that grabs data from your player
    public PlayerSaveData(CharacterInfo1 player, InventoryData invData)
    {
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        hp = player.CurrentHP;
        maxHP = player.MaxHP;
        en = player.CurrentEN;
        maxEN = player.MaxEN;
        baseAttk = player.BaseAttack;
        baseAttkRange = player.AttackRange;
        baseHitRate = player.HitRate;
        baseCriticalRate = player.BaseCriticalRate;
        baseCritDamage = player.BaseCritDamage;
        baseEvasion = player.EvasionRate;
        level = player.CurrentLevel;

        // Save position
        posX = player.transform.position.x;
        posY = player.transform.position.y;
        posZ = player.transform.position.z;


        // Ellison - inventory
        foreach (Item item in invData.items)
        {
            if (item != null) inventoryItemNames.Add(item.name);
        }
        for (int i = 0; i < invData.currentEquipment.Length; i++)
        {
            if (invData.currentEquipment[i] != null)
                equippedItemNames[i] = invData.currentEquipment[i].name;
        }
    }
}