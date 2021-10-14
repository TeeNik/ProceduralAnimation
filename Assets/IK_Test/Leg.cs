using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public MeshFilter MeshFilter;
    public IK IK;

    void Start()
    {
        IK.OnBonesUpdated = OnBonesUpdated;
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
        Mesh mesh = new Mesh();
        MeshCreator.CreateMesh(ref mesh, position, 30, 0.25f);
        MeshFilter.mesh = mesh;
    }
}