using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base NPC class that contains most basic behaviors. This class
/// can be extended for more specific behaviors.
/// </summary>
public class NPC : MonoBehaviour
{
    public bool enableGizmos;

    [SerializeField]
    protected bool flee = false;

    [SerializeField]
    protected Vector3 target;

    [SerializeField]
    protected NPCStatsScriptableObject stats;

    protected float maxSpeed = 15f;
    protected float wanderSpeed =  5f;
    protected float nearDistance = 5f;
    protected float smallDistance = 2f;
    protected float slowRadius = 10f;
    protected float targetRadius = 0.1f;
    protected float maxPrediction = 1f;
    protected float timeToTarget = 0.25f;

    [Header("Rotation Vars")]
    protected float rotationSpeed = 2f;
    protected float viewAngle = 45f;
    protected float minViewAngle = 20f;
    protected float currentViewAngle;
    protected float viewRange = 10f;

    // Current velocity.
    public Vector3 velocity { get; protected set; }

    // BEHAVIORS
    protected Behavior[] arrive;
    protected Align[] align;
    protected Behavior wander;
    protected Behavior pursue;

    // 0 = Kinematic, 1 = Steering.
    protected int behaviorIndex = 0;

    [SerializeField]
    private bool steeringAtStart = false;

    // Distance and direction between us and target.
    protected Vector3 distance;

    protected void Awake()
    {
        // Init Vars
        InitializeVars();

        // Initialize behaviors
        arrive = new Behavior[2];
        arrive[0] = new KinematicArrive() { maxSpeed = maxSpeed };
        arrive[1] = new SteeringArrive() { maxSpeed = maxSpeed, timeToTarget = timeToTarget, slowRadius = slowRadius, targetRadius = targetRadius };

        wander = new KinematicWander(wanderSpeed);
        pursue = new KinematicPursue(maxSpeed) { maxPredication = maxPrediction };

        // Initialize Rotation behaviors
        align = new Align[2];
        align[0] = new KinematicAlign(rotationSpeed);
        align[1] = new SteeringAlign();

        if (steeringAtStart)
            behaviorIndex = 1;
        else
            behaviorIndex = 0;
    }

    protected void InitializeVars()
    {
        maxSpeed = stats.maxSpeed;
        wanderSpeed = stats.wanderSpeed;
        nearDistance = stats.nearDistance;
        slowRadius = stats.slowRadius;
        targetRadius = stats.targetRadius;
        smallDistance = stats.smallDistance;
        maxPrediction = stats.maxPrediction;

        rotationSpeed = stats.rotationSpeed;
        viewAngle = stats.viewAngle;
        minViewAngle = stats.minViewAngle;
        viewRange = stats.viewRange;
        timeToTarget = stats.timeToTarget;

        currentViewAngle = viewAngle;
    }

    protected virtual void Update()
    {
        // Update speed-dependent arc
        UpdateViewAngle();

        // Switch between kinematic and steering.
        if (Input.GetButtonDown("Submit"))
        {
            SwitchBetweenKinematicAndSteering();
        }
    }

    protected virtual void FixedUpdate()
    {
        if (target != null)
            distance = target - transform.position;
            if (flee)
                distance *= -1;

        switch (behaviorIndex)
        {
            case 0:
                KinematicUpdate();
                break;
            case 1:
                SteeringUpdate();
                break;
            default:
                Debug.LogError("Invalid behaviorIndex: Should be either 0 or 1.");
                break;
        }
    }

    // ====================== UPDATES ====================== //

    protected virtual void KinematicUpdate()
    {
        KinematicMoveUpdate();
        RotateUpdate();
    }

    protected virtual void SteeringUpdate()
    {
        SteeringMoveUpdate();
        RotateUpdate();
    }

    /// <summary>
    /// Move transform using kinematic forces.
    /// </summary>
    protected virtual void KinematicMoveUpdate()
    {
        SteeringOutput steering;

        if (flee)
            steering = arrive[behaviorIndex].GetSteering(transform.position, target, velocity);
        else
            steering = arrive[behaviorIndex].GetSteering(target, transform.position, velocity);

        // Restrict y translation.
        steering.linear = new Vector3(steering.linear.x, 0f, steering.linear.z);

        velocity = steering.linear;

        // Update the position.
        transform.position += steering.linear * Time.deltaTime;
    }

