using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGraph : MonoBehaviour
{
    public enum GraphType
    {
        GRID,
        POV
    }

    [SerializeField]
    private GameObject nodePrefab;
    [SerializeField]
    private bool displayGridGizmos = false;

    [Header("Graph")]
    [SerializeField]
    private LayerMask unwalkableLayer;
    [SerializeField]
    private GraphType graphType = GraphType.GRID;
    public Vector2 gridWorldSize;

    [Header("Node")]
    public float nodeRadius;
    Node[,] grid;

    [SerializeField]
    private List<Node> povGrid;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    /// <summary>
    /// Create the tiled grid while looking out for obstacles.
    /// </summary>
    void CreateGrid()
    {
        // Create grid / tiled graph.
        if(graphType == GraphType.GRID)
        {
            grid = new Node[gridSizeX, gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridSizeY / 2;

            // Create nodes and Check for collision with unwalkable objects.
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    // Create vectors for each tile.
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableLayer));

                    // Create the node.
                    Node createdNode = Instantiate(nodePrefab, worldPoint, Quaternion.identity, transform).GetComponent<Node>();
                    createdNode.walkable = walkable;
                    createdNode.worldPosition = worldPoint;
                    createdNode.gridX = x;
                    createdNode.gridY = y;
                    createdNode.displayGizmos = displayGridGizmos;

                    grid[x, y] = createdNode;
                }
            }
        }
        // Create PoV style graph.
        else if(graphType == GraphType.POV)
        {
            int gridLength = povGrid.Count;
            for (int i = 0; i < gridLength; i++)
            {
                povGrid[i].walkable = true;
                povGrid[i].worldPosition = povGrid[i].transform.position;
                povGrid[i].displayGizmos = displayGridGizmos;
            }
        }
    }

    /// <summary>
    /// Get all Nodes that are neighbours of a specific node.
    /// </summary>
    /// <param name="node">Node to check around.</param>
    /// <returns>List of node neighbours.</returns>
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        if (graphType == GraphType.GRID)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
        }
        else if (graphType == GraphType.POV)
        {
            neighbours = node.neighbours;
        }

        return neighbours;
    }

    /// <summary>
    /// Get the node that is at a position in the world space.
    /// </summary>
    /// <param name="worldPosition">Coord of a world position.</param>
    /// <returns>The node that is in the coord</returns>
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Node nodeAtPosition = null;

        if (graphType == GraphType.GRID)
        {
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

            nodeAtPosition = grid[x, y];
        }
        else if (graphType == GraphType.POV)
        {
            float currDistance = Mathf.Infinity;
            int gridLength = povGrid.Count;

            Debug.Log($"WORLDPOS: {worldPosition}");

            // Search for the closes Node. O(n)
            for (int i = 0; i < gridLength; i++)
            {
                float newDst = (worldPosition - povGrid[i].worldPosition).magnitude;
                if (newDst < currDistance)
                {
                    nodeAtPosition = povGrid[i];
                    currDistance = newDst;
                }
            }
        }

        Debug.Log($"Return node: {nodeAtPosition.name}");
        return nodeAtPosition;
    }
}
