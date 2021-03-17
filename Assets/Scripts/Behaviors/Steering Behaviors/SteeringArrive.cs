using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringArrive : Behavior
{
    public float rotationSpeed = 2f;
    public float maxSpeed = 15f;
    public float maxAcceleration = 15f;

    // The radius for arriving at the target.
    public float targetRadius = 0.1f;

    // The radius for beginning to slow down.
    public float slowRadius = 10f;

    // The time over which to achieve target speed.
    public float timeToTarget = 0.1f;

    public SteeringArrive()
    {
        this.rotationSpeed = 2f;
        this.maxSpeed = 15f;
        this.maxAcceleration = 15f;
        this.targetRadius = 0.1f;
        this.slowRadius = 10f;
        this.timeToTarget = 0.1f;
    }

    public SteeringArrive(float rotationSpeed, float maxSpeed, float maxAcceleration, float targetRadius, float slowRadius, float timeToTarget)
    {
        this.rotationSpeed = rotationSpeed;
        this.maxSpeed = maxSpeed;
        this.maxAcceleration = maxAcceleration;
        this.targetRadius = targetRadius;
        this.slowRadius = slowRadius;
        this.timeToTarget = timeToTarget;
    }

    void FixedUpdate()
    {
        /*
        SteeringOutput steering = GetSteering(target.position - transform.position);

        // Update the position.
        transform.position += velocity * Time.deltaTime;

        // Update velocity.
        velocity += steering.linear * Time.deltaTime;

        // Limit velocity.
        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity *= maxSpeed;
        }
        */
    }

    public override SteeringOutput GetSteering(Vector3 target, Vector3 origin, Vector3 velocity)
    {
        debugTarget = target;
        debugOrigin = origin;
        Vector3 direction = target - origin;
        SteeringOutput result = new SteeringOutput();

        // Get the direction to the target.
        float distance = direction.magnitude;

        // Check if we are there, return no steering.
        if (distance < targetRadius)
        {
            // No steering.
            result.linear = Vector3.zero;

            return result;
        }

        float targetSpeed = 0f;
        // If we are outside the slowRadius, then move at max speed.
        if (distance > slowRadius)
        {
            targetSpeed = maxSpeed;
        }
        // Otherwise calculate a scaled speed.
        else
        {
            targetSpeed = maxSpeed * (distance / slowRadius);

        }

        // The target velocity combines speed and direction.
        Vector3 targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        // Acceleration tries to get to the target velocity.
        result.linear = targetVelocity - velocity;
        result.linear /= timeToTarget;

        // Check if the acceleration is too fast.
        if (result.linear.magnitude > maxAcceleration)
        {
            result.linear.Normalize();
            result.linear *= maxAcceleration;
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
        float x = slowRadius * Mathf.Cos(theta);
        float y = slowRadius * Mathf.Sin(theta);
        Vector3 pos = debugOrigin + new Vector3(x, 0, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = slowRadius * Mathf.Cos(theta);
            y = slowRadius * Mathf.Sin(theta);
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
