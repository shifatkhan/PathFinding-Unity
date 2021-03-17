using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicArrive : Behavior
{
    // The time over which to achieve target speed.
    public float timeToTarget = 0.25f;
    public float rotationSpeed = 2f;
    public float maxSpeed = 15f;
    public float radius = 0.1f;

    public KinematicArrive()
    {
        this.timeToTarget = 0.25f;
        this.rotationSpeed = 2f;
        this.maxSpeed = 15f;
        this.radius = 0.1f;
    }

    public KinematicArrive(float timeToTarget, float rotationSpeed, float maxSpeed, float radius)
    {
        this.timeToTarget = timeToTarget;
        this.rotationSpeed = rotationSpeed;
        this.maxSpeed = maxSpeed;
        this.radius = radius;
    }

    void FixedUpdate()
    {
        /*
        SteeringOutput steering = GetSteering(target.position - transform.position);
        
        // Update the position.
        transform.position += steering.linear * Time.deltaTime;
        */
    }

    public override SteeringOutput GetSteering(Vector3 target, Vector3 origin, Vector3 velocity)
    {
        debugTarget = target;
        debugOrigin = origin;
        Vector3 direction = target - origin;

        SteeringOutput result = new SteeringOutput();

        // Get the direction to the target.
        result.linear = direction;

        if(result.linear.magnitude < radius)
        {
            // No steering.
            result.linear = Vector3.zero;

            return result;
        }

        // We need to move to our target, we'd like to
        // get there in timeToTarget seconds.
        result.linear /= timeToTarget;

        // Limit velocity.
        if (result.linear.magnitude > maxSpeed)
        {
            result.linear.Normalize();
            result.linear *= maxSpeed;
        }

        return result;
    }

    // ====================== DEBUGGING ====================== //

    public override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(debugOrigin, debugTarget);

        // Draw slow radius
        Gizmos.color = Color.white;
        float theta = 0;
        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);
        Vector3 pos = debugOrigin + new Vector3(x, 0, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);
            newPos = debugOrigin + new Vector3(x, 0, y);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);
    }

    public override SteeringOutput GetSteering(Transform target, Transform origin, Vector3 velocity)
    {
        throw new System.NotImplementedException();
    }
}
