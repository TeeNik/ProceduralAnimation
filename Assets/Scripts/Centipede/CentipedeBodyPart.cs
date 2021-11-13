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

    private Vector3 PrevPosition;

    [Header("Legs")]
    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;

    public float BodyHeightBase = 0.2f;

    void Awake()
    {
        DistanceToLeader = Vector3.Distance(transform.position, Leader.transform.position);
        PrevPosition = transform.position;
    }

    private void Update()
    {
        if(IsHead)
        {
            Vector3 newPos = transform.position;
            if ((newPos - PrevPosition).sqrMagnitude > MoveDistThreshold * MoveDistThreshold)
            {
                float moveDist = (newPos - PrevPosition).magnitude;
                if (Follower != null)
                {
                    Follower.UpdateBodyPart(PrevPosition, moveDist);
                }
                PrevPosition = newPos;
            }
        }
    }

    void UpdateBodyPart(Vector3 prevPos, float moveDist)
    {
        Vector3 forward = transform.forward;

        if(Vector3.Distance(transform.position, Leader.position) > DistanceToLeader)
        {
            Vector3 moveDir = (prevPos - transform.position).normalized;
            PrevPosition = transform.position;
            transform.position += moveDir * moveDist;

            forward = Leader.transform.forward;

            //transform.rotation = Quaternion.Lerp(transform.rotation, Leader.transform.rotation, 0.05f);
            if (Follower != null)
            {
                Follower.UpdateBodyPart(PrevPosition, moveDist);
            }
        }

        var right = (frontLeftLegStepper.EndPoint + frontRightLegStepper.EndPoint) / 2;
        var up = Vector3.Cross(forward, frontRightLegStepper.EndPoint - frontLeftLegStepper.EndPoint);
        Quaternion rot = Quaternion.LookRotation(forward, up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.05f);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up * -1, out hit, 10.0f))
        {
            var heightDiff = Vector3.Distance(transform.position, hit.point);
            if(heightDiff < BodyHeightBase)
            {
                Vector3 position = transform.position + transform.up * (BodyHeightBase - heightDiff);
                transform.position = Vector3.Lerp(transform.position, position, 0.05f);
                //transform.position += transform.up * (BodyHeightBase - heightDiff);
            }
        }

    }
}
