using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicWander : Behavior
{
    public float maxRotation = 15f;
    public float maxSpeed = 15f;

    public KinematicWander()
    {
        this.maxSpeed = 15f;
    }

    public KinematicWander(float maxSpeed)
    {
        this.maxSpeed = maxSpeed;
    }

    void Update()
    {
        //SteeringOutput steering = GetSteering(null, null, velocity);

        //velocity = steering.linear;
        //// Update the position and random orientation.
        //transform.position += steering.linear * Time.deltaTime;
    }

    public override SteeringOutput GetSteering(Transform target, Transform origin, Vector3 velocity)
    {
        Vector3 newEulerRotation = origin.eulerAngles;
        
        // Only rotation y-axis.
        newEulerRotation.y += Random.Range(-maxRotation, maxRotation);

        origin.eulerAngles = newEulerRotation;

        SteeringOutput result = new SteeringOutput();
        result.linear = origin.forward * maxSpeed;

        return result;
    }

    public override void OnDrawGizmosSelected()
    {
    }

    public override SteeringOutput GetSteering(Vector3 target, Vector3 origin, Vector3 velocity)
    {
        throw new System.NotImplementedException();
    }
}
