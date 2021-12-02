using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [Header("Legs")]
    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;
    [SerializeField] LegStepper backLeftLegStepper;
    [SerializeField] LegStepper backRightLegStepper;

    [SerializeField] Leg[] Legs;

    [Header("Movement")]
    public Transform Body;
    public float MovementSpeed;
    public float RotationSpeed;

    public float BodyHeightBase = 0.2f;
    public float BodyAdjustSpeed = 0.05f;
    public float BodyAdjustRotationSpeed = 0.05f;

    private float rot = 0.0f;

    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
        StartCoroutine(AdjustBodyTransform());
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            Body.position = Vector3.MoveTowards(Body.position, Body.position + Body.forward * 10, MovementSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Body.position = Vector3.MoveTowards(Body.position, Body.position - Body.forward * 10, MovementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Body.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
            rot = -RotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Body.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
            rot = RotationSpeed * Time.deltaTime;
        }
    }

    IEnumerator LegUpdateCoroutine()
    {
        while (true)
        {
            do
            {
                frontLeftLegStepper.Move();
                backRightLegStepper.Move();
                yield return null;
            } while (backRightLegStepper.Moving || frontLeftLegStepper.Moving);

            do
            {
                frontRightLegStepper.Move();
                backLeftLegStepper.Move();
                yield return null;
            } while (backLeftLegStepper.Moving || frontRightLegStepper.Moving);
        }
    }
    
    private IEnumerator AdjustBodyTransform()
    {
        while (true)
        {
            Vector3 tipCenter = Vector3.zero;
            Vector3 bodyUp = Vector3.zero;

            foreach (Leg leg in Legs)
            {
                tipCenter += leg.Tip.position;

                RaycastHit tipHit;
                Vector3 tipNormal = Vector3.zero;
                if (Physics.Raycast(leg.Tip.position, leg.Tip.up.normalized * -1, out tipHit, 10.0f))
                {
                    tipNormal = tipHit.normal;
                }
                bodyUp += leg.Tip.up;
            }

            tipCenter /= Legs.Length;

            RaycastHit hit;
            if (Physics.Raycast(Body.position, Body.up * -1, out hit, 10.0f))
            {
                bodyUp += hit.normal;
            }

            bodyUp.Normalize();

            Vector3 bodyPos = tipCenter + bodyUp * BodyHeightBase;
            Body.position = Vector3.Lerp(Body.position, bodyPos, BodyAdjustSpeed);

            var frontPoint = (frontLeftLegStepper.EndPoint + frontRightLegStepper.EndPoint) / 2;
            var backPoint = (backLeftLegStepper.EndPoint + backRightLegStepper.EndPoint) / 2;

            var leftPoint = (frontLeftLegStepper.EndPoint + backLeftLegStepper.EndPoint) / 2;
            var rightPoint = (frontRightLegStepper.EndPoint + backRightLegStepper.EndPoint) / 2;

            Vector3 forward = Body.transform.forward;
            forward.y = (frontPoint - backPoint).y;

            Vector3 right = (rightPoint - leftPoint).normalized;
            Vector3 up = Vector3.Cross(right, -(forward));

            if(Mathf.Abs(rot) > 0.0001f)
            {
                forward = Quaternion.AngleAxis(rot * 100, up) * forward;
                rot = 0.0f;
            }

            var look = Quaternion.LookRotation(forward, up);
            //Body.rotation = Quaternion.Slerp(Body.rotation, look, BodyAdjustRotationSpeed);

            Debug.DrawLine(frontPoint, backPoint, Color.red);
            Debug.DrawLine(rightPoint, leftPoint, Color.blue);

            yield return new WaitForFixedUpdate();
        }
    }
    
}
