using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing an edge in a graph.
/// </summary>
public class Connection
{
    public Node startNode;
    public Node endNode;

    public Connection(Node startNode, Node endNode)
    {
        this.startNode = startNode;
        this.endNode = endNode;
    }

    public float cost
    {
        get
        {
            return Pathfinding.EuclideanDistanceHeuristic(startNode, endNode);
        }
    }
}
