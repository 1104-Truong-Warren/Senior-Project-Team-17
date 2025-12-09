// This whole script is possibly unnecessary now, but keeping it as-is just in case

using UnityEngine;
using UnityEngine.UI;

public class CollapseButtonManager : MonoBehaviour
{
    public Button collapseButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Two sets of functions to make collapse button interactable or uninteractable
    // Helps avoid a situation where the user clicks the button during movement and cancelling it
    // Mostly affects MouseController.cs functionality
    public void CollapseButtonUninteractable()
    {
        collapseButton.interactable = false;
    }

    public void CollapseButtonInteractable()
    {
        collapseButton.interactable = true;
    }
}
