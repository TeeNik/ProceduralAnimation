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

public static class Bezier
{
	//public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    //{
	//	return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    //}

	public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		t = Mathf.Clamp01(t);
		float oneMinusT = 1.0f - t;
		return oneMinusT * oneMinusT * oneMinusT * p0 +
			3.0f * oneMinusT * oneMinusT * t * p1 +
			3.0f * oneMinusT * t * t * p2 +
			t * t * t * p3;
	}

	public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return
			3.0f * oneMinusT * oneMinusT * (p1 - p0) +
			6.0f * oneMinusT * t * (p2 - p1) +
			3.0f * t * t * (p3 - p2);
	}
}
