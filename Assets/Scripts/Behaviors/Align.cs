using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Align
{
    protected Align()
    {
    }

    public abstract Quaternion GetRotation(Transform target, Transform origin, bool flee);

    public abstract Quaternion GetRotation(Vector3 target, Transform origin, bool flee);
}
