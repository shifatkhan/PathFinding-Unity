using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using UnityEngine;

[RequireComponent(typeof(PathRequestManager))]
public class Pathfinding : MonoBehaviour
{
    // Enum to keep track of which Heuristics we are using.
    public enum Heuristic
    {
        DIJKSTRA,
        EUCLIDEAN,
        CLUSTER
    }

    [SerializeField] 
    private Heuristic heuristic = Heuristic.EUCLIDEAN;

    PathRequestManager requestManager;

    Grid grid;

    private List<Node> openListNodes = new List<Node>();
    private List<Node> closedListNodes = new List<Node>();

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
        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        // Initialize vars.
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        startNode.parent = startNode;

        if (heuristic != Heuristic.DIJKSTRA)
            startNode.hCost = HeuristicDistance(startNode, targetNode);

        // Check if we can even reach the target node.
        if (startNode.walkable && targetNode.walkable)
        {
            // This list is used for showing the visited nodes.
            openListNodes.Clear();
            closedListNodes.Clear();

            // Initialize open and closed sets.
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            // Iterate through processing each node.
            while (openSet.Count > 0)
            {
                // Get the closest node.
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                closedListNodes.Add(currentNode);

                // Check if we reached goal and terminate.
                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                // Otherwise, loop through neighbours.
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    // Skip node if it is not walkable (e.g. a wall) or if it's in closed set.
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    openListNodes.Add(neighbour);
                    float newCostToNeighbour = currentNode.gCost + HeuristicDistance(currentNode, neighbour);

                    // Check if we found a shorter path to this neighbour.
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;

                        if(heuristic != Heuristic.DIJKSTRA)
                            neighbour.hCost = HeuristicDistance(neighbour, targetNode);

                        neighbour.parent = currentNode; // To retrace path.

                        // Add neighbour to openSet.
                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        yield return null;

        // We reached the goal or no more nodes to search.
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

    private float HeuristicDistance(Node nodeA, Node nodeB)
    {
        switch (heuristic)
        {
            case Heuristic.DIJKSTRA:
            case Heuristic.EUCLIDEAN:
                return EuclideanDistanceHeuristic(nodeA, nodeB);
            case Heuristic.CLUSTER:
                return ClusterHeuristic(nodeA, nodeB);
        }

        UnityEngine.Debug.LogError("Invalid Heuristic.");
        return -1;
    }

    /// <summary>
    /// We use Euclidean distance to calculate the distance between 2 nodes.
    /// 
    /// </summary>
    /// <param name="nodeA">Origin node</param>
    /// <param name="nodeB">Target node</param>
    /// <returns>Distance between nodes.</returns>
    public static float EuclideanDistanceHeuristic(Node nodeA, Node nodeB)
    {
        return Mathf.Sqrt(Mathf.Pow(nodeA.worldPosition.x - nodeB.worldPosition.x, 2) + Mathf.Pow(nodeA.worldPosition.z - nodeB.worldPosition.z, 2));
    }

    /// <summary>
    /// We set diagonal nodes to have a distance of 14, and perpendicular nodes to be 10.
    /// This is because we set our nodes to be 1 unit apart from each other.
    /// sqrt(1^2 + 1^2) = 1.4 --> multiply by 10 for better readability.
    /// 
    /// So we end up calculating the distance with: 14y + 10(x - y)
    /// </summary>
    /// <param name="nodeA">Origin node</param>
    /// <param name="nodeB">Target node</param>
    /// <returns>Distance between nodes.</returns>
    [System.Obsolete("This function was replaced by a more precise function: EuclideanDistanceHeuristic")]
    public static float EuclideanDistanceHeuristic2(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);

        return 14 * dstX + 10 * (dstY - dstX);
    }

    public static float ClusterHeuristic(Node nodeA, Node nodeB)
    {
        return 0;
    }

    private void OnDrawGizmos()
    {
        if(openListNodes != null && openListNodes.Count > 0)
        {
            int nodesLength = openListNodes.Count;
            for (int i = 0; i < nodesLength; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(openListNodes[i].worldPosition, Vector3.one * 0.25f);
            }
        }

        if (closedListNodes != null && closedListNodes.Count > 0)
        {
            int nodesLength = closedListNodes.Count;
            for (int i = 0; i < nodesLength; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(closedListNodes[i].worldPosition, Vector3.one * 0.25f);
            }
        }
    }
}
