using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] LegStepper legStepper;
    [SerializeField] Leg[] Legs;

    [Header("Movement")]
    public Transform Body;
    public float MovementSpeed;
    public float RotationSpeed;

    public float BodyHeightBase = 0.2f;
    public float BodyAdjustSpeed = 0.05f;
    public float BodyAdjustRotationSpeed = 0.05f;

    private Vector3 MoveTarget = Vector3.zero;
    private Quaternion RotationTarget = Quaternion.identity;

    void Awake()
    {
        //StartCoroutine(LegUpdateCoroutine());
        StartCoroutine(AdjustBodyTransform());
    }

    void Update()
    {
        CheckObstacle();

        if (Input.GetKey(KeyCode.W))
        {
            //Body.position = Vector3.MoveTowards(Body.position, Body.position + Body.forward * 10, MovementSpeed * Time.deltaTime);
            Body.position = Vector3.MoveTowards(Body.position, MoveTarget, MovementSpeed * Time.deltaTime);

            Vector3 lTargetDir = MoveTarget - Body.position;
            //transform.rotation = Quaternion.RotateTowards(Body.rotation, Quaternion.LookRotation(lTargetDir), RotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(Body.rotation, RotationTarget, RotationSpeed * Time.deltaTime);

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

    //IEnumerator LegUpdateCoroutine()
    //{
    //    while (true)
    //    {
    //        legStepper.Move();
    //        yield return null;
    //    }
    //}

    private IEnumerator AdjustBodyTransform()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    private Vector3 HitPos =Vector3.zero;


    public float RaycastHeight = 1.0f;
    public float RaycastForwardDist = 0.5f;

    private void CheckObstacle()
    {
        const float angle = 270.0f;
        const float step = 15.0f;

        for(int i = 0; i <= angle / step; ++i)
        {
            Vector3 up = Quaternion.AngleAxis(step * i, Body.right) * Body.up;
            Vector3 upPos = Body.position + up * RaycastHeight;
            Vector3 forward = Quaternion.AngleAxis(step * i, Body.right) * Body.forward;
            Vector3 forPos = upPos + forward * RaycastForwardDist;

            Debug.DrawLine(Body.position, upPos);
            Debug.DrawLine(upPos, forPos);


            RaycastHit hit;
            LayerMask groundRaycastMask = ~0;
            if (Physics.Linecast(upPos, forPos, out hit, groundRaycastMask))
            {
                HitPos = hit.point;
                MoveTarget = hit.point + hit.normal * BodyHeightBase;

                Vector3 newForward = Vector3.Cross(Body.right, hit.normal);
                DrawCoordSystem(MoveTarget, newForward, hit.normal, Body.right);
                RotationTarget = Quaternion.LookRotation(newForward, hit.normal);

                break;
            }
        }

    }

    void DrawCoordSystem(Vector3 origin, Vector3 forward, Vector3 up, Vector3 right)
    {
        const float length = 1.0f;
        Debug.DrawLine(origin, origin + forward * length, Color.blue);
        Debug.DrawLine(origin, origin + up * length, Color.green);
        Debug.DrawLine(origin, origin + right * length, Color.red);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(HitPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(MoveTarget, 0.1f);
    }

}
