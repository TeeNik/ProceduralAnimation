using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public MeshFilter MeshFilter;
    public IK IK;
    public Movement Movement;

    private Vector3[] _positions = null;

    void Start()
    {
        //IK.OnBonesUpdated = OnBonesUpdated;
        Movement.OnBonesUpdated = OnBonesUpdated;
        //Vector3[] points = new Vector3[5];
        //for (int i = 0; i < 5; ++i)
        //{
        //    points[i] = new Vector3(i, 0, 0);
        //}
        //Mesh mesh = new Mesh();
        //MeshCreator.CreateMesh(ref mesh, points, 30, 1);
        //MeshFilter.mesh = mesh;
    }

    void OnBonesUpdated(Vector3[] position)
    {
        List<Vector3> pos = new List<Vector3>();
        Transform child = transform.GetChild(0);
        while(child.childCount > 0)
        {
            pos.Add(transform.InverseTransformPoint(child.position));
            child = child.GetChild(0);
        }

        Mesh mesh = new Mesh();
        MeshCreator.CreateMesh(ref mesh, pos.ToArray(), 30, 0.25f);
        MeshFilter.mesh = mesh;
        //_positions = position;
    }

    private void OnDrawGizmos()
    {
        //Transform child = transform;
        //while(child.childCount > 0)
        //{
        //    Gizmos.DrawSphere(child.position, 0.25f);
        //    child = transform.GetChild(0);
        //}

        //if (_positions != null)
        //{
        //    foreach(Vector3 pos in _positions)
        //    {
        //        Gizmos.DrawSphere(transform.TransformPoint(pos), 0.5f);
        //    }
        //}

        var current = transform;
        while (current.childCount > 0)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position,
                Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.GetChild(0);
        }
    }
}