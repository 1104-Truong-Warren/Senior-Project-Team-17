// Stores information about world item
// Inspired by Weijun's EnemyInfo script to keep consistency
// - Ellison

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldItemInfo : MonoBehaviour
{
    public Item item;
    [SerializeField] OverlayTile1 Tile; // tile item is on

    public OverlayTile1 currentTile => Tile; // where the item tile is


    public void ItemSetTile(OverlayTile1 newtile)
    {
        if (Tile != null)
            Tile.hasItem = false;

        Tile = newtile;

        if (Tile != null)
        {
            Tile.hasItem = true;
            Tile.itemOnTile = this;
        }
    }
}
