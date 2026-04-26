// Warren

// The purpose of the script is it is used to select a level after the player presses "Play" in the main menu.
// This allows the user to select any level they wish to play, currently all of them is unlocked.

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    // Load Tutorial Level
    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    
    // Level 1 Button
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1"); 
    }
    
    // Level 2 Button
    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2"); 
    }

    // Level 3 Button
    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level3"); 
    }

    // Level 4 Button
    public void LoadLevel4()
    {
        SceneManager.LoadScene("Level4"); 
    }
    
    // Back to Main Menu
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}