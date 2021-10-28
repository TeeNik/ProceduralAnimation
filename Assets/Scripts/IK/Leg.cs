using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public MeshFilter MeshFilter;
    public IKInterface IK;

    void Start()
    {
        IK.OnBonesUpdated = OnBonesUpdated;
    }

    void OnBonesUpdated(Vector3[] position)
    {
        List<Vector3> pos = new List<Vector3>();
        Transform child = transform;
        while(child.childCount > 0)
        {
            child = child.GetChild(0);
            pos.Add(transform.InverseTransformPoint(child.position));
        }

        Mesh mesh = new Mesh();
        MeshCreator.CreateMesh(ref mesh, pos.ToArray(), 30, 0.1f);
        MeshFilter.mesh = mesh;
    }

    private void OnDrawGizmos()
    {
        /*
        var current = transform;
        while (current.childCount > 0)
        {
            current = current.GetChild(0);
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position,
                Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position),
                new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
        }
        */
    }
}