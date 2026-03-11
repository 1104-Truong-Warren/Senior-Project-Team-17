// Weijun

using UnityEngine;

public class PlayerFuryMode : MonoBehaviour
{
    [Header("Fury Mode Settings")]
    [SerializeField] private int killsToTrigger = 2; // how many kills it needs to trigger Fury Mode
    [SerializeField] private int baseFuryModeActionTurns = 2; // how long the base Fury mode lasts

    // Added by Warren
    private Animator animator;

    // Player in game stats
    private int currentKills = 0; // starting with 0 kills

    private int FuryModeRemains = 0; // how many more turns Fury mode remains

    public bool inFuryMode { get; private set; } // flag to check if Fury mode is active, when FuryMode > 0 is true

    public static PlayerFuryMode Instance { get; private set; } // allow data to be access by other scripts

    private PlayerFuryVisualEffect effect; // accessor 

    private void Awake()
    {
        Instance = this; // set up the Instance

        inFuryMode = false; // defalut is false; < 0

        effect = GetComponent<PlayerFuryVisualEffect>(); // set up the reference
    }

    public void EnemyKilledUpdate()
    {
        currentKills++; // add one to killing steak

        // current kills bigger or equal to trigger condition and player is not in Fury Mode call active function
        if (currentKills >= killsToTrigger && !inFuryMode)
        {
            FuryModeActive(); // calling the function set up
        }
    }

    private void FuryModeActive()
    {
        inFuryMode = true; // set fury mode active 

        FuryModeRemains = baseFuryModeActionTurns; // set the remaining Tunr equal to baseFuryModActionTurns

        effect.FuryFlashEffect(); // start playing the flashing effects

        Debug.Log("Fury Mode Active!"); // debug msg

        // Added by Warren
        ShowFuryModePopup();
    }

    public bool FuryModeGoingDown()
    {
        if (!inFuryMode) return false; // make sure it doesn't work when not in Fury mode, return false

        FuryModeRemains--; // goes down by 1 

        Debug.Log($"Fury Mode has: {FuryModeRemains} Turns Remaining"); // debug msg

        // if the Fury Mode goes down to 0 turn call exit function
        if (FuryModeRemains <= 0)
            ExitFuryMode(); // calling the exit function

        return true; // if the funcion runs return t
    }

    private void ExitFuryMode()
    {
        inFuryMode = false; // back to false since fury mode is overed

        // reset kills/remaining turns

        currentKills = 0; // reset

        FuryModeRemains = 0; // reset

        effect.StopFuryFlash(); // stop the flashing effect

        Debug.Log("Fury Mode Ended!"); // debug msg
    }

    public void ResetCurrentKills()
    {
        currentKills = 0; // reset the current kills
    }

    // Added by Warren, allows the image to appear for a brief moment Fury Mode is active, and then disappears with a zoom in and out animation.
    private void ShowFuryModePopup()
    {
        GameObject canvas = GameObject.Find("FuryModeCanvas");
        
        if (canvas == null)
        {
            Debug.LogError("Could not find FuryModeCanvas!");
            return;
        }
        
        Transform popupTransform = canvas.transform.Find("FuryModePopup");
        
        if (popupTransform == null)
        {
            Debug.LogError("Could not find FuryModePopup under FuryModeCanvas!");
            return;
        }
        
        GameObject popup = popupTransform.gameObject;
        
        animator = popup.GetComponent<Animator>();
        if (animator == null)
        {
            animator = popup.AddComponent<Animator>();
        }
        
        popup.SetActive(true);
        animator.Play("FuryModePopUp");
        
        StartCoroutine(HideAfterAnimation(popup, 1.5f));
    }

    // Added by Warren, this function waits for the animation to complete, and then hides the popup.
    private System.Collections.IEnumerator HideAfterAnimation(GameObject popup, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (popup != null)
        {
            popup.SetActive(false);
        }
    }
}
