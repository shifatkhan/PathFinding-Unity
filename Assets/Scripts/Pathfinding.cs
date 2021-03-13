using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;

    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        FindPathAstar(seeker.position, target.position);
    }

    void FindPathAstar(Vector3 startPos, Vector3 targetPos)
    {
        // DEBUG
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            // Check if we reached goal.
            if (currentNode == targetNode)
            {
                // DEBUG
                //sw.Stop();
                //print($"Path found: {sw.ElapsedMilliseconds} ms");

                RetracePath(startNode, targetNode);
                return;
            }

            // Check if neighbour is walkable or not.
            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                if(newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode; // To retrace path.

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // Reverse path to be from start to end.
        path.Reverse();

        grid.path = path;
    }

    /// <summary>
    /// We set diagonal nodes to have a distance of 14, and perpendicular nodes to be 10.
    /// So we end up calculating the distance with: 14y + 10(x - y)
    /// </summary>
    /// <param name="nodeA">Origin node</param>
    /// <param name="nodeB">Target node</param>
    /// <returns>Distance between nodes.</returns>
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
