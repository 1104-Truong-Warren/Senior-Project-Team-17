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

    //public string[] lines;
    private int index;

    // trying to use the dialogue asset scriptable object
    public DialogueAsset dialogueAsset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //textComponent.enabled = true;
        StartCoroutine(InitializeDialogue()); // start the dialogue initialization
    }

    IEnumerator InitializeDialogue()
    {
        yield return 3f;
        textComponent.text = string.Empty; // set the text to empty at the start
        StartDialogue(); // start the dialogue
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
        }
    }
}
