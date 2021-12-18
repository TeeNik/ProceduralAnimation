using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBodyPartNew : MonoBehaviour
{
    public CentipedeBodyPartNew Leader;
    public float DistanceToLeader;
    public CentipedeBodyPartNew Follower;
    public bool IsHead = false;
    public float BodyHeightBase = 0.5f;

    public Transform FrontPivot;
    public Transform BackPivot;

    public float FollowSpeed = 5.0f;

    [SerializeField] LegStepper LeftLegStepper;
    [SerializeField] LegStepper RightLegStepper;

    public void Init(CentipedeBodyPartNew leader, CentipedeBodyPartNew follower, LegStepper rightLegStepper, LegStepper leftLegStepper)
    {
        Leader = leader;
        Follower = follower;
        RightLegStepper = rightLegStepper;
        LeftLegStepper = leftLegStepper;
    }

    private void Update()
    {
        if(!IsHead)
        {
            Vector3 dir = Leader.BackPivot.position - FrontPivot.position;
            float dist = dir.magnitude;

            if(dist > DistanceToLeader)
            {
                float speed = Mathf.Min(5.0f, dist / DistanceToLeader);
                transform.position = Vector3.MoveTowards(transform.position, Leader.BackPivot.position, FollowSpeed * Time.deltaTime);

                if (Vector3.Angle(dir, FrontPivot.forward) > 5.0f)
                {
                    Quaternion toRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 10 * Time.deltaTime);
                }
            }


            var forward = dir.normalized;
            var up = Vector3.Cross(forward, RightLegStepper.EndPoint - LeftLegStepper.EndPoint);
            Quaternion rot = Quaternion.LookRotation(forward, up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.05f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.up * -1, out hit, 10.0f))
            {
                var heightDiff = Vector3.Distance(transform.position, hit.point);
                if (heightDiff < BodyHeightBase)
                {
                    Vector3 position = transform.position + transform.up * (BodyHeightBase - heightDiff);
                    transform.position = Vector3.Lerp(transform.position, position, 0.05f);
                }
            }
        }
    }
}
