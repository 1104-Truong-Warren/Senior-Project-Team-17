// Warren
// The purpose of this script is to manage the save files that appears when the player presses "LOAD GAME" in the main menu.
// It displays all of the available save fiels as clickable buttons, and it allows the player to choose which save file to load.
// When a save is selected, it stores the filename and then loads the game scene will all updated stats from the player that were previously saved.

// Source: https://www.youtube.com/watch?v=7R3OL8C0SFc - For setting up the display of loaded files in the menu
// Source: https://docs.unity3d.com/ScriptReference/PlayerPrefs.html - For storing the selected filename between scenes
// Source: https://docs.unity3d.com/ScriptReference/Object.Instantiate.html - For creating dynamic buttons from prefabs
// Source: https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/script-ScrollRect.html - For creating vertical scroll as menu expands from more saves

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveSelectUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject saveSelectPanel;
    [SerializeField] private Transform saveButtonContainer;
    [SerializeField] private GameObject saveButtonPrefab;
    [SerializeField] private Button backButton;
    
    private SaveManager saveManager; // Reference to the SaveManager to access save files.
    
    void Start()
    {
        // Finds the SaveManager in the scene in TitleScreen
        saveManager = FindObjectOfType<SaveManager>();
        
        // Back button to close the panel
        if (backButton != null)
        {
            backButton.onClick.AddListener(CloseSaveSelect);
        }
            
        if (saveSelectPanel != null)
        {
            saveSelectPanel.SetActive(false);
        }

    }
    
    // The purpose of this function is it is called by the MainMenu when the player press "LOAD GAME" and refreshes the list of saves.
    public void OpenSaveSelect()
    {
        saveManager = FindObjectOfType<SaveManager>();
        if (saveManager != null)
        {
            saveManager.RefreshSaveList();
        }
        
        PopulateSaveList();
        saveSelectPanel.SetActive(true);
    }
    
    // The purpose of this function is it closes the selection panel
    public void CloseSaveSelect()
    {
        saveSelectPanel.SetActive(false);
    }
    
    // The purpose of this function is it creatse buttons for each file that is found in the Saves folder.
    private void PopulateSaveList()
    {
        // Ensure proper spacing
        VerticalLayoutGroup layout = saveButtonContainer.GetComponent<VerticalLayoutGroup>();
        if (layout != null)
        {
            layout.spacing = 100f; // Set your desired spacing
        }
        
        // Clears any existing buttons from previous time the panel was opened.
        foreach (Transform child in saveButtonContainer)
            Destroy(child.gameObject);
        
        // Finds the SaveManager again to ensure that we have latest reference
        saveManager = FindObjectOfType<SaveManager>();
        if (saveManager == null) return;
        
        // Gets the list of save files from SaveManager
        List<SaveFileInfo> saves = saveManager.GetAvailableSaves();
        
        // If there are no saves, no it will show a message instead of buttons.
        if (saves.Count == 0)
        {
            GameObject msg = Instantiate(saveButtonPrefab, saveButtonContainer);
            msg.GetComponentInChildren<TextMeshProUGUI>().text = "No save files found";
            msg.GetComponent<Button>().interactable = false;
            return;
        }
        
        // Creates button for each save file
        foreach (SaveFileInfo save in saves)
        {
            GameObject buttonObj = Instantiate(saveButtonPrefab, saveButtonContainer);
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            buttonText.text = $"{save.fileName}\n{save.DisplayDate} - HP: {save.playerHP}/{save.playerMaxHP}";
            
            Button button = buttonObj.GetComponent<Button>();
            string fileName = save.fileName;
            button.onClick.AddListener(() => LoadSelectedSave(fileName));
        }
    }
    
    // The purpose of this function is when the save file button is clicked, then it stores the filename and loads the game scane.
    private void LoadSelectedSave(string fileName)
    {
        // Store the selected filename in PlayerPrefs so SaveManager can access it after the scene loads
        PlayerPrefs.SetString("LoadFileName", fileName);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Demo_pxiel_2D_Test_Grid");
        saveSelectPanel.SetActive(false); // Hides save selection panel
    }
}