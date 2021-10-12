using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public MeshFilter MeshFilter;

    void Start()
    {
        Vector3[] points = new Vector3[5];
        for (int i = 0; i < 5; ++i)
        {
            points[i] = new Vector3(i, 0, 0);
        }
        Mesh mesh = new Mesh();
        MeshCreator.CreateMesh(ref mesh, points, 30, 1);
        MeshFilter.mesh = mesh;
    }
}