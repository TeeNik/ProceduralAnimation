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
        if(Vector3.Distance(transform.position, Leader.position) > DistanceToLeader)
        {
            Vector3 moveDir = (prevPos - transform.position).normalized;
            PrevPosition = transform.position;
            transform.position += moveDir * moveDist;
            transform.rotation = Quaternion.Lerp(transform.rotation, Leader.transform.rotation, 0.05f);
            if (Follower != null)
            {
                Follower.UpdateBodyPart(PrevPosition, moveDist);
            }
        }
    }
}
