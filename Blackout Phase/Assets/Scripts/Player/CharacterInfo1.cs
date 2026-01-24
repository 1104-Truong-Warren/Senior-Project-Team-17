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
    private int RageMode = 2; // after defeating 2 enemy in a row, enters a special state, player can attack again (extra movement) on top of the AP

    [Header("Player Stats")]
    [SerializeField] private int HP;    //  the player's current
    [SerializeField] private int MaxHP; // the player's Max HP
    [SerializeField] private int EN; // the player's current EN (energey for skills)
    [SerializeField] private int MaxEN; // the player's max EN
    [SerializeField] private int baseMoveRange; // how far player able to move
    [SerializeField] private int baseAttk; // the basic attack of player
    [SerializeField] private int baseAttkRange; // basic attack range of player
    [SerializeField] private int basehitRate; // the basic hit rate of player
    [SerializeField] private int baseCriticalRate; // the basic critical rate for player
    [SerializeField] private int baseCritDamage; // the basic critical damage for player

    private OverlayTile1 standingOnTile; // stores the tile

    // public accessor for player's info
    public int CurrentHP => HP; 
    public int maxHP => MaxHP;
    public int CurrentEN => EN;
    public int maxEN => MaxEN;
    public int BaseAttk => baseAttk;
    public int BaseRange => baseAttkRange;

    // check EN
    public bool HasEN(int costEN) => EN >= costEN; // left EN right cost (>=) a right symbol

    //public int MoveRange => moveRange;

    public OverlayTile1 CurrentTile => standingOnTile;

    public static CharacterInfo1 Instance { get; private set; } // access
    public int currentAP {  get; private set; } // access the AP

    //public OverlayTile PlayerSetTile() => CurrentTile;// helper

    private void Awake()
    {
        Instance = this; // set up the player accessor

        currentAP = maxAP; // Start with 2AP
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

        EN -= costEN; // EN - costEN

        return true;
    } 

    public void RestoreEN(int amountEN)
    {
        // if the amount is negative return
        if (amountEN <= 0) return;

        EN = Mathf.Min(maxEN, EN + amountEN); // compare maxEN and the current EN + restore amount and use the minimum, make sure we don't over cap the EN limit
    }
        
}
