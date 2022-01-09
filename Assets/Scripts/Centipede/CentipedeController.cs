using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeController : BaseController
{
    public LegStepperSetup LegStepperSetup;

    [Header("Movement")]
    public Transform Body;
    public float MovementSpeed;
    public float RotationSpeed;

    public float BodyHeightBase = 0.2f;
    public float BodyAdjustSpeed = 0.05f;
    public float BodyAdjustRotationSpeed = 0.05f;

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

        CentipedeBodyPartNew[] bodyParts = GetComponentsInChildren<CentipedeBodyPartNew>();
        int i = 0;
        for (int j = 0; j < bodyParts.Length; j++)
        {
            CentipedeBodyPartNew bodyPart = bodyParts[j];
            CentipedeBodyPartNew leader = j > 0 ? bodyParts[j - 1] : null;
            CentipedeBodyPartNew follower = j < bodyParts.Length - 1 ? bodyParts[j + 1] : null;
            LegStepper rightStepper = null;
            LegStepper leftStepper = null;

            if(j > 0)
            {
                rightStepper = LegSteppers[i + 1];
                leftStepper = LegSteppers[i];
                i += 2;
            }
            bodyPart.Init(leader, follower,  rightStepper, leftStepper);
        }
    }

    void Update()
    {
        ProcessInput();
    }

    protected override void InternalProcessInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Body.position = Vector3.MoveTowards(Body.position, Body.position + Body.forward * 10, MovementSpeed * Time.deltaTime);
        }
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    Body.position = Vector3.MoveTowards(Body.position, Body.position - Body.forward * 10, MovementSpeed * Time.deltaTime);
        //}
        if (Input.GetKey(KeyCode.A))
        {
            Body.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Body.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
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
        int index = 0;
        while (true)
        {
            for(int i = 0; i < LegSteppers.Length; ++i)
            {
                if (index < 3)
                {
                    LegSteppers[i].Move();
                    if (LegSteppers[i].Moving)
                    {
                        ++i;
                    }
                }
                index = i % 2 == 0 ? index + 1 : 0;
            }
            yield return null;
        }
    }
}
