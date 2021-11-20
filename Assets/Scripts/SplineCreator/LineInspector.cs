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
        Handles.color = Color.white;
        Handles.DrawLine(line.A, line.B);
    }
}
