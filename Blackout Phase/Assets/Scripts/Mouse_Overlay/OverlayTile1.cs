// These are based on this channel on YouTube: https://www.youtube.com/@lawlessgames3844
// and some additional fixing from online sources Unity Discussion:https://discussions.unity.com/, reddit, YouTube
// I should have keep tract on the exact page but I forgot to save some of the links 
// Weijun
using UnityEngine;

public class OverlayTile1 : MonoBehaviour
{
    public int G; // tile point
    public int H; // tile point

    public int F { get { return G + H; } } // returns the tile point

    public bool isBlocked; // flag to see if tile is blocked

    public OverlayTile1 previousTile; // store the last tile

    public Vector3Int gridLocation;  // locatin of the grid

    public bool hasPlayer; // has player flag?
    public bool hasEnemy;  // has enmey flag?
    public bool debugSelected = false; // debug flag

    public bool hasItem; // has item flag - Ellison
    [SerializeField] public WorldItemInfo itemOnTile; // the item on the tile - Ellison

    public bool Occupied => hasEnemy || hasPlayer; // either condition is true the tile is being used

    /*
    public void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f); // translucent red
    }*/

    // Update is called once per frame
    //void Update()
    //{
    //    /*if (Input.GetMouseButtonDown(0))
    //    {
    //        //gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

    //        HideTile(); // calls the hide in the beginning to make it transparent
    //    }*/
    //}

    // show tile
    public void ShowEnemyTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, 0.85f); // get the sprite render change display color, enemy red
    }

    public void ShowPlayerTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 0.7f, 1f, 0.85f); // get the sprite render change display color, player blue
    }

    public void ShowPlayerMoveRangeTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f); // get the sprite render change display color, player movement white
    }

    public void ShowPlayerAttackRangeTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0f, 0.55f); // get the sprite render change display color, player attack orange
    }

    // hide it changing color
    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0); // get the sprite render change display color
    }

    // resets all the tiles to initial
    public void ResetTiles()
    {
        G = 0;
        H = 0;

        previousTile = null;

        HideTile();
    }

    // function to pick up item on the tile - Ellison
    public void PickUpItem()
    {
        if (hasItem)
        {
            Inventory.instance.Add(itemOnTile.item); // Add the item to the inventory
            hasItem = false; // Remove the item from the tile
            Destroy(itemOnTile.gameObject); // Destroy the item GameObject in the scene
            itemOnTile = null; // Clear the reference to the item
        }
    }
}
// us draw Gizmos to display the x,y on the editor before running the game
//#if UNITY_EDITOR

//private void OnDrawGizmos()
//{
//    if (debugSelected) // if debug is true
//    {
//        //Gizmos.color = Color.red; // shows Red

//        //Gizmos.DrawWireCube(transform.position, new Vector3(1, 0.1f, 0.5f)); // draw a cube on the position

//        UnityEditor.Handles.Label(transform.position + new Vector3(0, 0.15f, 0),
//        $"({gridLocation.x}, {gridLocation.y})"); // checks the position of x&y while editing using Gizmos

//    }
//}
//#endif


