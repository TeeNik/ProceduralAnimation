using UnityEngine;

public static class Bezier
{
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