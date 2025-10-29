using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private int[,] gridArray;

    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridArray = new int[width, height];

        Debug.Log($"Grid created with width: {width} and height: {height}");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x, y] = 0; // Initialize all cells to 0
            }
        }
    }
}
