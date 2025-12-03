using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    public int G;
    public int H;

    public int F { get { return G +  H; } } // returns the tile point

    public bool isBlocked; // flag to see if tile is blocked

    public OverlayTile previousTile; // store the last tile

    public Vector3Int gridLocation; 

    /*
    public void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f); // translucent red
    }*/

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            //gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

            HideTile(); // calls the hide in the beginning to make it transparent
        }*/
    }
 
    // show tile
    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1); // get the sprite render change display color
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
}
