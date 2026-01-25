// Warren
// The purpose of this script is to add and control the main menu functionality, including starting the game by loading the main gameplay scene and existing the application when the user selects the quit button.
// Resource: https://www.youtube.com/watch?v=-GWjA6dixV4 - For setting up main menu and the implementation of start game button and quitting to desktop

using UnityEngine;
using UnityEngine.SceneManagement; // This built-in library is used to load and manage scenes, allow to switch between various and different scenes.

public class MainMenu : MonoBehaviour
{
    public void PlayGame() // Calls in Unity's built-in SceneMananger function, it will load the scene depending on the name.
    {
        SceneManager.LoadScene("Demo_pxiel_2D_Test_Grid");
    }

    // Later on, will implement a settings button, which will load another scene in which the user can adjust their play style, such as increasing volume, adjusting controls, enable camera functionalities, etc.

    public void QuitGame() // Calls in Unity's built-in Application and Quit function, when you press quit in the main menu, it will close the application completely, stopping all processes. 
    {
        Application.Quit();
        #if UNITY_EDITOR // Preprocessor directive, the code will only execute when the Unity Editor is running.
        UnityEditor.EditorApplication.isPlaying = false; // Stops play mode in the Unity Editor
        #endif // Ends the conditional compilation block.
    }
}
