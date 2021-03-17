using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behavior
{
    // Only used for debugging purposes.
    public Vector3 debugTarget { get; protected set; }
    public Vector3 debugOrigin { get; protected set; }

    protected Behavior()
    {
    }

    public abstract SteeringOutput GetSteering(Vector3 target, Vector3 origin, Vector3 velocity);
    public abstract SteeringOutput GetSteering(Transform target, Transform origin, Vector3 velocity);

    public abstract void OnDrawGizmosSelected();
}
