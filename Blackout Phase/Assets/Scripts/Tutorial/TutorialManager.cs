// Manages the entire tutorial sequence
// Apologize in advance for how long it is, I didn't realize how much this was going to be
// until I was like halfway done
// Ellison
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// enum to track current step of the tutorial
public enum TutorialStep
{
    Blackscreen,
    CameraMovementIntro,
    CameraMovement,
    PlayerMovementIntro,
    PlayerMovement,
    Combat1Intro,
    Combat1,
    LevelUpIntro,
    LevelUp,
    MoveToNextIntro,
    MoveToNext,
    Combat2Intro,
    Combat2,
    Combat3Intro,
    Combat3,
    Reaction,
    ReactionCleanup,
    InventoryIntro,
    Inventory,
    UseItemIntro,
    UseItem,
    Conclusion,
    Complete
}

public class TutorialManager : MonoBehaviour
{
    [Header("Misc. Components")]
    public GameObject dialoguePanel; // default position 670 top, 110 bottom
    // moved position is 600 top, 180 bottom
    public Dialogue dialogueBox; // box ALL dialogue gets loaded into
    public TutorialMouseController mouseController; // for controlling the mouse and player movement
    public GameObject blackscreen; 
    public DragCamera2D cameraController;
    public Sprite incompleteBox;
    public Sprite completeBox;

    public GameObject stepCompletePanel;

    public AudioSource bigStepCompleteSource; // on the canvas

    [Header("Step-Specific Components")]

    [Header("Blackscreen Step")]
    public DialogueAsset blackscreenDialogue;

    [Header("CameraMovementIntro Step")]
    public DialogueAsset cameraMovementIntroDialogue;

    [Header("CameraMovement Step")]
    public GameObject cameraMovementPanel;
    public GameObject panStatusImage;
    public GameObject zoomStatusImage;

    [Header("PlayerMovementIntro Step")]
    public DialogueAsset playerMovementIntroDialogue;
    public GameObject actionMenu;
    public TutorialActionMenu actionMenuScript;

    [Header("PlayerMovement Step")]
    public GameObject cursor;
    public GameObject playerMovementPanel;
    public GameObject move1StatusImage;
    public GameObject move2StatusImage;

    [Header("Combat1Intro Step")]
    public DialogueAsset combat1IntroDialogue;
    public TutorialEnemySpawner enemySpawner1;

    [Header("Combat1 Step")]
    public GameObject combat1Panel;
    public GameObject moveStatusImage;
    public GameObject attackStatusImage;
    public GameObject confirmStatusImage;
    public DamageObserver damageObserver;

    [Header("LevelUpIntro Step")]
    public DialogueAsset levelUpIntroDialogue;
    public LevelsManager levelsManager;

    [Header("LevelUp Step")]
    public GameObject levelUpHud;
    public DialogueAsset levelUpDialogue;
    public GameObject levelUpPanel;
    public GameObject selectStatusImage;
    public bool hudShown = false;
    public GameObject blocker;

    [Header("MoveToNextIntro Step")]
    public DialogueAsset moveToNextIntroDialogue;

    [Header("MoveToNext Step")]
    public GameObject moveToNextPanel;
    public GameObject move3StatusImage;
    public GameObject move4StatusImage;

    [Header("Combat2Intro Step")]
    public DialogueAsset combat2IntroDialogue;
    public TutorialEnemySpawner enemySpawner2;
    public TutorialEnemySpawner enemySpawner3;

    [Header("Combat2 Step")]
    public GameObject combat2Panel;
    public GameObject defeat2StatusImage;
    public GameObject defeat3StatusImage;

    [Header("Combat3Intro Step")]
    public DialogueAsset combat3IntroDialogue;
    public TutorialEnemySpawner enemySpawner4;

    [Header("Combat3 Step")]
    public GameObject combat3Panel;
    public GameObject skipStatusImage;

    [Header("Reaction Step")]
    public ReactionUI reactionUI;
    public GameObject reactionScreenObject;
    public DialogueAsset reactionDialogue;
    public GameObject reactionPanel;
    public GameObject reactStatusImage;
    public bool reactionShown = false;
    public GameObject blocker2;
    public GameObject blocker3;
    public GameObject blocker4;

    [Header("ReactionCleanup Step")]
    public DialogueAsset reactionCleanupDialogue;

