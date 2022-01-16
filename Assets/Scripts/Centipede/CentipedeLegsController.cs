using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeLegsController : MonoBehaviour
{
    [SerializeField] private LegStepperSetup LegStepperSetup;

    [Header("Movement")]
    [SerializeField] private Transform Body;
    [SerializeField] private float BodyHeightBase = 0.2f;
    [SerializeField] private float BodyAdjustSpeed = 0.05f;
    [SerializeField] private float BodyAdjustRotationSpeed = 0.05f;

    private LegStepper[] LegSteppers;

    void Awake()
    {
        Init();

        StartCoroutine(LegUpdateCoroutine());
        StartCoroutine(AdjustBodyTransform());
    }

    private void Init()
    {
        LegSteppers = LegStepperSetup.CreateLegSteppers();

        CentipedeBodyPart[] bodyParts = GetComponentsInChildren<CentipedeBodyPart>();
        int i = 0;
        for (int j = 0; j < bodyParts.Length; j++)
        {
            CentipedeBodyPart bodyPart = bodyParts[j];
            CentipedeBodyPart leader = j > 0 ? bodyParts[j - 1] : null;
            CentipedeBodyPart follower = j < bodyParts.Length - 1 ? bodyParts[j + 1] : null;
            LegStepper rightStepper = null;
            LegStepper leftStepper = null;

            if(j > 0)
            {
                rightStepper = LegSteppers[i + 1];
                leftStepper = LegSteppers[i];
                i += 2;

                ConnectorOrientation connector = leader.GetComponentInChildren<ConnectorOrientation>();
                connector.PointA = leader.BackPivot;
                connector.PointB = bodyPart.FrontPivot;
            }

            bodyPart.Init(leader, follower,  rightStepper, leftStepper);
        }
    }


    private IEnumerator AdjustBodyTransform()
    {
        while (true)
        {
            Vector3 bodyUp = Vector3.zero;
            RaycastHit hit;

            Vector3 rayDir = Body.up * -1 + 2.0f * Body.forward;
            Debug.DrawLine(Body.position, Body.position + 3.0f * rayDir, Color.green);
            if(Physics.Raycast(Body.position, rayDir.normalized, out hit, 10.0f))
            {
                bodyUp += hit.normal;
            }
            if (Physics.Raycast(Body.position, Body.up * -1, out hit, 10.0f))
            {
                bodyUp += hit.normal;
            }
            bodyUp /= 2;
            bodyUp.Normalize();

            Vector3 bodyPos = hit.point + bodyUp * BodyHeightBase;
            Body.position = Vector3.Lerp(Body.position, bodyPos, BodyAdjustSpeed);

            Vector3 bodyRight = Vector3.Cross(bodyUp, Body.forward);
            Vector3 bodyForward = Vector3.Cross(bodyRight, bodyUp);

            Quaternion bodyRotation = Quaternion.LookRotation(bodyForward, bodyUp);
            Body.rotation = Quaternion.Slerp(Body.rotation, bodyRotation, BodyAdjustRotationSpeed);

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator LegUpdateCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < LegSteppers.Length; ++i)
            {
                if (!IsLegsAheadMoving(4, i))
                {
                    LegSteppers[i].Move();
                }
            }

            yield return null;
        }

    }

    bool IsLegsAheadMoving(int numOfLegs, int index)
    {
        if(index < numOfLegs)
        {
            return false;
        }
        for(int i = 1; i <= numOfLegs; ++i)
        {
            if(!LegSteppers[index - i].Moving)
            {
                return false;
            }
        }
        return true;
    }
}
