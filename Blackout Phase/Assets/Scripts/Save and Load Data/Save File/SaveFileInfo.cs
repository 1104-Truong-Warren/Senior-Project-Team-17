// Warren

// The purpose of this script is it defines the structure for displaying the save file information in the load option in the title screen.
// It stores the data about each save file without loading the game data.
// It is used by SaveSelectUI.cs to create the list of available saves.

// Source: https://learn.microsoft.com/en-us/dot/csharp/programming-guide/classes-and-structs/auto-implemented-properties - For property syntax
// Source: https://learn.microsoft.com/en-us/dot/standard/base-types/standard-date-and-time-format-strings - For date formatting
// Source: https://docs.unity3d.com/Manual/JSONSerialization.html - For serializable classes
// Source: https://discussions.unity.com/t/saving-and-loading-json-data/885118 - For checking out more game scenarios of how data is saved to file

using System;

[System.Serializable]
public class SaveFileInfo
{
    public string fileName;
    public string fullPath;
    public DateTime saveDate; // The data and time  of when the file was saved
    public int playerHP;
    public int playerMaxHP;
    public string sceneName;
    
    public string DisplayDate => saveDate.ToString("yyyy-MM-dd HH:mm"); // To save the data using Year-Month-Day Hour:Minute format
    public string DisplayInfo => $"HP: {playerHP}/{playerMaxHP}";
}