    [Header("InventoryIntro Step")]
    public DialogueAsset inventoryIntroDialogue;

    [Header("Inventory Step")]
    public GameObject inventoryButton;
    public GameObject openInventoryPanel;
    public GameObject openInventoryStatusImage;
    public Inventory inventory;

    [Header("UseItemIntro Step")]
    public DialogueAsset useItemIntroDialogue;

    [Header("UseItem Step")]
    public GameObject useItemPanel;
    public GameObject useBothStatusImage;

    [Header("Conclusion Step")]
    public DialogueAsset conclusionDialogue;

    // Dictionary to map tutorial steps to their panel's AudioSource
    private Dictionary<TutorialStep, AudioSource> stepAudioSources = new Dictionary<TutorialStep, AudioSource>();

    public TutorialStep currentStep = TutorialStep.Blackscreen;
    public bool currentStepComplete = false;

    // Track which tasks have been marked complete to avoid repeated calls
    private bool zoomMarkedComplete = false;
    private bool panMarkedComplete = false;

    public bool stepMarkedComplete = false;

    private bool move1aDone = false;
    private bool move1bDone = false;
    private bool move2aDone = false;
    private bool move2bDone = false;
    private bool move3aDone = false;
    private bool move3bDone = false;

    private bool combat1MoveDone = false;
    public bool combat1AttackDone = false;
    private bool combat1ConfirmDone = false;

    public bool levelUpSelectDone = false;

    private bool transition1aDone = false;
    private bool transition1bDone = false;
    private bool transition2aDone = false;
    private bool transition2bDone = false;
    private bool transition3aDone = false;
    private bool transition3bDone = false;

    private bool enemy2Defeated = false;
    private bool enemy3Defeated = false;

    private bool combat3Skipped = false;

    public bool reactionSelectDone = false;

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

        AudioSource playerMovementAudio = playerMovementPanel.GetComponent<AudioSource>();
        if (playerMovementAudio != null)
        {
            stepAudioSources[TutorialStep.PlayerMovement] = playerMovementAudio;
        }
        else
        {
            Debug.LogWarning("No AudioSource found on playerMovementPanel");
        }

        AudioSource combat1Audio = combat1Panel.GetComponent<AudioSource>();
        if (combat1Audio != null)
        {
            stepAudioSources[TutorialStep.Combat1] = combat1Audio;
        }
        else
        {
            Debug.LogWarning("No AudioSource found on combat1Panel");
        }

        AudioSource moveToNextAudio = moveToNextPanel.GetComponent<AudioSource>();
        if (moveToNextAudio != null)
        {
            stepAudioSources[TutorialStep.MoveToNext] = moveToNextAudio;
        }
        else
        {
            Debug.LogWarning("No AudioSource found on moveToNextPanel");
        }

