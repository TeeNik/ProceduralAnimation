using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    public List<RobotJoint> Joints;
    public float SamplingDistance;
    public float LearningRate;

    public float DistanceFromTarget(Vector3 target, float[] angles)
    {
        Vector3 point = ForwardKinematics(angles);
        return Vector3.Distance(point, target);
    }

    public Vector3 ForwardKinematics(float[] angles)
    {
        Vector3 point = Joints[0].transform.position;
        Quaternion rotation = Quaternion.identity;

        for (int i = 1; i < Joints.Count; ++i)
        {
            rotation *= Quaternion.AngleAxis(angles[i-1], Joints[i-1].Axis);
            point = point + rotation * Joints[i].StartOffset;
        }

        return point;
    }

    public void InverseKinematics(Vector3 target, float[] angles)
    {
        if(DistanceFromTarget(target, angles) < DistanceFromTarget())
        for (int i = 0; i < Joints.Count; ++i)
        {
            float gradient = PartialGradient(target, angles, i);
            angles[i] -= LearningRate * gradient;
        }
    }

    public float PartialGradient(Vector3 target, float[] angles, int i)
    {
        float angle = angles[i];
        float f_x = DistanceFromTarget(target, angles);
        angles[i] += SamplingDistance;
        float f_x_plus_d = DistanceFromTarget(target, angles);
        float gradient = (f_x_plus_d - f_x) / SamplingDistance;
        angles[i] = angle;
        return gradient;
    }

}
