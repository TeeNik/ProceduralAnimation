#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor
{
    private BezierCurve curve;
    private Transform transform;
    private Quaternion rotation;

	private const int lineSteps = 10;
	private const float directionalScale = 0.5f;

    private void OnSceneGUI()
    {
		curve = target as BezierCurve;
		transform = curve.transform;
		rotation = Tools.pivotRotation == PivotRotation.Local ? transform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint(0);
		Vector3 p1 = ShowPoint(1);
		Vector3 p2 = ShowPoint(2);
		Vector3 p3 = ShowPoint(3);

		Handles.color = Color.gray;
		Handles.DrawLine(p0, p1);
		Handles.DrawLine(p2, p3);

		ShowDirections();
		Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2.0f);
	}

	private Vector3 ShowPoint(int index)
	{
		Vector3 point = transform.TransformPoint(curve.points[index]);
		EditorGUI.BeginChangeCheck();
		point = Handles.DoPositionHandle(point, rotation);
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(curve, "Move Point");
			EditorUtility.SetDirty(curve);
			curve.points[index] = transform.InverseTransformPoint(point);
		}
		return point;
	}

	private void ShowDirections()
    {
		Handles.color = Color.green;
		Vector3 point = curve.GetPoint(0.0f);
		Handles.DrawLine(point, point + curve.GetDirection(0.0f) * directionalScale);
		for(int i = 1; i <= lineSteps; ++i)
        {
			point = curve.GetPoint(i / (float)lineSteps);
			Handles.DrawLine(point, point + curve.GetDirection(i / (float)lineSteps) * directionalScale);
        }
    }
}

#endif