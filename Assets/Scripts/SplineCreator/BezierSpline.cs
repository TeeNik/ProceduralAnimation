using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour
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
		int i;
		if(t >= 1.0f)
        {
			t = 1.0f;
			i = points.Length - 4;
        }
		else
        {
			t = Mathf.Clamp01(t) * GetCurveCount();
			i = (int)t;
			t -= i;
			i *= 3;
        }
		return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}

	public Vector3 GetVelocity(float t)
	{
		int i;
		if (t >= 1.0f)
		{
			t = 1.0f;
			i = points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * GetCurveCount();
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) -
			transform.position;
	}

	public Vector3 GetDirection(float t)
	{
		return GetVelocity(t).normalized;
	}

	public void AddCurve()
	{
		Vector3 point = points[points.Length - 1];
		Array.Resize(ref points, points.Length + 3);
		point.x += 1.0f;
		points[points.Length - 3] = point;
		point.x += 1.0f;
		points[points.Length - 2] = point;
		point.x += 1.0f;
		points[points.Length - 1] = point;
		point.x += 1.0f;
	}

	public int GetCurveCount()
	{
		return (points.Length - 1) / 3;
	}
}