        AudioSource combat2Audio = combat2Panel.GetComponent<AudioSource>();
        if (combat2Audio != null)
        {
            stepAudioSources[TutorialStep.MoveToNext] = combat2Audio;
        }
        else
        {
            Debug.LogWarning("No AudioSource found on combat2Panel");
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

            case TutorialStep.PlayerMovement:
                yield return StartCoroutine(RunPlayerMovementStep());
                break;

            case TutorialStep.Combat1Intro:
                yield return StartCoroutine(RunCombat1IntroStep(dialogueBox));
                break;

            case TutorialStep.Combat1:
                yield return StartCoroutine(RunCombat1Step());
                break;

            case TutorialStep.LevelUpIntro:
                yield return StartCoroutine(RunLevelUpIntroStep(dialogueBox));
                break;

            case TutorialStep.LevelUp:
                yield return StartCoroutine(RunLevelUpStep(dialogueBox));
                break;

            case TutorialStep.MoveToNextIntro:
                yield return StartCoroutine(RunMoveToNextIntroStep(dialogueBox));
                break;

            case TutorialStep.MoveToNext:
                yield return StartCoroutine(RunMoveToNextStep());
                break;

            case TutorialStep.Combat2Intro:
                yield return StartCoroutine(RunCombat2IntroStep(dialogueBox));
                break;

            case TutorialStep.Combat2:
                yield return StartCoroutine(RunCombat2Step());
                break;

            case TutorialStep.Combat3Intro:
                yield return StartCoroutine(RunCombat3IntroStep(dialogueBox));
                break;

            case TutorialStep.Combat3:
                yield return StartCoroutine(RunCombat3Step());
                break;

            case TutorialStep.Reaction:
                yield return StartCoroutine(RunReactionStep(dialogueBox));
                break;

            case TutorialStep.ReactionCleanup:
                yield return StartCoroutine(RunReactionCleanupStep(dialogueBox));
                break;

            case TutorialStep.InventoryIntro:
                yield return StartCoroutine(RunInventoryIntroStep(dialogueBox));
                break;

            case TutorialStep.Inventory:
                yield return StartCoroutine(RunInventoryStep());
                break;

            case TutorialStep.UseItemIntro:
                yield return StartCoroutine(RunUseItemIntroStep(dialogueBox));
                break;

            case TutorialStep.UseItem:
                yield return StartCoroutine(RunUseItemStep());
                break;

            case TutorialStep.Conclusion:
                yield return StartCoroutine(RunConclusionStep(dialogueBox));
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

        // alse set the 3 enemies' sprites off since there's nowhere else to do it
        enemySpawner2.spriteRenderer.enabled = false;
        enemySpawner3.spriteRenderer.enabled = false;
        enemySpawner4.spriteRenderer.enabled = false;
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

    private IEnumerator RunPlayerMovementStep()
    {
        if (!actionMenu.activeSelf)
        {
            actionMenu.SetActive(true);
        }
        stepCompletePanel.SetActive(false);
        cursor.SetActive(true);
        playerMovementPanel.SetActive(true);
        mouseController.BeginBlinkingSequence();

        yield return new WaitUntil(() => currentStepComplete);
    }

    private IEnumerator RunCombat1IntroStep(Dialogue dialogue)
    {
        actionMenuScript.CloseMenu(); // close action menu
        dialogue.Reinitialize(combat1IntroDialogue); // set the dialogue asset for the combat intro dialogue
        dialogue.gameObject.SetActive(true);
        actionMenu.SetActive(false);

        // Move dialogue box back down to default position
        RectTransform rectTransform = dialoguePanel.GetComponent<RectTransform>();
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 110); // Set bottom offset
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -670); // Set top offset (negative)

