using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    private const int maxAP = 2; // action points for player each turn max out at 2 

    [Header("Player Stats")]
    [SerializeField] private int HP;    //  the player's current
    [SerializeField] private int MaxHP; // the player's Max HP
    [SerializeField] private int baseMoveRange; // how far player able to move

    private OverlayTile standingOnTile; // stores the tile

    // public accessor for player's info
    public int hp => HP; 
    public int maxHP => MaxHP;

    //public int MoveRange => moveRange;

    public OverlayTile CurrentTile => standingOnTile;

    public static CharacterInfo Instance { get; private set; } // access
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

    public void PlayerSetTile(OverlayTile tile)
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
        
}
