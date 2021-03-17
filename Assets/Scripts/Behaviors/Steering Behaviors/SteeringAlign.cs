using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringAlign : Align
{
    private float rotationSpeedRads;
    public float slowDownThreshold = 5.0f;
    private float goalRotationSpeedRads;
    public float maxRotationSpeedRads = 2;
    public float maxRotationAccelerationRads = 1;
    private float accelerationRads = 0.1f;

    // The angle for arriving at the target.
    public float targetAngle = 1f;

    // The time over which to achieve target speed.
    public float timeToTarget = 0.25f;

    public Transform target;

    Quaternion lookWhereYoureGoing;

    public SteeringAlign()
    {
        slowDownThreshold = 5.0f;
        maxRotationSpeedRads = 2;
        maxRotationAccelerationRads = 1;
        accelerationRads = 0.1f;
    }

    public SteeringAlign(float rotationSpeedRads)
    {
        this.rotationSpeedRads = rotationSpeedRads;
        slowDownThreshold = 5.0f;
        maxRotationSpeedRads = 2;
        maxRotationAccelerationRads = 1;
        accelerationRads = 0.1f;
    }

    void Update()
    {
        // transform.rotation = GetRotation((target.transform.position - transform.position).normalized);
    }

    public override Quaternion GetRotation(Transform target, Transform origin, bool flee)
    {
        Vector3 goalFacing;
        if (flee)
            goalFacing = (origin.transform.position - target.position).normalized;
        else
            goalFacing = (target.transform.position - origin.position).normalized;

        float goalAngleToTarget = Vector3.Angle(goalFacing, origin.forward);

        // No need to rotate if we're close enough.
        if (goalAngleToTarget < targetAngle)
        {
            rotationSpeedRads = 0;
        }

        // Generate a speed that we would like to reach.
        goalRotationSpeedRads = maxRotationSpeedRads * (goalAngleToTarget / slowDownThreshold);

        // Calculate the radial acceleration
        accelerationRads = (goalRotationSpeedRads - rotationSpeedRads) / timeToTarget;

        // Enforce the max rotational acceleration
        if (Mathf.Abs(accelerationRads) >= maxRotationAccelerationRads)
        {
            accelerationRads = maxRotationAccelerationRads;
        }

        // Apply the acceleration to the current rotation speed.
        rotationSpeedRads = rotationSpeedRads + (accelerationRads * Time.deltaTime);
        if (Mathf.Abs(rotationSpeedRads) >= maxRotationSpeedRads)
        {
            rotationSpeedRads = maxRotationSpeedRads;
        }

        // Rotate.
        lookWhereYoureGoing = Quaternion.LookRotation(goalFacing, Vector3.up);
        return Quaternion.RotateTowards(origin.rotation, lookWhereYoureGoing, rotationSpeedRads);
    }

    public override Quaternion GetRotation(Vector3 target, Transform origin, bool flee)
    {
        Vector3 goalFacing;
        if (flee)
            goalFacing = (origin.position - target).normalized;
        else
            goalFacing = (target - origin.position).normalized;

        float goalAngleToTarget = Vector3.Angle(goalFacing, origin.forward);

        // No need to rotate if we're close enough.
        if (goalAngleToTarget < targetAngle)
        {
            rotationSpeedRads = 0;
        }

        // Generate a speed that we would like to reach.
        goalRotationSpeedRads = maxRotationSpeedRads * (goalAngleToTarget / slowDownThreshold);

        // Calculate the radial acceleration
        accelerationRads = (goalRotationSpeedRads - rotationSpeedRads) / timeToTarget;

        // Enforce the max rotational acceleration
        if (Mathf.Abs(accelerationRads) >= maxRotationAccelerationRads)
        {
            accelerationRads = maxRotationAccelerationRads;
        }

        // Apply the acceleration to the current rotation speed.
        rotationSpeedRads = rotationSpeedRads + (accelerationRads * Time.deltaTime);
        if (Mathf.Abs(rotationSpeedRads) >= maxRotationSpeedRads)
        {
            rotationSpeedRads = maxRotationSpeedRads;
        }

        // Rotate.
        lookWhereYoureGoing = Quaternion.LookRotation(goalFacing, Vector3.up);
        return Quaternion.RotateTowards(origin.rotation, lookWhereYoureGoing, rotationSpeedRads);
    }
}
