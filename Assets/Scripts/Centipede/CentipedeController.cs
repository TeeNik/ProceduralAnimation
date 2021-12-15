using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeController : MonoBehaviour
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

        CentipedeBodyPart[] bodyParts = GetComponentsInChildren<CentipedeBodyPart>();
        int i = 0;
        foreach (var bodyPart in bodyParts)
        {
            if(!bodyPart.IsHead)
            {
                bodyPart.InitLegSteppers(LegSteppers[i + 1], LegSteppers[i]);
                i += 2;
            }
        }
    }

    void Update()
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
            if (Physics.Raycast(Body.position, Body.up * -1, out hit, 10.0f))
            {
                bodyUp += hit.normal;
            }
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
            for(int i = 0; i < LegSteppers.Length; ++i)
            {
                LegSteppers[i].Move();
                if (LegSteppers[i].Moving)
                {
                    ++i;
                }
            }
            yield return null;
        }
    }
}
