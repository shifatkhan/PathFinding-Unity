using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringOutput
{
    public Vector3 linear;
	public Quaternion angular;

	/// <summary>
	/// Default Constructor.
	/// </summary>
	public SteeringOutput()
	{
		linear = new Vector3(0, 0, 0);
		angular = new Quaternion();
	}

	public SteeringOutput(Vector3 linear)
	{
		this.linear = linear;
		angular = new Quaternion();
	}
}
