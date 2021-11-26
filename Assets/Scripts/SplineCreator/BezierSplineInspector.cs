using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineInspector : Editor
{
    private BezierSpline spline;
    private Transform transform;
    private Quaternion rotation;

	private const int stepsPerCurve = 10;
	private const float directionalScale = 0.5f;

	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	private int selectedIndex = -1;

	private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	private void OnSceneGUI()
	{
		spline = target as BezierSpline;
		transform = spline.transform;
		rotation = Tools.pivotRotation == PivotRotation.Local ? transform.rotation : Quaternion.identity;

		Vector3 p0 = ShowPoint(0);
		for (int i = 1; i < spline.GetControlPointCount(); i += 3)
        {
			Vector3 p1 = ShowPoint(i);
			Vector3 p2 = ShowPoint(i + 1);
			Vector3 p3 = ShowPoint(i + 2);

			Handles.color = Color.gray;
			Handles.DrawLine(p0, p1);
			Handles.DrawLine(p2, p3);

			Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2.0f);
			p0 = p3;
		}

		ShowDirections();
	}

	public override void OnInspectorGUI()
    {
		spline = target as BezierSpline;
		EditorGUI.BeginChangeCheck();
		bool loop = EditorGUILayout.Toggle("Loop", spline.GetLoopValue());
		if(EditorGUI.EndChangeCheck())
        {
			Undo.RecordObject(spline, "Toggle Loop");
			EditorUtility.SetDirty(spline);
			spline.SetLoopValue(loop);
        }

		if(selectedIndex >= 0 && selectedIndex < spline.GetControlPointCount())
        {
			DrawSelectedPointInspector();
        }

		if (GUILayout.Button("Add Curve"))
        {
			Undo.RecordObject(spline, "Add Curve");
			spline.AddCurve();
			EditorUtility.SetDirty(spline);
        }
    }

	private void DrawSelectedPointInspector()
    {
		GUILayout.Label("Selected Point");
		EditorGUI.BeginChangeCheck();
		Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
		if(EditorGUI.EndChangeCheck())
        {
			Undo.RecordObject(spline, "Move Point");
			EditorUtility.SetDirty(spline);
			spline.SetControlPoint(selectedIndex, point);
        }

		EditorGUI.BeginChangeCheck();
		BezierControlPointMode mode = (BezierControlPointMode)
			EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
		if(EditorGUI.EndChangeCheck())
        {
			Undo.RecordObject(spline, "Chance Point Mode");
			spline.SetControlPointMode(selectedIndex, mode);
			EditorUtility.SetDirty(spline);
        }
    }

	private Vector3 ShowPoint(int index)
	{
		Vector3 point = transform.TransformPoint(spline.GetControlPoint(index));
		float size = HandleUtility.GetHandleSize(point);
		if(index == 0)
        {
			size *= 2.0f;
        }
		Handles.color = modeColors[(int)spline.GetControlPointMode(index)];
		if(Handles.Button(point, rotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
			selectedIndex = index;
			Repaint();
        }
		if(selectedIndex == index)
        {
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, rotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(spline, "Move Point");
				EditorUtility.SetDirty(spline);
				spline.SetControlPoint(index, transform.InverseTransformPoint(point));
			}
		}
		return point;
	}

	private void ShowDirections()
	{
		Handles.color = Color.green;
		Vector3 point = spline.GetPoint(0.0f);
		Handles.DrawLine(point, point + spline.GetDirection(0.0f) * directionalScale);
		int steps = stepsPerCurve * spline.GetCurveCount();
		for (int i = 1; i <= steps; ++i)
		{
			point = spline.GetPoint(i / (float)steps);
			Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionalScale);
		}
	}
}
