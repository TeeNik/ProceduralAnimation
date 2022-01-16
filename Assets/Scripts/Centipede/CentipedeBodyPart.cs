using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBodyPart : MonoBehaviour
{
    public CentipedeBodyPart Leader;
    public float DistanceToLeader;
    public CentipedeBodyPart Follower;
    public bool IsHead = false;
    public float BodyHeightBase = 0.5f;

    public Transform FrontPivot;
    public Transform BackPivot;

    public float FollowSpeed = 5.0f;

    [SerializeField] LegStepper LeftLegStepper;
    [SerializeField] LegStepper RightLegStepper;

    public void Init(CentipedeBodyPart leader, CentipedeBodyPart follower, LegStepper rightLegStepper, LegStepper leftLegStepper)
    {
        Leader = leader;
        Follower = follower;
        RightLegStepper = rightLegStepper;
        LeftLegStepper = leftLegStepper;
    }

    private void Awake()
    {
        UpdateHeight();
    }

    void UpdateHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up * -1, out hit, 100.0f))
        {
            var heightDiff = Vector3.Distance(transform.position, hit.point);
            if (heightDiff > BodyHeightBase)
            {
                transform.position = hit.point + hit.normal * BodyHeightBase;
            }
        }
    }

    public void UpdatePosition()
    {
        if(!IsHead)
        {
            Vector3 dir = Leader.BackPivot.position - FrontPivot.position;
            float dist = dir.magnitude;

            if(dist > DistanceToLeader)
            {
                //float speed = Mathf.Min(5.0f, dist / DistanceToLeader);
                float speed = Mathf.Min(FollowSpeed * Time.deltaTime, dist * 0.95f); ;
                transform.position = Vector3.MoveTowards(transform.position, Leader.BackPivot.position, speed);
            }

            if (Vector3.Angle(dir, FrontPivot.forward) > 1.0f)
            {
                Quaternion toRotation = Quaternion.LookRotation(dir, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 1 * Time.deltaTime);
            }

            var forward = dir.normalized;
            var up = Vector3.Cross(forward, RightLegStepper.EndPoint - LeftLegStepper.EndPoint);
            Quaternion rot = Quaternion.LookRotation(forward, up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.05f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up * -1, out hit, 10.0f))
            {
                var heightDiff = Vector3.Distance(transform.position, hit.point);
                if (heightDiff > BodyHeightBase)
                {
                    Vector3 position = transform.position + transform.up * (BodyHeightBase - heightDiff);
                    transform.position = Vector3.Lerp(transform.position, position, 0.05f);
                }
            }
        }
    }
}
