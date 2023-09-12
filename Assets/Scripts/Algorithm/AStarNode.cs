using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    [Header("AStarNode Variables")]
    public int h = 0;   // Estimated cost from this cell to target
    public int g = 0;   // Total cost to get to this cell from start
    public int f = 0;   // Sum of H and G
    public Cell cell;
    public AStarNode parentNode;

    public AStarNode(Cell cell, AStarNode parent = null, int h = 0, int g = 0, int f = 0)
    {
        this.cell = cell;
        parentNode = parent;

        this.h = h;
        this.g = g;
        this.f = f;
    }
}
