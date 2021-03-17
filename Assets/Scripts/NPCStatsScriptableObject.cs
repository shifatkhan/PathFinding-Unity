using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Stats", menuName = "NPC/NPC Stats")]
public class NPCStatsScriptableObject : ScriptableObject
{
    [Header("Movement Vars")]
    public float maxSpeed = 15f;
    public float wanderSpeed = 5f;
    public float nearDistance = 5f;
    public float smallDistance = 2f;
    public float slowRadius = 10f;
    public float targetRadius = 0.1f;
    public float maxPrediction = 1f;
    public float timeToTarget = 0.1f;

    [Header("Rotation Vars")]
    public float rotationSpeed = 2f;
    public float viewAngle = 45f;
    public float minViewAngle = 20f;
    public float viewRange = 10f;
}
