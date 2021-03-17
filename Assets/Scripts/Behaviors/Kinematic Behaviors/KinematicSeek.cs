using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicSeek : Behavior
{
    public float maxSpeed = 15f;

    public KinematicSeek()
    {
        this.maxSpeed = 15f;
    }

    public KinematicSeek(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }

    void Update()
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

        result.linear = direction;

        // The velocity is along this direction, at full speed.
        result.linear.Normalize();
        result.linear *= maxSpeed;

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