    /// <summary>
    /// Move transform usin euler steering forces.
    /// </summary>
    protected virtual void SteeringMoveUpdate()
    {
        SteeringOutput steering;

        if (flee)
            steering = arrive[behaviorIndex].GetSteering(transform.position, target, velocity);
        else
            steering = arrive[behaviorIndex].GetSteering(target, transform.position, velocity);

        // Restrict y translation.
        steering.linear = new Vector3(steering.linear.x, 0f, steering.linear.z);

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
    }

    /// <summary>
    /// Rotate transform.
    /// </summary>
    protected void RotateUpdate()
    {
        Quaternion newRotation = align[behaviorIndex].GetRotation(target, transform, flee);
        transform.rotation = Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
    }

    // ====================== UTILS ====================== //

    protected void StopMovement()
    {
        velocity = Vector3.zero;
    }

    /// <summary>
    /// Update view angle based on velocity.
    /// </summary>
    protected void UpdateViewAngle()
    { 
        currentViewAngle = Mathf.Clamp(viewAngle - (velocity.magnitude / maxSpeed * viewAngle), minViewAngle, viewAngle);
    }

    /// <summary>
    /// Check if target is in view angle and within distance.
    /// </summary>
    /// <param name="direction">Distance between us and target.</param>
    /// <returns>Bool in view or not.</returns>
    protected bool IsTargetInView()
    {
        return (Vector3.Angle(transform.forward, distance) < currentViewAngle / 2) && (distance.magnitude <= viewRange);
    }

    public void SetTarget(Vector3 target)
    {
        this.target = target;
    }
    protected void SwitchBetweenKinematicAndSteering()
    {
        velocity = Vector3.zero;
        behaviorIndex = (behaviorIndex + 1) % arrive.Length;
        steeringAtStart = !steeringAtStart;
    }

    public void SetBehaviorIndex(int behaviorIndex)
    {
        velocity = Vector3.zero;
        this.behaviorIndex = (behaviorIndex + 1) % arrive.Length;
        steeringAtStart = behaviorIndex == 1;
    }

    // ====================== DEBUGGING ====================== //
    protected virtual void OnDrawGizmos()
    {
        // DRAW TARGET
        Gizmos.color = Color.green;
        if(target != null)
            Gizmos.DrawLine(transform.position, target);

        if (enableGizmos)
        {
            // DRAW VIEW CONE
            Gizmos.color = Color.yellow;
            float halfAngle = currentViewAngle / 2.0f;

            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);

            Vector3 leftRayDirection = leftRayRotation * transform.forward * viewRange;
            Vector3 rightRayDirection = rightRayRotation * transform.forward * viewRange;

            Gizmos.DrawRay(transform.position, leftRayDirection);
            Gizmos.DrawRay(transform.position, rightRayDirection);

            Gizmos.DrawLine(transform.position + rightRayDirection, transform.position + leftRayDirection);

            // DRAW NEAR RADIUS
            Gizmos.color = Color.white;
            float theta = 0;
            float x = nearDistance * Mathf.Cos(theta);
            float y = nearDistance * Mathf.Sin(theta);
            Vector3 pos = transform.position + new Vector3(x, 0, y);
            Vector3 newPos = pos;
            Vector3 lastPos = pos;
            for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
            {
                x = nearDistance * Mathf.Cos(theta);
                y = nearDistance * Mathf.Sin(theta);
                newPos = transform.position + new Vector3(x, 0, y);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.DrawLine(pos, lastPos);

            // DRAW SMALL RADIUS
            Gizmos.color = Color.grey;
            theta = 0;
            x = smallDistance * Mathf.Cos(theta);
            y = smallDistance * Mathf.Sin(theta);
            pos = transform.position + new Vector3(x, 0, y);
            newPos = pos;
            lastPos = pos;
            for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
            {
                x = smallDistance * Mathf.Cos(theta);
                y = smallDistance * Mathf.Sin(theta);
                newPos = transform.position + new Vector3(x, 0, y);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.DrawLine(pos, lastPos);

            // DRAW SLOW RADIUS
            Gizmos.color = Color.grey;
            theta = 0;
            x = slowRadius * Mathf.Cos(theta);
            y = slowRadius * Mathf.Sin(theta);
            pos = transform.position + new Vector3(x, 0, y);
            newPos = pos;
            lastPos = pos;
            for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
            {
                x = slowRadius * Mathf.Cos(theta);
                y = slowRadius * Mathf.Sin(theta);
                newPos = transform.position + new Vector3(x, 0, y);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.DrawLine(pos, lastPos);
        }
    }
}
