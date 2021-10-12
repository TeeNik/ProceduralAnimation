using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public MeshFilter MeshFilter;

    void Start()
    {
        Vector3[] points = new Vector3[4];
        for (int i = 0; i < 4; ++i)
        {
            points[i] = new Vector3(0, i, 0);
        }
        Mesh mesh = new Mesh();
        MeshCreator.CreateMesh(ref mesh, points);
        MeshFilter.mesh = mesh;
    }
}