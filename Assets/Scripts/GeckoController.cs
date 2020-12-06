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

    void LateUpdate()
    {
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
}
