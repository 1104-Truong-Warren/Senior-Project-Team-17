// script for dialogue, put script on dialogue box
// parts from https://youtu.be/8oTYabhj248?si=gCoLGAp91jRGE_xI for basic setup
// - Ellison
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public float textSpeed;
    public bool dialogueDone = false;

    //public string[] lines;
    public int index;

    // trying to use the dialogue asset scriptable object
    public DialogueAsset dialogueAsset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //textComponent.enabled = true;
        //StartCoroutine(InitializeDialogue()); // start the dialogue initialization
    }

    IEnumerator InitializeDialogue()
    {
        yield return 3f;
        textComponent.text = string.Empty; // set the text to empty at the start
        StartDialogue(); // start the dialogue
    }

    public void Reinitialize(DialogueAsset newDialogueAsset)
    {
        StopAllCoroutines(); // stop any ongoing coroutines

        // Reset all states
        dialogueAsset = newDialogueAsset; // update the dialogue asset
        index = 0; // reset the index
        textComponent.text = string.Empty; // clear the text
        dialogueDone = false; // reset the dialogue done flag

        // Start dialogue again
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) // if left mouse button is clicked
        {
            if(textComponent.text == dialogueAsset.dialogue[index]) // if the current line is fully displayed
            {
                NextLine(); // move to the next line
            }
            else
            {
                StopAllCoroutines(); // stop the typing effect
                textComponent.text = dialogueAsset.dialogue[index]; // display the full line immediately
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // Clear text before typing the new line
        textComponent.text = string.Empty;

        // Type each character one by one with a delay of textSpeed seconds
        foreach (char c in dialogueAsset.dialogue[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < dialogueAsset.dialogue.Length - 1)
        {
            index++;
            textComponent.text = string.Empty; // clear the text for the next line
            StartCoroutine(TypeLine()); // start typing the next line
        }
        else
        {
            gameObject.SetActive(false); // deactivate the dialogue box when all lines are done
            dialogueDone = true; // set the dialogue done flag to true
        }
    }

    public int GetCurrentLineIndex()
    {
        return index;
    } 
}
