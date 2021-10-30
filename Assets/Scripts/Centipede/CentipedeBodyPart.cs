using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBodyPart : MonoBehaviour
{
    public Transform Leader;
    public float DistanceToLeader;
    public CentipedeBodyPart Follower;
    public bool IsHead = false;

    public float MoveDistThreshold = 0.05f;

    private Vector3 MovePosition;
    private Vector3 PrevPosition;

    void Awake()
    {
        DistanceToLeader = Vector3.Distance(transform.position, Follower.transform.position);
        PrevPosition = transform.position;
    }

    private void Update()
    {
        if(IsHead)
        {
            Vector3 pos = transform.position;
            if ((pos - PrevPosition).sqrMagnitude > MoveDistThreshold * MoveDistThreshold)
            {
                if(Follower != null)
                {
                    Follower.UpdateBodyPart(PrevPosition);
                }
            }
        }
    }

    void UpdateBodyPart(Vector3 prevPos)
    {


    }

    /*
    IEnumerator MoveToPosition()
    {
        Moving = true;

        Quaternion startRot = transform.rotation;
        Vector3 startPoint = transform.position;

        Quaternion endRot = homeTransform.rotation;

        Vector3 towardHome = homeTransform.position - transform.position;
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        Vector3 endPoint = homeTransform.position + overshootVector;

        Vector3 centerPoint = (startPoint + endPoint) / 2;
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2;

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = Easing.EaseInOutCubic(timeElapsed / moveDuration);

            transform.position = Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime);

            transform.rotation = Quaternion.Lerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        Moving = false;
    }
    */
}
