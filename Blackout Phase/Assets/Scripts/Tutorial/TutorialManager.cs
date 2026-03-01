// Manages the entire tutorial sequence
// Ellison
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


// enum to track current step of the tutorial
public enum TutorialStep
{
    Blackscreen,
    CameraMovementIntro,
    CameraMovement,
    PlayerMovementIntro,
    PlayerMovement,
    Complete
}

public class TutorialManager : MonoBehaviour
{
    public GameObject dialoguePanel; // default position 670 top, 110 bottom
    // moved position is 600 top, 180 bottom
    public Dialogue dialogueBox; // box ALL dialogue gets loaded into
    public TutorialMouseController mouseController; // for controlling the mouse and player movement
    public GameObject blackscreen; 
    public DragCamera2D cameraController;
    public Sprite completeBox;

    public GameObject stepCompletePanel;

    public AudioSource bigStepCompleteSource; // on the canvas

    // Blackscreen Step
    public DialogueAsset blackscreenDialogue;

    // CameraMovementIntro Step
    public DialogueAsset cameraMovementIntroDialogue;

    // CameraMovement Step
    public GameObject cameraMovementPanel;
    public GameObject panStatusImage;
    public GameObject zoomStatusImage;

    // PlayerMovementIntro
    public DialogueAsset playerMovementIntroDialogue;
    public GameObject actionMenu;

    // Dictionary to map tutorial steps to their panel's AudioSource
    private Dictionary<TutorialStep, AudioSource> stepAudioSources = new Dictionary<TutorialStep, AudioSource>();

    public TutorialStep currentStep = TutorialStep.Blackscreen;
    public bool currentStepComplete = false;

    // Track which tasks have been marked complete to avoid repeated calls
    private bool zoomMarkedComplete = false;
    private bool panMarkedComplete = false;

