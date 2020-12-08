using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoController : MonoBehaviour
{
    
    [SerializeField] private Transform target;
    [SerializeField] private Transform headBone;
    [SerializeField] private float headTurnSpeed;
    [SerializeField] private float headMaxTurnAngle;

    [SerializeField] Transform leftEyeBone;
    [SerializeField] Transform rightEyeBone;

    [SerializeField] float eyeTrackingSpeed;
    [SerializeField] float leftEyeMaxYRotation;
    [SerializeField] float leftEyeMinYRotation;
    [SerializeField] float rightEyeMaxYRotation;
    [SerializeField] float rightEyeMinYRotation;

    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;
    [SerializeField] LegStepper backLeftLegStepper;
    [SerializeField] LegStepper backRightLegStepper;

    [SerializeField] float turnSpeed;
    [SerializeField] float moveSpeed;

    [SerializeField] float turnAcceleration;
    [SerializeField] float moveAcceleration;

    [SerializeField] float minDistToTarget;
    [SerializeField] float maxDistToTarget;

    [SerializeField] float maxAngleToTarget;

    private Vector3 currentVelocity;
    private float currentAngularVelocity;


    void Awake()
    {
        StartCoroutine(LegTracking());
    }

    void LateUpdate()
    {
        RootMotionUpdate();
        HeadTracking();
        //EyesTracking();
    }

    void HeadTracking()
    {
        Quaternion currentLocalRot = headBone.localRotation;
        headBone.localRotation = Quaternion.identity;

        Vector3 targetWorldLookDir = target.position - headBone.position;
        Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

        targetLocalLookDir = Vector3.RotateTowards(Vector3.forward, targetLocalLookDir, Mathf.Deg2Rad * headMaxTurnAngle, 0);
        Quaternion targetLocalRotation = Quaternion.LookRotation(targetLocalLookDir, Vector3.up);
        headBone.localRotation = Quaternion.Slerp(currentLocalRot, targetLocalRotation, 1 - Mathf.Exp(-headTurnSpeed * Time.deltaTime));
    }

    void EyesTracking()
    {
        Quaternion targetEyeRotation = Quaternion.LookRotation(target.position - headBone.position, Vector3.up);
        leftEyeBone.rotation = Quaternion.Slerp(leftEyeBone.rotation, targetEyeRotation,
            1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime));
        rightEyeBone.rotation = Quaternion.Slerp(rightEyeBone.rotation, targetEyeRotation,
            1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime));
    }

    void RootMotionUpdate()
    {
        Vector3 towardTarget = target.position - transform.position;
        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);

        float angleToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, transform.up);
        float targetAngularVelocity = 0;

        if (Math.Abs(angleToTarget) > maxAngleToTarget)
        {
            targetAngularVelocity = angleToTarget > 0 ? turnSpeed : -turnSpeed;
        }

        currentAngularVelocity = Mathf.Lerp(currentAngularVelocity, targetAngularVelocity,
            1 - Mathf.Exp(-turnAcceleration * Time.deltaTime));

        transform.Rotate(0, Time.deltaTime * currentAngularVelocity, 0, Space.World);

        Vector3 targetVelocity = Vector3.zero;
        if (Mathf.Abs(angleToTarget) < 90)
        {
            float disToTarget = Vector3.Distance(transform.position, target.position);
            if (disToTarget > maxDistToTarget)
            {
                targetVelocity = moveSpeed * towardTargetProjected.normalized;
            }
            else if (disToTarget < minDistToTarget)
            {
                targetVelocity = moveSpeed * -towardTargetProjected.normalized;
            }
        }

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity,
            1 - Mathf.Exp(-moveAcceleration * Time.deltaTime));

        transform.position += currentVelocity * Time.deltaTime;
    }

    IEnumerator LegTracking()
    {
        while (true)
        {
            do
            {
                frontLeftLegStepper.Move();
                backRightLegStepper.Move();
                yield return null;
            }
            while (frontLeftLegStepper.Moving || backRightLegStepper.Moving);

            do
            {
                frontRightLegStepper.Move();
                backLeftLegStepper.Move();
                yield return null;
            }
            while (frontRightLegStepper.Moving || backLeftLegStepper.Moving);
        }
    }
}
