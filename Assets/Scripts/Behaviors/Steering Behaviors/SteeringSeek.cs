using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringSeek : Behavior
{
    public float maxSpeed = 15f;
    public float maxAcceleration = 15f;

    public SteeringSeek()
    {
        this.maxSpeed = 15f;
        this.maxAcceleration = 15f;
    }

    public SteeringSeek(float maxSpeed, float maxAcceleration)
    {
        this.maxSpeed = maxSpeed;
        this.maxAcceleration = maxAcceleration;
    }

    void Update()
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

        result.linear = direction;

        // The velocity is along this direction, at full speed.
        result.linear.Normalize();
        result.linear *= maxAcceleration;

        return result;
    }

    public override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(debugOrigin, debugTarget);
    }

    public override SteeringOutput GetSteering(Transform target, Transform origin, Vector3 velocity)
    {
        throw new System.NotImplementedException();
    }
}
