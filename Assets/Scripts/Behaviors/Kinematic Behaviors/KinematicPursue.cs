using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicPursue : KinematicSeek
{
    // The maximum prediction time.
    public float maxPredication;

    public KinematicPursue() : base()
    {
    }

    public KinematicPursue(float maxSpeed) : base(maxSpeed)
    {
    }

    public override SteeringOutput GetSteering(Transform target, Transform origin, Vector3 velocity)
    {
        SteeringOutput result = new SteeringOutput();

        // 1. Calculate the target to delegate to seek.
        debugTarget = target.position;
        debugOrigin = origin.position;
        Vector3 direction = target.position - origin.position;
        float distance = direction.magnitude;

        // Work out our current speed.
        float speed = velocity.magnitude;

        // Check if speed gives a reasonable prediction time.
        float prediction;
        if (speed <= distance / maxPredication)
        {
            prediction = maxPredication;
        }
        else
        {
            prediction = distance / speed;
        }

        Vector3 targetPrediction = target.position;
        targetPrediction += target.GetComponent<NPC>().velocity * prediction;

        return GetSteering(targetPrediction, origin.position, velocity);
    }

    public override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(debugOrigin, debugTarget);
    }
}
