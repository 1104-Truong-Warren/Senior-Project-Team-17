// These are based on this channel on YouTube: https://www.youtube.com/@lawlessgames3844
// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// The character info stats is made by me, I added stats for the character hp, movementRange
// functionalities, AP, and how it works 
// Weijun
using UnityEngine; // default 

public class CharacterInfo1 : MonoBehaviour
{
    private int maxAP = 2; // action points for player each turn max out at 2 aside from passive skills
    //private int RageMode = 2; // after defeating 2 enemy in a row, enters a special state, player can attack again (extra movement) on top of the AP

    [Header("Player Stats")]
    [SerializeField] private int HP;    //  the player's current
    [SerializeField] private int MaxHP; // the player's Max HP
    [SerializeField] private int EN; // the player's current EN (energey for skills)
    [SerializeField] private int MaxEN; // the player's max EN
    [SerializeField] private int baseMoveRange; // how far player able to move
    [SerializeField] private int baseAttk; // the basic attack of player
    [SerializeField] private int baseAttkRange; // basic attack range of player
    [SerializeField] private int baseHitRate; // the basic hit rate of player
    [SerializeField] private int baseCriticalRate; // the basic critical rate for player
    [SerializeField] private int baseCritDamage; // the basic critical damage for player
    [SerializeField] private int baseEvasion; // evasion rate of the player

    private OverlayTile1 standingOnTile; // stores the tile

    // public accessor for player's info
    public int CurrentHP => HP; 
    public int maxHP => MaxHP;
    public int CurrentEN => EN;
    public int maxEN => MaxEN;
    public int BaseAttk => baseAttk;
    public int BaseRange => baseAttkRange;
    public int BaseHitRate => baseHitRate;
    public int BaseCriticalRate => baseCriticalRate;
    public int BaseCritDamage => baseCritDamage;    
    public int BaseEvasion => baseEvasion; 

    // check EN
    public bool HasEN(int costEN) => EN >= costEN; // left EN right cost (>=) a right symbol

    //public int MoveRange => moveRange;

    public OverlayTile1 CurrentTile => standingOnTile;

    public static CharacterInfo1 Instance { get; private set; } // access
    public int currentAP {  get; private set; } // access the AP

    private Animator animator; // Added by Warren

    //public OverlayTile PlayerSetTile() => CurrentTile;// helper

    private void Awake()
    {
        Instance = this; // set up the player accessor

        currentAP = maxAP; // Start with 2AP

        animator = GetComponent<Animator>(); // Added by Warren
    }

    public void ResetAP()
    {
        currentAP = maxAP; // every turn starts we reset the AP to max
    }

    public void ApUsed(int amountAP)
    {
        currentAP = Mathf.Max(0, currentAP - amountAP); // comparing the 0, currentAP - 1 perturn, return the lower value, no negative AP

        //Debug.Log($"Player spend {amountAP}AP => AP Left: {currentAP}"); // debug shows current AP
    }

    public int GetMoveRange()
    {
        // AP is at 2 use the base movement range
        if (currentAP == 2)
            return baseMoveRange;

        else if (currentAP == 1)
            return Mathf.FloorToInt(baseMoveRange * 0.5f); // reduced to 50% of the base movement range for the second AP point

        else
            return 0; // nonthing match 
    }

    public void PlayerSetTile(OverlayTile1 tile)
    {
        // if current tile is not null reset the tile 
        if (CurrentTile != null)
            CurrentTile.hasPlayer = false; 

        standingOnTile = tile; // instead of directly accessing use this function

        CurrentTile.hasPlayer = true; // after the new tile is set hasPlayer = T
    }

    public void PlayerTakeDamage(int dmg)
    {
        HP -= dmg; // current hp - dmg

        if (HP <= 0) // check if player have HP left
        {
            HP = 0; // reset it to 0

            Debug.Log($"{name} has died."); // debug

            TurnManager.Instance.SetTurnState(TurnState.GameOver); // Game Over!
        }
    }

    public bool PlayerEnCheck(int costEN)
    {
        // if the costEN is less than or equal to 0 free turn, for the rage mode 
        if (costEN <= 0) return true;

        // if the player EN is less than EN cost return false
        if (EN < costEN) return false;

        return true;
    } 

    public void PlayerSpendEN(int costEN)
    {
        // if the EN cost is negative or greater than total EN get out
        if (costEN < 0 || costEN > CurrentEN || CurrentEN == 0) return;

        EN -= costEN; // EN - costEN
    }

    public void RestoreEN(int amountEN)
    {
        // if the amount is negative return
        if (amountEN <= 0) return;

        EN = Mathf.Min(maxEN, EN + amountEN); // compare maxEN and the current EN + restore amount and use the minimum, make sure we don't over cap the EN limit
    }

    // ADDED BY WARREN: New method to increase maximum HP (for level up choices)
    // also used to increase max HP for equipped gear - Ellison
    public void IncreaseMaxHP(int amount)
    {
        MaxHP += amount;
        HP += amount;
        Debug.Log($"HP increased to {HP}/{MaxHP}");
        
        UpdateAllUI();
    }

    // added function to decrease max HP, mostly used for recalculating gear bonuses
    // best way I could think of doing this was using this function to subtract the old bonus then add the new
    // could also be used for debuffs later if needed
    // - Ellison
    public void DecreaseMaxHP(int amount)
    {
        MaxHP -= amount;
        //HP = Mathf.Min(HP, MaxHP); // make sure current HP doesn't exceed new max HP
        HP -= amount; // decrease current HP by the same amount to reflect the change, if we just set it to the new max it would heal the player if they were below the new max already
        Debug.Log($"HP decreased to {HP}/{MaxHP}");
        
        UpdateAllUI();
    }

    // function to restore HP
    // - Ellison
    public void RestoreHP(int amount)
    {
        if (amount <= 0) return; // if the amount is negative or zero return
        HP = Mathf.Min(MaxHP, HP + amount); // compare maxHP and the current HP + restore amount and use the minimum, make sure we don't over cap the HP limit
    }

    // New method to increase maximum EN (for level up choices)
    public void IncreaseMaxEN(int amount)
    {
        MaxEN += amount;
        EN += amount;
        Debug.Log($"EN increased to {EN}/{MaxEN}");
        
        UpdateAllUI();
    }
    
    // New method to increase base attack (for level up choices)
    public void IncreaseAttack(int amount)
    {
        baseAttk += amount;
        Debug.Log($"Attack increased to {baseAttk}");
    
        UpdateAllUI();
    }

    // function to decrease base attack, same logic for DecreaseMaxHP, check above
    // - Ellison
    public void DecreaseAttack(int amount)
    {
        baseAttk -= amount;
        Debug.Log($"Attack decreased to {baseAttk}");
    
        UpdateAllUI();
    }

    // New method to update all of the UI and have LevelsManager.cs update it visually on screen during runtime
    private void UpdateAllUI()
    {
        // Find all UI text elements and update them directly
        CharacterInfoDisplay[] displays = FindObjectsOfType<CharacterInfoDisplay>();
        
        if (displays.Length == 0)
        {
            Debug.LogWarning("No CharacterInfoDisplay found in scene!");
            return;
        }
        
        Debug.Log($"Found {displays.Length} UI display(s), updating...");
    }

    // Added by Warren
    public void PlayAttackAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }
        
}
