using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, IHeapItem<Node>
{
    public bool displayGizmos;
    public float gizmoSize = 0.9f;
    public bool walkable;

    [Header("Coordinates")]
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    [Header("Pathfinding")]
    public float gCost; // Traversed cost - distance from start node.
    public float hCost; // Heuristic cost - distance from end node.

    public Node parent;
    public List<Node> neighbours;
    private NodeGraph nodeGraph;

    int heapIndex;

    [System.Obsolete("We converted this class into a Monobehaviour. We do not need a constructor anymore.", true)]
    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    private void Awake()
    {
        nodeGraph = GameObject.FindGameObjectWithTag("Tile Manager").GetComponent<NodeGraph>();
        if (nodeGraph.graphType == NodeGraph.GraphType.POV)
        {
            SetWorldPosition();
            FindPOVNeighbours();
        }
    }

    private void Update()
    {
        
    }

    /// <summary>
    /// Get the total cost , f cost
    /// </summary>
    /// <returns></returns>
    public float fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get => heapIndex;
        set => heapIndex = value;
    }

    public void SetWorldPosition()
    {
        worldPosition = transform.position;
    }

    /// <summary>
    /// Automatically setup neighbours using raycasting.
    /// </summary>
    public void FindPOVNeighbours()
    {
        int graphLength = nodeGraph.povGrid.Count;

        for (int i = 0; i < graphLength; i++)
        {
            // Skip self.
            if (this == nodeGraph.povGrid[i])
                continue;

            RaycastHit hit;
            if (!Physics.Linecast(transform.position, nodeGraph.povGrid[i].worldPosition, out hit, layerMask: nodeGraph.unwalkableLayer))
            {
                neighbours.Add(nodeGraph.povGrid[i]);
            }
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        if (compare == 0)
        {
            compare = gCost.CompareTo(nodeToCompare.gCost);
        }

        return -compare;
    }

    private void OnDrawGizmos()
    {
        if (displayGizmos)
        {
            if (walkable)
            {
                Gizmos.color = Color.white;

                if (neighbours != null)
                {
                    int neighboursLength = neighbours.Count;
                    for (int i = 0; i < neighboursLength; i++)
                    {
                        if (neighbours[i] != null && neighbours[i].walkable)
                            Gizmos.DrawLine(worldPosition, neighbours[i].worldPosition);
                    }
                }
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawCube(worldPosition, new Vector3(gizmoSize, 0.2f, gizmoSize));
        }
    }
}
