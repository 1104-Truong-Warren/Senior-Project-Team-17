// Warren

// The purpose of this script is it allows the player to save the game by accessing the pause menu and press the "SAVE GAME" option.

// Source: https://youtu.be/1mf730eb5Wo?si=r3LT36KQ9M9q7Mbk - For foundation of save/load system and covers JSON serialization, creating save data classes, and managing save files.

using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    
    private string saveFolderPath;
    public List<SaveFileInfo> availableSaves = new List<SaveFileInfo>(); // List that holds data for all found save files
    
    void Awake()
    {
        // Checks to see of only one SaveManager exists, should only exist in the title screen
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFolderPath = Application.persistentDataPath + "/Saves/"; // Constructs path to the Saves folder
            
            // If the Saves folder doesn't exist, then it will create one automatically.
            if (!Directory.Exists(saveFolderPath)) 
                Directory.CreateDirectory(saveFolderPath);
            
            RefreshSaveList();

            // Checks if the file have been loaded, it is stored in PlayerPrefs by SaveSelectUI when the save is chosen.
            if (PlayerPrefs.HasKey("LoadFileName"))
            {
                string fileName = PlayerPrefs.GetString("LoadFileName");
                PlayerPrefs.DeleteKey("LoadFileName"); // Clears the file so that it only run once.
                LoadGame(fileName);
            }
        }
        else
        {
            // If another instance exists, it destroys it.
            Destroy(gameObject);
        }
    }
    
    // The purpose of this function is it scanes the Saves folder, reads every .txt file, and then creates SaveFileInfo objects for the load option in the menu.
    public void RefreshSaveList()
    {
        availableSaves.Clear();
        
        if (!Directory.Exists(saveFolderPath))
            return;
    
        string[] files = Directory.GetFiles(saveFolderPath, "*.txt");
        
        // Processes each file that is found
        foreach (string file in files)
        {
            try
            {
                // Reads the entire file and conver the JSON back to a PlayerSaveData object
                string json = File.ReadAllText(file);
                PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
                
                SaveFileInfo info = new SaveFileInfo
                {
                    fileName = Path.GetFileName(file),
                    fullPath = file,
                    saveDate = File.GetLastWriteTime(file),
                    playerHP = data.hp,
                    playerMaxHP = data.maxHP,
                };
                
                availableSaves.Add(info);
            }
            catch (Exception e)
            {
                // If the file didn't read
                Debug.LogWarning($"Failed to read save file {file}: {e.Message}");
            }
        }
    }
    
    // The purpose of this function is it is called by the pause menu's Save button to create a new save file.
    public void SaveGame()
    {
        CharacterInfo1 player = CharacterInfo1.Instance;
        if (player == null) return;
        
        string fileName = "Save_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt"; // Generates filename to by using the current date and time
        string fullPath = saveFolderPath + fileName;
        
        PlayerSaveData data = new PlayerSaveData(player); // Creates the data object that contins the player's current state.
        string json = JsonUtility.ToJson(data, true); // Converts the data object to a JSON string
        
        File.WriteAllText(fullPath, json); // Writes the JSON string to the new file
        Debug.Log("Game saved! HP: " + data.hp);
        
        RefreshSaveList(); // Updates the list of saves so the new file appears in the load option.
    }
    

    // The purpose of this function is it loads a spcific save file which is called from SaveSelectUI or Awake().
    public void LoadGame(string fileName)
    {
        string fullPath = saveFolderPath + fileName;
        
        if (!File.Exists(fullPath)) return;
        
        string json = File.ReadAllText(fullPath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        
        // Start the coroutine to wait for the player to spawn.
        StartCoroutine(LoadGameAfterSceneLoad(data));
    }
    

    // The purpose of this function/coroutine is that it waits for the player to exist in the scene, then it applies the data that will be loaded in.
    private IEnumerator LoadGameAfterSceneLoad(PlayerSaveData data)
    {
        // Wait for the scene to load
        yield return null;
        
        // Keep waiting until the player actually exists
        CharacterInfo1 player = null;
        int attempts = 0;
        
        while (player == null && attempts < 6000) // Set the time to 10 minutes to give player enough time to place character down.
        {
            player = CharacterInfo1.Instance;
            if (player == null)
            {
                // Try to find it by tag if Instance is null
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player1");
                if (playerObj != null)
                {
                    player = playerObj.GetComponent<CharacterInfo1>();
                }
            }
            
            if (player == null)
            {
                attempts++;
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        // If the player is found, applies the saved data.
        if (player != null)
        {
            player.LoadFromSaveData(data);
            Debug.Log($"Game loaded! HP: {data.hp}");
            
            // Updates positioning
            player.transform.position = new Vector3(data.posX, data.posY, data.posZ);
        }
        else
        {
            Debug.LogError("Failed to find player after 10 seconds!");
        }
    }
    
    // The purpose of this function is it gets the list of all availble saves.
    public List<SaveFileInfo> GetAvailableSaves()
    {
        return availableSaves;
    }

    // The purpose of this function is to permanently delete a save file from the computer's hard drive.
    public void DeleteSaveFile(string fullPath)
    {
        try
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log($"Deleted save file: {fullPath}");
                RefreshSaveList(); // Update the list after deletion
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete save file: {e.Message}");
        }
    }
}