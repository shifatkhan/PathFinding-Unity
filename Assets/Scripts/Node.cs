using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    public int gCost; // Traversed cost - distance from start node.
    public int hCost; // Heuristic cost - distance from end node.

    public Node parent;

    int heapIndex;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    /// <summary>
    /// Get the total cost , f cost
    /// </summary>
    /// <returns></returns>
    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
