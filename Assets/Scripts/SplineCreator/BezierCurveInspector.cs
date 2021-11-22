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

    private void OnSceneGUI()
    {
		curve = target as BezierCurve;
		transform = curve.transform;
		rotation = Tools.pivotRotation == PivotRotation.Local ? transform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint(0);
		Vector3 p1 = ShowPoint(1);
		Vector3 p2 = ShowPoint(2);

		Handles.color = Color.gray;
		Handles.DrawLine(p0, p1);
		Handles.DrawLine(p1, p2);

		Handles.color = Color.white;
		Vector3 lineStart = curve.GetPoint(0.0f);
		for(int i = 1; i <= lineSteps; ++i)
        {
			Vector3 lineEnd = curve.GetPoint(i / (float)lineSteps);
			Handles.DrawLine(lineStart, lineEnd);
			lineStart = lineEnd;
        }
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
}