        // Wait until dialogue is done
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
            }
            if (dialogue.GetCurrentLineIndex() == 3)
            {
                // Move enemy to its spot 1
                enemySpawner1.TriggerOneTileMove(enemySpawner1.spot1TilePosition);
            }
            yield return null;
        }
        actionMenu.SetActive(true);
        currentStepComplete = true;
    }

    private IEnumerator RunCombat1Step()
    {
        if (!actionMenu.activeSelf)
        {
            actionMenu.SetActive(true);
        }
        stepCompletePanel.SetActive(false);
        combat1Panel.SetActive(true);
        mouseController.StartMilestoneBlinking(mouseController.GetCurrentMilestoneTarget());

        yield return new WaitUntil(() => currentStepComplete);
    }

    private IEnumerator RunLevelUpIntroStep(Dialogue dialogue)
    {
        actionMenuScript.CloseMenu(); // close action menu
        dialogue.Reinitialize(levelUpIntroDialogue); // set the dialogue asset for the level up intro dialogue
        dialogue.gameObject.SetActive(true);
        actionMenu.SetActive(false);

        // Wait until dialogue is done
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
            }
            yield return null;
        }
        levelsManager.IncreaseXP(100); // give enough XP to level up
        currentStepComplete = true;
    }

    private IEnumerator RunLevelUpStep(Dialogue dialogue)
    {
        // wait until level up hud appears
        yield return new WaitUntil(() => levelUpHud.activeInHierarchy);
        blocker.SetActive(true);
        hudShown = true;

        dialogue.Reinitialize(levelUpDialogue); // set the dialogue asset for the level up midpoint dialogue
        dialogue.gameObject.SetActive(true);
        // Wait until dialogue is done
        while (!dialogue.dialogueDone)
        {
            yield return null;
        }
        blocker.SetActive(false);
        stepCompletePanel.SetActive(false);
        levelUpPanel.SetActive(true);

        yield return new WaitUntil(() => currentStepComplete);
    }

    private IEnumerator RunMoveToNextIntroStep(Dialogue dialogue)
    {
        dialogue.Reinitialize(moveToNextIntroDialogue);
        dialogue.gameObject.SetActive(true);

        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
            }
            yield return null;
        }
        currentStepComplete = true;
    }

    private IEnumerator RunMoveToNextStep()
    {
        if (!actionMenu.activeSelf)
        {
            actionMenu.SetActive(true);
        }
        stepCompletePanel.SetActive(false);
        moveToNextPanel.SetActive(true);
        mouseController.BeginSecondBlinkingSequence();

        yield return new WaitUntil(() => currentStepComplete);
    }

    private IEnumerator RunCombat2IntroStep(Dialogue dialogue)
    {
        actionMenuScript.CloseMenu(); // close action menu
        dialogue.Reinitialize(combat2IntroDialogue); // set the dialogue asset for the combat intro dialogue
        dialogue.gameObject.SetActive(true);
        actionMenu.SetActive(false);

        bool enemiesMoving = false;
        // Wait until dialogue is done
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 0)
            {
                enemySpawner2.spriteRenderer.enabled = true;
                enemySpawner3.spriteRenderer.enabled = true;
            }
            if (dialogue.GetCurrentLineIndex() == 1 && !enemiesMoving)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
                enemySpawner2.TriggerPathMove();
                enemySpawner3.TriggerPathMove();
                enemiesMoving = true;
            }
            yield return null;
        }
        actionMenu.SetActive(true);
        currentStepComplete = true;
    }

    private IEnumerator RunCombat2Step()
    {
        if (!actionMenu.activeSelf)
        {
            actionMenu.SetActive(true);
        }
        stepCompletePanel.SetActive(false);
        combat2Panel.SetActive(true);

        yield return new WaitUntil(() => currentStepComplete);
    }

    private IEnumerator RunCombat3IntroStep(Dialogue dialogue)
    {
        actionMenuScript.CloseMenu(); // close action menu
        dialogue.Reinitialize(combat3IntroDialogue); // set the dialogue asset for the combat intro dialogue
        dialogue.gameObject.SetActive(true);
        actionMenu.SetActive(false);

        bool enemiesMoving = false;
        // Wait until dialogue is done
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
            }
            if (dialogue.GetCurrentLineIndex() == 2)
            {
                enemySpawner4.spriteRenderer.enabled = true;
            }
            if (dialogue.GetCurrentLineIndex() == 3 && !enemiesMoving)
            {
                enemySpawner4.TriggerPathMove();
                enemiesMoving = true;
            }
            yield return null;
        }
        actionMenu.SetActive(true);
        currentStepComplete = true;
    }

    private IEnumerator RunCombat3Step()
    {
        actionMenu.SetActive(false);
        stepCompletePanel.SetActive(false);
        combat3Panel.SetActive(true);

        yield return new WaitUntil(() => currentStepComplete);
    }

    private IEnumerator RunReactionStep(Dialogue dialogue)
    {
        reactionUI.ShowReactionUI();
        reactionUI.isTutorial = true;
        // wait for appear
        yield return new WaitUntil(() => reactionScreenObject.activeInHierarchy);
        blocker2.SetActive(true);
        blocker3.SetActive(true);
        blocker4.SetActive(true);
        reactionShown = true;
        stepCompletePanel.SetActive(false);

        dialogue.Reinitialize(reactionDialogue); // set the dialogue asset for the level up midpoint dialogue
        dialogue.gameObject.SetActive(true);
        // Wait until dialogue is done
        while (!dialogue.dialogueDone)
        {
            yield return null;
        }
        blocker4.SetActive(false);

        reactionPanel.SetActive(true);

        yield return new WaitUntil(() => currentStepComplete);
        reactionUI.isTutorial = false;
        blocker2.SetActive(false);
        blocker3.SetActive(false);
    }

    private IEnumerator RunReactionCleanupStep(Dialogue dialogue)
    {
        dialogue.Reinitialize(reactionCleanupDialogue); // set the dialogue asset for the combat intro dialogue
        dialogue.gameObject.SetActive(true);
        bool countered = false;
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1 && !countered)
            {
                stepCompletePanel.SetActive(false);
                AttackEnemy(4, 10);
                levelsManager.IncreaseXP(25);
                countered = true;
            }
            yield return null;
        }
    }

    private IEnumerator RunInventoryIntroStep(Dialogue dialogue)
    {
        dialogue.Reinitialize(inventoryIntroDialogue); // set the dialogue asset for the combat intro dialogue
        dialogue.gameObject.SetActive(true);
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
            }
            yield return null;
        }
    }

    private IEnumerator RunInventoryStep()
    {
        inventoryButton.gameObject.SetActive(true);
        if (!actionMenu.activeSelf)
        {
            actionMenu.SetActive(true);
        }
        stepCompletePanel.SetActive(false);
        openInventoryPanel.SetActive(true);

        yield return new WaitUntil(() => currentStepComplete);
        // prompt UI refresh to get the pre added items to appear
        inventory.onItemChangedCallback.Invoke();

    }

    private IEnumerator RunUseItemIntroStep(Dialogue dialogue)
    {
        dialogue.Reinitialize(useItemIntroDialogue); // set the dialogue asset for the combat intro dialogue
        dialogue.gameObject.SetActive(true);
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
            }
            yield return null;
        }
    }

    private IEnumerator RunUseItemStep()
    {
        stepCompletePanel.SetActive(false);
        useItemPanel.SetActive(true);

        yield return new WaitUntil(() => currentStepComplete);
    }

    private IEnumerator RunConclusionStep(Dialogue dialogue)
    {
        dialogue.Reinitialize(conclusionDialogue); // set the dialogue asset for the combat intro dialogue
        dialogue.gameObject.SetActive(true);
        while (!dialogue.dialogueDone)
        {
            if (dialogue.GetCurrentLineIndex() == 1)
            {
                // clear step complete panel from last step
                stepCompletePanel.SetActive(false);
            }
            yield return null;
        }
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
                if (zoomMarkedComplete && panMarkedComplete && !stepMarkedComplete)
                {
                    StartCoroutine(CompleteStepWithDelay(currentStep, 2f)); // Pass current step
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.PlayerMovement:
                if (mouseController.movedSpot1a && !move1aDone)
                {
                    markTaskComplete(move1StatusImage);
                    move1aDone = true;
                }
                if (mouseController.movedSpot1b && !move1bDone)
                {
                    markTaskComplete(move2StatusImage);
                    move1bDone = true;
                }
                // reset for move 2
                if (move1aDone && move1bDone && !move2aDone && !move2bDone && !stepMarkedComplete)
                {
                    move1StatusImage.GetComponent<Image>().sprite = incompleteBox;
                    move2StatusImage.GetComponent<Image>().sprite = incompleteBox;
                }
                if (mouseController.movedSpot2a && !move2aDone)
                {
                    markTaskComplete(move1StatusImage);
                    move2aDone = true;
                }
                if (mouseController.movedSpot2b && !move2bDone)
                {
                    markTaskComplete(move2StatusImage);
                    move2bDone = true;
                }
                // reset for move 3
                if (move2aDone && move2bDone && !move3aDone && !move3bDone && !stepMarkedComplete)
                {
                    move1StatusImage.GetComponent<Image>().sprite = incompleteBox;
                    move2StatusImage.GetComponent<Image>().sprite = incompleteBox;
                }
                if (mouseController.movedSpot3a && !move3aDone)
                {
                    markTaskComplete(move1StatusImage);
                    move3aDone = true;
                }
                if (mouseController.movedSpot3b && !move3bDone)
                {
                    markTaskComplete(move2StatusImage);
                    move3bDone = true;
                }
                // check final steps
                if (move3aDone && move3bDone && !stepMarkedComplete)
                {
                    StartCoroutine(CompleteStepWithDelay(currentStep, 2f)); // Pass current step
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.Combat1:
                if (mouseController.movedCombat1Spot && !combat1MoveDone)
                {
                    markTaskComplete(moveStatusImage);
                    combat1MoveDone = true;
                }
                if (combat1MoveDone && mouseController.attackCombat1Prepare && !combat1AttackDone)
                {
                    markTaskComplete(attackStatusImage);
                    combat1AttackDone = true;
                }
                // edge case for canceling attack
                if (!mouseController.attackCombat1Prepare && combat1AttackDone)
                {
                    attackStatusImage.GetComponent<Image>().sprite = incompleteBox;
                    combat1AttackDone = false;
                }
                if (combat1MoveDone && combat1AttackDone && mouseController.confirmedCombat1Attack && !combat1ConfirmDone)
                {
                    markTaskComplete(confirmStatusImage);
                    enemySpawner1.loseHealth(10);
                    //damageObserver.ShowDamage(mouseController.characterInfo.BaseAttk);
                    combat1ConfirmDone = true;
                }
                if (combat1ConfirmDone && !stepMarkedComplete)
                {
                    StartCoroutine(CompleteStepWithDelay(currentStep, 2f)); // Pass current step
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.LevelUp:
                // if hud has appeared and disappeared by picking upgrade
                if (hudShown && !levelUpHud.activeInHierarchy && !levelUpSelectDone)
                {
                    markTaskComplete(selectStatusImage);
                    levelUpSelectDone = true;
                }
                if (levelUpSelectDone && !stepMarkedComplete)
                {
                    StartCoroutine(CompleteStepWithDelay(currentStep, 1f)); // Pass current step
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.MoveToNext:
                if (mouseController.movedTransition1a && !transition1aDone)
                {
                    markTaskComplete(move3StatusImage);
                    transition1aDone = true;
                }
                if (mouseController.movedTransition1b && !transition1bDone)
                {
                    markTaskComplete(move4StatusImage);
                    transition1bDone = true;
                }
                // reset
                if (transition1aDone && transition1bDone && !transition2aDone && !transition2bDone && !stepMarkedComplete)
                {
                    move3StatusImage.GetComponent<Image>().sprite = incompleteBox;
                    move4StatusImage.GetComponent<Image>().sprite = incompleteBox;
                }
                if (mouseController.movedTransition2a && !transition2aDone)
                {
                    markTaskComplete(move3StatusImage);
                    transition2aDone = true;
                }
                if (mouseController.movedTransition2b && !transition2bDone)
                {
                    markTaskComplete(move4StatusImage);
                    transition2bDone = true;
                }
                // reset
                if (transition2aDone && transition2bDone && !transition3aDone && !transition3bDone && !stepMarkedComplete)
                {
                    move3StatusImage.GetComponent<Image>().sprite = incompleteBox;
                    move4StatusImage.GetComponent<Image>().sprite = incompleteBox;
                }
                if (mouseController.movedTransition3a && !transition3aDone)
                {
                    markTaskComplete(move3StatusImage);
                    transition3aDone = true;
                }
                if (mouseController.movedTransition3b && !transition3bDone)
                {
                    markTaskComplete(move4StatusImage);
                    transition3bDone = true;
                }
                // check final steps
                if (transition3aDone && transition3bDone && !stepMarkedComplete)
                {
                    StartCoroutine(CompleteStepWithDelay(currentStep, 2f)); // Pass current step
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.Combat2:
                if (!enemySpawner2.isAlive && !enemy2Defeated)
                {
                    markTaskComplete(defeat2StatusImage);
                    levelsManager.IncreaseXP(50);
                    enemy2Defeated = true;
                }
                if (!enemySpawner3.isAlive && !enemy3Defeated)
                {
                    markTaskComplete(defeat3StatusImage);
                    levelsManager.IncreaseXP(50);
                    enemy3Defeated = true;
                }
                if (enemy2Defeated && enemy3Defeated && !stepMarkedComplete)
                {
                    StartCoroutine(CompleteStepWithDelay(currentStep, 2f));
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.Combat3:
                if (mouseController.skipCombat3 && !combat3Skipped)
                {
                    markTaskComplete(skipStatusImage);
                    combat3Skipped = true;
                }
                if (combat3Skipped && !stepMarkedComplete)
                {
                    StartCoroutine(CompleteStepWithDelay(currentStep, 2f));
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.Reaction:
                // get from ReactionUI
                if (reactionSelectDone && !stepMarkedComplete)
                {
                    markTaskComplete(reactStatusImage);
                    StartCoroutine(CompleteStepWithDelay(currentStep, 0.5f)); // Pass current step
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.Inventory:
                if (actionMenuScript.inventoryOpen && !stepMarkedComplete)
                {
                    markTaskComplete(openInventoryStatusImage);
                    StartCoroutine(CompleteStepWithDelay(currentStep, 1f));
                    stepMarkedComplete = true;
                }
                break;

            case TutorialStep.UseItem:
                if (inventory.items.Count == 0 && !stepMarkedComplete)
                {
                    markTaskComplete(useBothStatusImage);
                    StartCoroutine(CompleteStepWithDelay(currentStep, 1f));
                    stepMarkedComplete = true;
                    actionMenuScript.CloseInventoryScreen();
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
            case TutorialStep.PlayerMovement:
                playerMovementPanel.SetActive(false);
                break;
            case TutorialStep.Combat1:
                combat1Panel.SetActive(false);
                break;
            case TutorialStep.LevelUp:
                levelUpPanel.SetActive(false);
                break;
            case TutorialStep.MoveToNext:
                moveToNextPanel.SetActive(false);
                break;
            case TutorialStep.Combat2:
                combat2Panel.SetActive(false);
                break;
            case TutorialStep.Combat3:
                combat3Panel.SetActive(false);
                break;
            case TutorialStep.Reaction:
                reactionPanel.SetActive(false);
                break;
            case TutorialStep.Inventory:
                openInventoryPanel.SetActive(false);
                break;
            case TutorialStep.UseItem:
                useItemPanel.SetActive(false);
                break;
        }
    }

    private void ProgressToNextStep()
    {
        // Reset completion flag for next step
        stepMarkedComplete = false;

        currentStep = currentStep switch
        {
            TutorialStep.Blackscreen => TutorialStep.CameraMovementIntro,
            TutorialStep.CameraMovementIntro => TutorialStep.CameraMovement,
            TutorialStep.CameraMovement => TutorialStep.PlayerMovementIntro,
            TutorialStep.PlayerMovementIntro => TutorialStep.PlayerMovement,
            TutorialStep.PlayerMovement => TutorialStep.Combat1Intro,
            TutorialStep.Combat1Intro => TutorialStep.Combat1,
            TutorialStep.Combat1 => TutorialStep.LevelUpIntro,
            TutorialStep.LevelUpIntro => TutorialStep.LevelUp,
            TutorialStep.LevelUp => TutorialStep.MoveToNextIntro,
            TutorialStep.MoveToNextIntro => TutorialStep.MoveToNext,
            TutorialStep.MoveToNext => TutorialStep.Combat2Intro,
            TutorialStep.Combat2Intro => TutorialStep.Combat2,
            TutorialStep.Combat2 => TutorialStep.Combat3Intro,
            TutorialStep.Combat3Intro => TutorialStep.Combat3,
            TutorialStep.Combat3 => TutorialStep.Reaction,
            TutorialStep.Reaction => TutorialStep.ReactionCleanup,
            TutorialStep.ReactionCleanup => TutorialStep.InventoryIntro,
            TutorialStep.InventoryIntro => TutorialStep.Inventory,
            TutorialStep.Inventory => TutorialStep.UseItemIntro,
            TutorialStep.UseItemIntro => TutorialStep.UseItem,
            TutorialStep.UseItem => TutorialStep.Conclusion,
            TutorialStep.Conclusion => TutorialStep.Complete,
            _ => TutorialStep.Complete
        };

        Debug.Log($"Tutorial progressed to: {currentStep}");
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    private void OnTutorialComplete()
    {
        Debug.Log("Tutorial Complete!");
        StartCoroutine(Delay(2f));
        SceneManager.LoadScene("Demo_pxiel_2D_Test_Grid");
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

    public void AttackEnemy(int enemyID, int damage)
    {
        TutorialEnemySpawner targetSpawner = null;

        if (enemyID == 1) targetSpawner = enemySpawner1;
        else if (enemyID == 2) targetSpawner = enemySpawner2;
        else if (enemyID == 3) targetSpawner = enemySpawner3;
        else if (enemyID == 4) targetSpawner = enemySpawner4;

        if (targetSpawner != null)
        {
            targetSpawner.loseHealth(damage);

            if (damageObserver != null)
            {
                Debug.Log("Damage being shown allegedly");
                damageObserver.ShowPlayerDamage(damage, targetSpawner.enemyInfo.transform.position);
            }
        }
    }

    public void EnemyAttackSequence()
    {
        if (enemySpawner2.isAlive)
        {
            mouseController.characterInfo.PlayerTakeDamage(5);
        }
        if (enemySpawner3.isAlive)
        {
            mouseController.characterInfo.PlayerTakeDamage(5);
        }
    }
}
