#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Line))]
public class LineInspector : Editor
{
    private void OnSceneGUI()
    {
        Line line = target as Line;
        Transform transform = line.transform;
        Quaternion rotation = Tools.pivotRotation == PivotRotation.Local ? transform.rotation : Quaternion.identity;

        Vector3 a = transform.TransformPoint(line.A);
        Vector3 b = transform.TransformPoint(line.B);
        Handles.color = Color.white;
        Handles.DrawLine(a, b);

        Handles.DoPositionHandle(a, rotation);
        Handles.DoPositionHandle(b, rotation);

        EditorGUI.BeginChangeCheck();
        a = Handles.DoPositionHandle(a, rotation);
        if(EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.A = transform.InverseTransformPoint(a);
        }
        EditorGUI.BeginChangeCheck();
        b = Handles.DoPositionHandle(b, rotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(line, "Move Point");
            EditorUtility.SetDirty(line);
            line.B = transform.InverseTransformPoint(b);
        }
    }
}

#endif