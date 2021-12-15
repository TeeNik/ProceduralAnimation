using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class FabricIK : IKInterface
{

    public int ChainLength = 2;

    public Transform Target;
    public Transform Pole;

    [Header("Solver Parameters")]
    public int Iterations = 10;
    public float Delta = 0.001f;
    [Range(0, 1)]
    public float SnapBackStrength = 1.0f;

    protected float[] BonesLength;
    protected float CompleteLength;
    protected Transform[] Bones;
    protected Vector3[] Positions;

    protected Vector3[] StartDirectionSucc;
    protected Quaternion[] StartRotationBone;
    protected Quaternion StartRotationTarget;
    protected Quaternion StartRotationRoot;

    void Start()
    {
        Init();
    }

    void Init()
    {
        Bones = new Transform[ChainLength + 1];
        Positions = new Vector3[ChainLength + 1];
        BonesLength = new float[ChainLength];

        StartDirectionSucc = new Vector3[ChainLength + 1];
        StartRotationBone = new Quaternion[ChainLength + 1];
        StartRotationTarget = Target.rotation;

        CompleteLength = 0.0f;

        var current = transform.GetChild(0);
        for (int i = 0; i < Bones.Length; ++i)
        {
            Bones[i] = current;
            if(current.childCount > 0)
            {
                current = current.GetChild(0);
            }
        }

        for (int i = Bones.Length - 1; i >= 0; --i)
        {
            current = Bones[i];
            StartRotationBone[i] = current.rotation;

            if(i == Bones.Length - 1)
            {
                StartDirectionSucc[i] = Target.position - current.position;
            }
            else
            {
                StartDirectionSucc[i] = Bones[i + 1].position - current.position;
                BonesLength[i] = (Bones[i + 1].position - current.position).magnitude;
                CompleteLength += BonesLength[i];
            }
        }
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if(Target == null)
        {
            return;
        }

        if(BonesLength.Length != ChainLength)
        {
            Init();
        }    

        for(int i = 0; i < Bones.Length; ++i)
        {
            Positions[i] = Bones[i].position;
        }

        var rootRot = (Bones[0].parent != null) ? Bones[0].parent.rotation : Quaternion.identity;
        var rootRotDiff = rootRot * Quaternion.Inverse(StartRotationRoot);

        if((Target.position - Bones[0].position).sqrMagnitude >= CompleteLength * CompleteLength)
        {
            var direction = (Target.position - Positions[0]).normalized;
            for (int i = 1; i < Positions.Length; ++i)
            {
                Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
            }
        }
        else
        {
            for (int iter = 0; iter < Iterations; ++iter)
            {
                for (int i = Positions.Length - 1; i > 0; --i)
                {
                    if (i == Positions.Length - 1)
                    {
                        Positions[i] = Target.position;
                    }
                    else
                    {
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                    }
                }

                for (int i = 1; i < Positions.Length; ++i)
                {
                    Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];
                }

                if((Positions[Positions.Length - 1] - Target.position).sqrMagnitude < Delta * Delta)
                {
                    break;
                }
            }
        }

        if (Pole != null)
        {
            for(int i = 1; i < Positions.Length - 1; ++i)
            {
                var plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(Pole.position);
                var projectedBone = plane.ClosestPointOnPlane(Positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);
                Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
            }
        }

        for (int i = 0; i < Positions.Length; ++i)
        {
            Bones[i].position = Positions[i];
        }

        List<Vector3> localPositions = new List<Vector3>();
        foreach(var bone in Bones)
        {
            localPositions.Add(Bones[0].transform.InverseTransformPoint(bone.position));
        }
        OnBonesUpdated?.Invoke(localPositions.ToArray());
    }

    private void OnDrawGizmos()
    {
        var current = transform.GetChild(0);
        for(int i = 0; i < ChainLength && current.childCount != 0; ++i)
        {
            var scale = Vector3.Distance(current.position, current.GetChild(0).position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position,
                Quaternion.FromToRotation(Vector3.up, current.GetChild(0).position - current.position),
                new Vector3(scale, Vector3.Distance(current.GetChild(0).position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.GetChild(0);
        }
    }
}
