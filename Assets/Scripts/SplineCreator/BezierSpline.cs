using UnityEngine;
using System;

public enum BezierControlPointMode
{
	Free,
	Aligned,
	Mirrored
}

public class BezierSpline : MonoBehaviour
{
	[SerializeField] private Vector3[] points;
	[SerializeField] private BezierControlPointMode[] modes;

	public void Reset()
	{
		points = new Vector3[] {
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(2.0f, 0.0f, 0.0f),
			new Vector3(3.0f, 0.0f, 0.0f),
			new Vector3(4.0f, 0.0f, 0.0f),
		};
		modes = new BezierControlPointMode[]
		{
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
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

		Array.Resize(ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length - 2];
	}

	public int GetCurveCount()
	{
		return (points.Length - 1) / 3;
	}

	public int GetControlPointCount()
    {
		return points.Length;
    }

	public Vector3 GetControlPoint(int index)
    {
		return points[index];
    }

	public void SetControlPoint(int index, Vector3 point)
    {
		points[index] = point;
    }

	public BezierControlPointMode GetControlPointMode(int index)
	{
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		modes[(index + 1) / 3] = mode;
	}
}
