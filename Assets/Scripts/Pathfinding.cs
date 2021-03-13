using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;

[RequireComponent(typeof(PathRequestManager))]
public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;

    Grid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPathAstar(startPos, targetPos));
    }

    IEnumerator FindPathAstar(Vector3 startPos, Vector3 targetPos)
    {
        // DEBUG
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
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

                    pathSuccess = true;
                    break;
                }

                // Check if neighbour is walkable or not.
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int newCostToNeighbour = currentNode.gCost + HeuristicDistance(currentNode, neighbour);

                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = HeuristicDistance(neighbour, targetNode);
                        neighbour.parent = currentNode; // To retrace path.

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        yield return null;

        if (pathSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(wayPoints, pathSuccess);
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        // Reverse path to be from start to end.
        Array.Reverse(waypoints);

        return waypoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        int pathLength = path.Count;
        for (int i = 1; i < pathLength; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }

            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    
    private int HeuristicDistanceVector(Node nodeA, Node nodeB)
    {
        return (int)Mathf.Abs((nodeA.worldPosition - nodeB.worldPosition).magnitude);
    }

    /// <summary>
    /// We set diagonal nodes to have a distance of 14, and perpendicular nodes to be 10.
    /// So we end up calculating the distance with: 14y + 10(x - y)
    /// </summary>
    /// <param name="nodeA">Origin node</param>
    /// <param name="nodeB">Target node</param>
    /// <returns>Distance between nodes.</returns>
    private int HeuristicDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);

        return 14 * dstX + 10 * (dstY - dstX);
    }

    private void DebugDrawNode(Node node)
    {

    }
}
