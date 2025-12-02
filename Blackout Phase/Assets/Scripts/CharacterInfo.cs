using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int HP;    //  the player's current
    [SerializeField] private int MaxHP; // the player's Max HP
                                        //[SerializeField] private int moveRange; // how far player able to move

    private OverlayTile standingOnTile; // stores the tile

    // public accessor for player's info
    public int hp => HP; 
    public int maxHP => MaxHP;

    //public int MoveRange => moveRange;

    public OverlayTile CurrentTile => standingOnTile;

    public static CharacterInfo Instance { get; private set; } // access

    //public OverlayTile PlayerSetTile() => CurrentTile;// helper

    private void Awake()
    {
       Instance = this; // set up the player accessor
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
            HP = 0;

            Debug.Log($"{name} has died.");
        }
    }
        
}
