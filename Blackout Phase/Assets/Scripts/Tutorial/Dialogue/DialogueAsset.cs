// parts from https://gamedevbeginner.com/dialogue-systems-in-unity/#dialogue_authoring for scriptable objects to store dialogue
// Ellison
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialogue Asset", menuName = "Dialogue/Dialogue Asset")]
public class DialogueAsset : ScriptableObject
{
    [TextArea]
    public string[] dialogue;
}