    private bool stepMarkedComplete = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeAudioSources();
        StartCoroutine(RunTutorial());
    }

    // Initialize the audio sources for each step's panel
    private void InitializeAudioSources()
    {
        // Get the AudioSource from each step's panel
        // For CameraMovement step, get the AudioSource from tutorialMovementPanel
        AudioSource cameraMovementAudio = cameraMovementPanel.GetComponent<AudioSource>();
        if (cameraMovementAudio != null)
        {
            stepAudioSources[TutorialStep.CameraMovement] = cameraMovementAudio;
        }
        else
        {
            Debug.LogWarning("No AudioSource found on tutorialMovementPanel");
        }

        // Add more steps as needed
        // Example:
        // AudioSource blackscreenAudio = blackscreen.GetComponent<AudioSource>();
        // if (blackscreenAudio != null)
        // {
        //     stepAudioSources[TutorialStep.Blackscreen] = blackscreenAudio;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (!currentStepComplete)
        {
            CheckStepCompletion();
        }
    }

    private IEnumerator RunTutorial()
    {
        while (currentStep != TutorialStep.Complete)
        {
            yield return ExecuteStep(currentStep);
            ProgressToNextStep();
        }

        OnTutorialComplete();
    }

    private IEnumerator ExecuteStep(TutorialStep step)
    {
        currentStepComplete = false;

        switch (step)
        {
            case TutorialStep.Blackscreen:
                blackscreen.SetActive(true);
                yield return StartCoroutine(RunBlackscreenStep(dialogueBox));
                blackscreen.SetActive(false);
                break;
            case TutorialStep.CameraMovementIntro:
                yield return StartCoroutine(RunDialogueMovementIntroStep(dialogueBox));
                break;

            case TutorialStep.CameraMovement:
                yield return StartCoroutine(RunCameraMovementStep());
                break;

            case TutorialStep.PlayerMovementIntro:
                yield return StartCoroutine(RunPlayerMovementIntroStep(dialogueBox));
                break;

            case TutorialStep.Complete:
                break;
        }
    }

    private IEnumerator RunBlackscreenStep(Dialogue dialogue)
    {
        dialogue.Reinitialize(blackscreenDialogue); // set the dialogue asset for the blackscreen dialogue
        dialogue.gameObject.SetActive(true);
        // Wait until dialogue is done (check the dialogueDone flag)
        yield return new WaitUntil(() => dialogue.dialogueDone);
        currentStepComplete = true;
    }

    private IEnumerator RunDialogueMovementIntroStep(Dialogue dialogue)
    {
        dialogue.Reinitialize(cameraMovementIntroDialogue); // set the dialogue asset for the movement intro dialogue
        dialogue.gameObject.SetActive(true);

        // Wait until dialogue is done (also check for certain index to show mission panel)
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 5)
            {
                // Make mission panel appear because dialogue talks about it
                cameraMovementPanel.SetActive(true);
            }
            yield return null;
        }
        currentStepComplete = true;
    }

    private IEnumerator RunCameraMovementStep()
    {
        // Show camera movement instructions/panel (redundant now)
        cameraMovementPanel.SetActive(true);

        // Wait for player to move camera
        yield return new WaitUntil(() => currentStepComplete);
    }

    private IEnumerator RunPlayerMovementIntroStep(Dialogue dialogue)
    {
        dialogue.Reinitialize(playerMovementIntroDialogue); // set the dialogue asset for the movement intro dialogue
        dialogue.gameObject.SetActive(true);
        // Wait until dialogue is done (also check for certain index to show action menu)
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
            }
            if (dialogue.GetCurrentLineIndex() == 2)
            {
                // Move dialogue box up to make room for action menu
                RectTransform rectTransform = dialoguePanel.GetComponent<RectTransform>();
                rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 180); // Set bottom offset
                rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -600); // Set top offset (negative)
                actionMenu.SetActive(true);
            }
            yield return null;
        }
        currentStepComplete = true;
    }

    private void CheckStepCompletion()
    {
        switch (currentStep)
        {
            case TutorialStep.CameraMovement:
                if (cameraController.hasDoneZoom && !zoomMarkedComplete)
                {
                    markTaskComplete(zoomStatusImage);
                    zoomMarkedComplete = true;
                }
                if (cameraController.hasDonePan && !panMarkedComplete)
                {
                    markTaskComplete(panStatusImage);
                    panMarkedComplete = true;
                }
                if (cameraController.hasDoneZoom && cameraController.hasDonePan && !stepMarkedComplete)
                {
                    StartCoroutine(CompleteStepWithDelay(currentStep, 2f)); // Pass current step
                    stepMarkedComplete = true;
                }
                break;
        }
    }

    private IEnumerator CompleteStepWithDelay(TutorialStep step, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Hide the current step's panel
        HideStepPanel(step);
        
        // Show the step complete panel
        stepCompletePanel.SetActive(true);
        
        // Play the big step complete audio
        bigStepCompleteSource.Play();
        
        // Mark step as complete
        currentStepComplete = true;
    }

    private void HideStepPanel(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.CameraMovement:
                cameraMovementPanel.SetActive(false);
                break;
            case TutorialStep.CameraMovementIntro:
                // Add panel hiding logic for this step if needed
                break;
            case TutorialStep.Blackscreen:
                // Blackscreen is already handled in ExecuteStep
                break;
        }
    }

    private void ProgressToNextStep()
    {
        currentStep = currentStep switch
        {
            TutorialStep.Blackscreen => TutorialStep.CameraMovementIntro,
            TutorialStep.CameraMovementIntro => TutorialStep.CameraMovement,
            TutorialStep.CameraMovement => TutorialStep.PlayerMovementIntro,
            TutorialStep.PlayerMovementIntro => TutorialStep.PlayerMovement,
            TutorialStep.PlayerMovement => TutorialStep.Complete,
            _ => TutorialStep.Complete
        };

        Debug.Log($"Tutorial progressed to: {currentStep}");
    }

    private void OnTutorialComplete()
    {
        Debug.Log("Tutorial Complete!");
        // Handle tutorial completion - disable tutorial systems, enable normal gameplay, etc.
    }

    private void markTaskComplete(GameObject statusImage)
    {
        statusImage.GetComponent<Image>().sprite = completeBox;
        
        // Get the AudioSource for the current step and play it
        if (stepAudioSources.ContainsKey(currentStep) && stepAudioSources[currentStep] != null)
        {
            stepAudioSources[currentStep].Play();
        }
    }
}
