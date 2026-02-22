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
    DialogueMovementIntro,
    CameraMovement,
    Complete
}

public class TutorialManager : MonoBehaviour
{
    public Dialogue dialogueBox; // box ALL dialogue gets loaded into
    public TutorialMouseController mouseController; // for controlling the mouse and player movement
    public GameObject blackscreen; 
    public DragCamera2D cameraController;
    public Sprite incompleteBox;
    public Sprite completeBox;

    public DialogueAsset blackscreenDialogue;
    public DialogueAsset movementIntroDialogue;

    // Movement tutorial
    public GameObject tutorialMovementPanel; // panel for movement tutorial
    public GameObject panStatusImage;
    public GameObject zoomStatusImage;

    [SerializeField] public TutorialStep currentStep = TutorialStep.Blackscreen;
    public bool currentStepComplete = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(RunTutorial());
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
            case TutorialStep.DialogueMovementIntro:
                yield return StartCoroutine(RunDialogueMovementIntroStep(dialogueBox));
                break;

            case TutorialStep.CameraMovement:
                yield return StartCoroutine(RunCameraMovementStep());
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
        dialogue.Reinitialize(movementIntroDialogue); // set the dialogue asset for the movement intro dialogue
        dialogue.gameObject.SetActive(true);

        // Wait until dialogue is done (also check for certain index to show mission panel)
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 5)
            {
                // Make mission panel appear because dialogue talks about it
                tutorialMovementPanel.SetActive(true);
            }
            yield return null;
        }
        currentStepComplete = true;
    }

    private IEnumerator RunCameraMovementStep()
    {
        // Show camera movement instructions/panel (redundant now)
        tutorialMovementPanel.SetActive(true);

        // Wait for player to move camera
        yield return new WaitUntil(() => currentStepComplete);
    }

    private void CheckStepCompletion()
    {
        switch (currentStep)
        {
            case TutorialStep.CameraMovement:
                if (cameraController.hasDoneZoom)
                {
                    zoomStatusImage.GetComponent<Image>().sprite = completeBox;
                }
                if (cameraController.hasDonePan)
                {
                    panStatusImage.GetComponent<Image>().sprite = completeBox;
                }
                if (cameraController.hasDoneZoom && cameraController.hasDonePan)
                {
                    Delay(2f); // small delay before marking complete
                    currentStepComplete = true;
                }
                break;
        }
    }

    private void ProgressToNextStep()
    {
        currentStep = currentStep switch
        {
            TutorialStep.Blackscreen => TutorialStep.DialogueMovementIntro,
            TutorialStep.DialogueMovementIntro => TutorialStep.CameraMovement,
            TutorialStep.CameraMovement => TutorialStep.Complete,
            _ => TutorialStep.Complete
        };

        Debug.Log($"Tutorial progressed to: {currentStep}");
    }

    private void OnTutorialComplete()
    {
        Debug.Log("Tutorial Complete!");
        // Handle tutorial completion - disable tutorial systems, enable normal gameplay, etc.
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}
