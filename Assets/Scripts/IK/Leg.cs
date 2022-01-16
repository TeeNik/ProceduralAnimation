using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public MeshFilter MeshFilter;
    public IKInterface IK;
    public Transform Tip;
    public bool UseMeshGeneration = true;

    void Awake()
    {
        IK.OnBonesUpdated = OnBonesUpdated;
    }

    void OnBonesUpdated(Vector3[] position)
    {
        if(UseMeshGeneration)
        {
            GenerateMeshForLeg();
        } 
    }

    private void GenerateMeshForLeg()
    {
        List<Vector3> pos = new List<Vector3>();
        Transform child = transform;
        while (child.childCount > 0)
        {
            child = child.GetChild(0);
            pos.Add(transform.InverseTransformPoint(child.position));
        }

        Destroy(MeshFilter.sharedMesh);
        Mesh mesh = new Mesh();
        MeshCreator.CreateMesh(ref mesh, pos.ToArray(), 30, 0.1f);
        MeshFilter.sharedMesh = mesh;
    }
}