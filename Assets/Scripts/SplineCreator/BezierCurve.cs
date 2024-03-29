using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Vector3[] points;

	public void Reset()
	{
		points = new Vector3[] {
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(2.0f, 0.0f, 0.0f),
			new Vector3(3.0f, 0.0f, 0.0f),
			new Vector3(4.0f, 0.0f, 0.0f),
		};
	}

	public Vector3 GetPoint(float t)
    {
		return transform.TransformPoint(Bezier.GetPoint(points[0], points[1], points[2], points[3], t));
    }

	public Vector3 GetVelocity(float t)
    {
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[0], points[1], points[2], points[3], t)) -
			transform.position;
    }

	public Vector3 GetDirection(float t)
    {
		return GetVelocity(t).normalized;
    }
}