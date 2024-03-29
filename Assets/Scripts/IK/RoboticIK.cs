using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoboticIK : IKInterface
{
    [Header("Parameters")]
    public float SamplingDistance;
    public float LearningRate;
    public float DistanceThreshold;
    public float Angle;

    [Header("References")]
    public GameObject Armature;
    public Transform Target;
    public Transform EndEffector;

    [Header("Debug")]
    [SerializeField]
    int bonesCount;
    [SerializeField]
    Joint[] joints;
    List<Joint> tempJoints = new List<Joint>();
    [SerializeField]
    float[] angles;
    Vector3[] axis;

    private void Start()
    {
        InitializeBonesAxis();

        // Get an array of joints instead of a list
        bonesCount = tempJoints.Count;
        joints = new Joint[bonesCount];
        tempJoints.CopyTo(joints);

        // Initialize rotations to 0
        angles = new float[bonesCount];
        for (int i = 0; i < bonesCount; i++)
            angles[i] = 0;
    }

    private void InitializeBonesAxis()
    {
        axis = new Vector3[3];
        axis[0] = new Vector3(1, 0, 0);
        axis[1] = new Vector3(0, 1, 0);
        axis[2] = new Vector3(0, 0, 1);

        Transform bone = Armature.transform;
        Joint joint;
        int axisCounter = 0;

        // Add joint script to each bone with correct axis and min/max rotations
        while (bone.childCount > 0)
        {
            bone = bone.GetChild(0);
            joint = bone.gameObject.AddComponent<Joint>();
            joint.Axis = axis[axisCounter];
            joint.MinAngle = -Angle;
            joint.MaxAngle = Angle;
            axisCounter = (axisCounter + 1) % 3;
            tempJoints.Add(joint);
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset rotations
            for (int i = 0; i < bonesCount; i++)
                angles[i] = 0;
        }
        else
        {
            // Calculate rotations with IK
            InverseKinematics(Target.position);
        }

        // Apply rotations
        for (int i = 0; i < bonesCount; i++)
            joints[i].transform.localRotation = Quaternion.AngleAxis(angles[i], joints[i].Axis);

        Vector3[] transformedPos = new Vector3[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            transformedPos[i] = Armature.transform.InverseTransformPoint(joints[i].transform.position);
        }
        OnBonesUpdated?.Invoke(transformedPos);
    }

    public void InverseKinematics(Vector3 target)
    {
        // Have we almost reached our target?
        if (DistanceFromTarget(target) < DistanceThreshold)
            return;

        for (int i = bonesCount - 1; i >= 0; i--)
        {
            // Update angles depending on learning rate, clamped between min/max angles
            float gradient = PartialGradient(target, i);
            angles[i] -= LearningRate * gradient;
            angles[i] = Mathf.Clamp(angles[i], joints[i].MinAngle, joints[i].MaxAngle);

            if (DistanceFromTarget(target) < DistanceThreshold)
                return;
        }
    }

    public float PartialGradient(Vector3 target, int i)
    {
        // Save current angle
        float angle = angles[i];

        // Calculate error function for current angle and angle + sampling distance
        float f_x = DistanceFromTarget(target);
        angles[i] += SamplingDistance;
        float f_xPlusSampligDistance = DistanceFromTarget(target);

        // Gradient : [F(x + h) - F(x)] / h
        float gradient = (f_xPlusSampligDistance - f_x) / SamplingDistance;

        // Reset angle
        angles[i] = angle;

        return gradient;
    }

    public float DistanceFromTarget(Vector3 target)
    {
        Vector3 point = ForwardKinematics();
        return Vector3.Distance(point, target);
    }

    public Vector3 ForwardKinematics()
    {
        Vector3 prevPoint = joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;

        for (int i = 1; i < bonesCount; i++)
        {
            rotation *= Quaternion.AngleAxis(angles[i - 1], joints[i - 1].Axis);
            Vector3 nextPoint = prevPoint + rotation * joints[i].StartOffset;

            prevPoint = nextPoint;
        }

        return prevPoint;
    }
}
