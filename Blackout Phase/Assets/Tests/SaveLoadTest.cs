// Save and Load Player Data Testing - Warren 

using NUnit.Framework;
using UnityEngine;

public class SaveLoadTest
{
    [Test]
    public void ValidatePlayerStat()
    {
        // Created a mock player GameObject to act as the player. 
        // This simulates the player in the game scene.
        GameObject testPlayer = new GameObject("TestPlayer");
        CharacterInfo1 player = testPlayer.AddComponent<CharacterInfo1>();

        // Created a PlayerSaveData object with test stats. 
        // This simulates a saved game file with the values that are saved.
        PlayerSaveData dataToSave = new PlayerSaveData()
        {
            hp = 50,
            maxHP = 100,
            en = 30,
            maxEN = 50,
            baseAttk = 10,
            baseAttkRange = 5,
            baseHitRate = 80,
            baseCriticalRate = 20,
            baseCritDamage = 150,
            baseEvasion = 10,
            posX = 1f,
            posY = 2f,
            posZ = 3f
        };

        // Load the test data into the player. 
        // This simulates the player being loaded from a save file.
        player.LoadFromSaveData(dataToSave);

        // Verify that all stats were correctly applied onto the player. 
        // This ensures that the player's runtime values match the saved data.
        Assert.AreEqual(dataToSave.hp, player.CurrentHP, "HP should match");
        Assert.AreEqual(dataToSave.maxHP, player.maxHP, "Max HP should match");
        Assert.AreEqual(dataToSave.en, player.CurrentEN, "EN should match");
        Assert.AreEqual(dataToSave.maxEN, player.maxEN, "Max EN should match");
        Assert.AreEqual(dataToSave.baseAttk, player.BaseAttk, "Base Attack should match");
        Assert.AreEqual(dataToSave.baseAttkRange, player.BaseRange, "Attack Range should match");
        Assert.AreEqual(dataToSave.baseHitRate, player.BaseHitRate, "Hit Rate should match");
        Assert.AreEqual(dataToSave.baseCriticalRate, player.BaseCriticalRate, "Critical Rate should match");
        Assert.AreEqual(dataToSave.baseCritDamage, player.BaseCritDamage, "Crit Damage should match");
        Assert.AreEqual(dataToSave.baseEvasion, player.BaseEvasion, "Evasion should match");

        // Verify that the player's position matches the saved data.
        // Will show up invisible during testing.
        Assert.AreEqual(dataToSave.posX, player.transform.position.x, "X position should match");
        Assert.AreEqual(dataToSave.posY, player.transform.position.y, "Y position should match");
        Assert.AreEqual(dataToSave.posZ, player.transform.position.z, "Z position should match");

        // Destroy the temporary GameObject to prevent memory leak.
        Object.DestroyImmediate(testPlayer);
    }
}


