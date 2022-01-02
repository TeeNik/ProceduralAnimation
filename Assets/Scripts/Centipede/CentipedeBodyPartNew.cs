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


    private void Update()
    {
        if(!IsHead)
        {
            Vector3 dir = Leader.BackPivot.position - FrontPivot.position;
            float dist = dir.magnitude;

            if(dist > DistanceToLeader)
            {
                //float speed = Mathf.Min(5.0f, dist / DistanceToLeader);
                float speed = Mathf.Min(FollowSpeed * Time.deltaTime, dist * 0.95f); ;
                Vector3 move = Vector3.MoveTowards(transform.position, Leader.BackPivot.position, speed);
                if((move - transform.position).magnitude > dist)
                {
                    Debug.Log("bad");
                }
                //move = Vector3.ClampMagnitude(move, DistanceToLeader);
                transform.position = move;

                if (Vector3.Angle(dir, FrontPivot.forward) > 15.0f)
                {
                    float z = transform.rotation.eulerAngles.z;

                    Quaternion toRotation = Quaternion.LookRotation(dir, transform.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 1 * Time.deltaTime);
                    //float z = transform.rotation.eulerAngles.z;
                    //Vector3 newDirection = Vector3.RotateTowards(transform.forward, dir.normalized, 1 * Time.deltaTime, 0.0f);
                    //Quaternion look = Quaternion.LookRotation(newDirection);
                    //look.eulerAngles = new Vector3(look.eulerAngles.x, look.eulerAngles.y, z);
                    //transform.rotation = look;
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
                if (heightDiff > BodyHeightBase)
                {
                    Vector3 position = transform.position + transform.up * (BodyHeightBase - heightDiff);
                    transform.position = Vector3.Lerp(transform.position, position, 0.05f);
                }
            }
        }
    }
}
