using System.Collections.Generic; // for the List<T> and dictionary <T, T> for pathfinding// for the List<T> and dictionary <T, T> for pathfinding
using UnityEngine;
using System.Linq; // filter numbers that are greater than 10, x=> x.F is using it, ordering etc...

public class EnemyPathFinder 
{
    private readonly EnemyTileScanner scanner; // access the enemy scanner

    public EnemyPathFinder(EnemyTileScanner scanner)
    {
        this.scanner = scanner; // set it up
    }

    public List<OverlayTile1> FindPath(OverlayTile1 start, OverlayTile1 end)
    {
        // if start or end not found returns a new list
        if (start == null || end == null)
        {
            Debug.LogError("EnemyPathFinder: start/end is null!"); // debug
            return new List<OverlayTile1> ();
        }

        Debug.Log("Pathfinder started: " + start.name + " -> " + end.name); //debug

        List<OverlayTile1> open = new List<OverlayTile1>(); // using a new list for enemy pathfinding

        HashSet<OverlayTile1> closed = new HashSet<OverlayTile1>();

        start.G = 0; // start position

        start.H = Manhattan(start, end); // for the distance calculation

        start.previousTile = null; // not setup yet

        open.Add(start); // add the start to open

        // if open has data read it
        while (open.Count > 0)
        {
            OverlayTile1 current = open.OrderBy(n => n.F).First(); // orders them

            if (current == end)
                return BuildPath(start, end); // reaches the end start buiding

            open.Remove(current); // deletete it

            closed.Add(current); // add them to the closed list

            //var neighbours = scanner.GetNeighbours(current);

            Debug.Log("Current Tile: " + current.gridLocation); //debug

            foreach (OverlayTile1 neighbour in scanner.GetNeighbours(current)) // runs all the neighbour tiles in scanner list
            {
                //Debug.Log("Neighbour count = " + neighbour.Count);

                if (closed.Contains(neighbour)) // if the closed list contains the tile keep on going, skip
                    continue;
                 
                int G = current.G + 1; // calculation for movenment, start-> A->B -> current, G = current+1

                if (!open.Contains(neighbour) || G < neighbour.G) // if it doesn't containt neighbour tile or it's shorter than our distance?
                {
                    neighbour.G = G; // set it to the new distance

                    neighbour.H = Manhattan(neighbour, end); // pass it to do the calculation

                    neighbour.previousTile = current; // set the previousTile to current

                    if (!open.Contains(neighbour))
                        open.Add(neighbour); // if it is not in the contained list add it
                }
            }
        }

        Debug.LogWarning("EnemeyPathFinder: No path found!");
        return new List<OverlayTile1>(); // returns the empty list
    }

    private int Manhattan(OverlayTile1 a, OverlayTile1 b)
    {
        return Mathf.Abs(a.gridLocation.x - b.gridLocation.x) + 
            Mathf.Abs(a.gridLocation.y - b.gridLocation.y);

        // grid math calculation point a -> point b
    }

    //public EnemyTileScanner TestScanner()
    //{
    //    return scanner;  // for testing
    //}

    private List<OverlayTile1> BuildPath(OverlayTile1 start, OverlayTile1 end)
    {
        List<OverlayTile1> path = new List<OverlayTile1>(); // create a new path

        OverlayTile1 current = end; // current overlay is the end result

        // list is not empty and not equal to end result
        while (current != start && current != null)
        {
            path.Add(current); // add the current into the list

            current = current.previousTile; //  backwards writting in data
        }

        path.Reverse(); // reverse the list

        Debug.Log("EnemyPathFinder: path length = " + path.Count); // debug 
        return path; // finished
    }
}
