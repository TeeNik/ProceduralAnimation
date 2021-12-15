using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBodyPartNew : MonoBehaviour
{
    public CentipedeBodyPartNew Leader;
    public float DistanceToLeader;
    public CentipedeBodyPartNew Follower;
    public bool IsHead = false;

    public Transform FrontPivot;
    public Transform BackPivot;


    private void Update()
    {
        if(!IsHead)
        {
            Vector3 dir = Leader.BackPivot.position - FrontPivot.position;
            float dist = dir.magnitude;

            if(dist > DistanceToLeader)
            {
                transform.position = Vector3.MoveTowards(transform.position, Leader.BackPivot.position, 0.5f * Time.deltaTime);
            }
            if(Vector3.Angle(dir, transform.forward) > 5.0f)
            {
                Quaternion toRotation = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 10 * Time.deltaTime);
            }
        }
    }

    void UpdateBodyPart(Transform backPivot)
    {

    }
}
