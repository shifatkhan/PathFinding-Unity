using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 10;
    Vector3[] path;
    int targetIndex;
    public float arrivalDistance = 1f;

    private NPC npc;

    private void Awake()
    {
        npc = GetComponent<NPC>();
    }

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;

            int pathLength = path.Length;
            float totalLength = 0;
            for (int i = 0; i < pathLength-1; i++)
            {
                totalLength += (path[i + 1] - path[i]).magnitude;
            }

            Debug.Log($"Total path length = {totalLength}");

            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if((currentWaypoint - transform.position).magnitude <= arrivalDistance)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    // Exit out of coroutine since we reached the end.
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            //transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            npc.SetTarget(currentWaypoint);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            // Draw path.
            for (int i = targetIndex; i < path.Length; i++)
            {
                // Draw nodes.
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(path[i], 0.5f);

                // Draw lines.
                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
