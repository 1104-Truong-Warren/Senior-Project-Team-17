// Warren

// The purpose of this script is it defines the data structure for saving and loading the character's information.
// It stores all of the stats and positioning from CharacterInfo.cs
// It can be converted to JSON and saved to a txt file.

// Source: https://docs.unity3d.com/Manual/JSONSerialization.html - For JsonUtility and serialization
// Source: https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html - For save file location
// Source: https://learn.microsoft.com/en-us/dot/standard/serialization/system-text-json/overview - For JSON data structure

using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
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
    
    // Position
    public float posX;
    public float posY;
    public float posZ;
    
    // Empty constructor needed for loading
    public PlayerSaveData() { }
    
    // Constructor that grabs data from your player
    public PlayerSaveData(CharacterInfo1 player)
    {
        hp = player.CurrentHP;
        maxHP = player.maxHP;
        en = player.CurrentEN;
        maxEN = player.maxEN;
        baseAttk = player.BaseAttk;
        baseAttkRange = player.BaseRange;
        baseHitRate = player.BaseHitRate;
        baseCriticalRate = player.BaseCriticalRate;
        baseCritDamage = player.BaseCritDamage;
        baseEvasion = player.BaseEvasion;
        
        // Save position
        posX = player.transform.position.x;
        posY = player.transform.position.y;
        posZ = player.transform.position.z;
    }
}