using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

//used for reading CSV files
using System.IO;
using JetBrains.Annotations;
using System;

public class Holder : MonoBehaviour
{

    public bool findPath = false;

    public GridBehavior heldObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (findPath)
        {
            foreach (GameObject grid in heldObject.path)
            {
                int xpos = grid.GetComponent<GridStat>().x;
                int ypos = grid.GetComponent<GridStat>().y;
                Debug.Log("Path step at: " + xpos + ", " + ypos);
            }
            findPath = false;
        }



    }
}
