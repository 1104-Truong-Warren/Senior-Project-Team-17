using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int x;
    public int y;
    public int gCost;
    public int hCost;
    public int fCost;
    public PathNode cameFromNode;

    public PathNode(NewGrid<PathNode> grid, int x, int y)
    {
        this.x = x;
        this.y = y;
        this.gCost = int.MaxValue;
        this.hCost = 0;
        this.fCost = int.MaxValue;
        this.cameFromNode = null;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}