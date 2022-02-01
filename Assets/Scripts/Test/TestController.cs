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


    private int RayIndex = 0;


    void Awake()
    {
        //StartCoroutine(LegUpdateCoroutine());
        StartCoroutine(AdjustBodyTransform());
    }

    void Update()
    {
        //const float dist = 1.0f;
        //Vector3 upDebug = Body.position + Body.up * dist;
        //Debug.DrawLine(Body.position, upDebug);
        //Vector3 forDebug = upDebug + Body.forward * dist;
        //Debug.DrawLine(upDebug, forDebug, Color.green);
        //
        //Vector3 forShortDebug = upDebug + Body.forward * dist * 0.25f;
        //Vector3 downDebug = forShortDebug + Body.up * dist * -3.0f;
        //Debug.DrawLine(forShortDebug, downDebug, Color.blue);
        //Vector3 downBackDebug = downDebug + Body.forward * -dist;
        //Debug.DrawLine(downDebug, downBackDebug, Color.red);

        CheckObstacle();
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ++RayIndex;
        }

        if (Input.GetKey(KeyCode.W))
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

    private void CheckObstacle()
    {
        //Vector3 up = Body.up;
        //up = Quaternion.AngleAxis(15.0f * RayIndex, Body.right) * up;
        //Vector3 upPos = Body.position + up * 5.0f;
        //Vector3 forward = Quaternion.AngleAxis(15.0f * RayIndex, Body.right) * Body.forward;
        //Vector3 forPos = upPos + forward * 2.0f;
        //
        //Debug.DrawLine(Body.position, upPos);
        //Debug.DrawLine(upPos, forPos);



        const float height = 2.0f;
        const float forwardDist = 1.0f;
        const float angle = 270.0f;
        const float step = 15.0f;

        for(int i = 0; i <= angle / step; ++i)
        {
            Vector3 up = Quaternion.AngleAxis(step * i, Body.right) * Body.up;
            Vector3 upPos = Body.position + up * height;
            Vector3 forward = Quaternion.AngleAxis(step * i, Body.right) * Body.forward;
            Vector3 forPos = upPos + forward * forwardDist;

            Debug.DrawLine(Body.position, upPos);
            Debug.DrawLine(upPos, forPos);


            RaycastHit hit;
            LayerMask groundRaycastMask = ~0;
            if (Physics.Linecast(upPos, forPos, out hit, groundRaycastMask))
            {
                HitPos = hit.point;
                break;
            }
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(HitPos, 0.2f);
    }

}
