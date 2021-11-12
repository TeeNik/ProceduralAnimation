using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeController : MonoBehaviour
{
    [SerializeField] Leg[] Legs;

    [Header("Legs")]
    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;

    [Header("Movement")]
    public Transform Body;
    public float MovementSpeed;
    public float RotationSpeed;

    public float BodyHeightBase = 0.2f;
    public float BodyAdjustSpeed = 0.05f;
    public float BodyAdjustRotationSpeed = 0.05f;

    void Awake()
    {
        //StartCoroutine(LegUpdateCoroutine());
        StartCoroutine(AdjustBodyTransform());
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
            Vector3 tipCenter = Vector3.zero;
            Vector3 bodyUp = Vector3.zero;

            //Collect leg information to calculate body transform
            foreach (Leg leg in Legs)
            {
                tipCenter += leg.Tip.position;
            
                RaycastHit tipHit;
                Vector3 tipNormal = Vector3.zero;
                if (Physics.Raycast(leg.Tip.position, leg.Tip.up.normalized * -1, out tipHit, 10.0f))
                {
                    tipNormal = tipHit.normal;
                }
                //bodyUp += leg.Tip.up /*+ tipNormal*/;
            }
            
            tipCenter /= Legs.Length;

            RaycastHit hit;
            if (Physics.Raycast(Body.position, Body.up * -1, out hit, 10.0f))
            {
                bodyUp += hit.normal;
            }

            bodyUp.Normalize();

            // Interpolate postition from old to new
            Vector3 bodyPos =/* tipCenter + */ hit.point + bodyUp * BodyHeightBase;
            Body.position = Vector3.Lerp(Body.position, bodyPos, BodyAdjustSpeed);

            // Calculate new body axis
            Vector3 bodyRight = Vector3.Cross(bodyUp, Body.forward);
            Vector3 bodyForward = Vector3.Cross(bodyRight, bodyUp);

            // Interpolate rotation from old to new
            Quaternion bodyRotation = Quaternion.LookRotation(bodyForward, bodyUp);

            Body.rotation = Quaternion.Slerp(Body.rotation, bodyRotation, BodyAdjustRotationSpeed);


            //var front = (frontLeftLegStepper.EndPoint + frontRightLegStepper.EndPoint) / 2;
            //var forwardRot = Quaternion.LookRotation(front - back);
            //forwardRot.y = Body.rotation.y;
            //
            //var left = (frontLeftLegStepper.EndPoint + backLeftLegStepper.EndPoint) / 2;
            //var right = (frontRightLegStepper.EndPoint + backRightLegStepper.EndPoint) / 2;
            //var rightRot = Quaternion.LookRotation(right - left);
            //
            //Vector3 up = Vector3.Cross(right - left, -(front - back));
            //var look = Quaternion.LookRotation(front - back, up);
            ////look.y = Body.rotation.y;
            //
            //Body.rotation = Quaternion.Slerp(Body.rotation, look, BodyAdjustRotationSpeed);

            yield return new WaitForFixedUpdate();
        }
    }
}
