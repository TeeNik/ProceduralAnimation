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

    private float rot = 0.0f;


    private bool disableControl = false;
    private Vector3 movePos;
    private Quaternion moveQuat;
    private Vector3 moveUp;

    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
        StartCoroutine(AdjustBodyTransform());
    }

    void Update()
    {
        const float dist = 1.0f;
        Vector3 upDebug = Body.position + Body.up * dist;
        Debug.DrawLine(Body.position, upDebug);
        Vector3 forDebug = upDebug + Body.forward * dist;
        Debug.DrawLine(upDebug, forDebug, Color.green);

        Vector3 forShortDebug = upDebug + Body.forward * dist * 0.25f;
        Vector3 downDebug = forShortDebug + Body.up * dist * -3.0f;
        Debug.DrawLine(forShortDebug, downDebug, Color.blue);
        Vector3 downBackDebug = downDebug + Body.forward * -dist;
        Debug.DrawLine(downDebug, downBackDebug, Color.red);

        if (!disableControl)
        {
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
                //Body.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
                rot = -RotationSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                //Body.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
                rot = RotationSpeed * Time.deltaTime;
            }

            RaycastHit hit;
            if (Physics.Raycast(upDebug, Body.forward, out hit, dist))
            {
                movePos = hit.point + hit.normal * 0.5f;

                Vector3 forward = Vector3.Cross(-hit.normal, Body.right);
                moveQuat = Quaternion.LookRotation(forward, hit.normal);

                moveUp = hit.normal;
                disableControl = true;
            } 
            else if (Physics.Raycast(forShortDebug, -Body.up, out hit, 3.0f * dist))
            {
                Debug.Log(hit.collider.gameObject);
            }
            else if (Physics.Raycast(downDebug, -Body.forward, out hit, dist))
            {
                movePos = hit.point + hit.normal * 0.5f;

                Vector3 forward = Vector3.Cross(-hit.normal, Body.right);
                moveQuat = Quaternion.LookRotation(forward, hit.normal);

                moveUp = hit.normal;
                disableControl = true;
            }
        }
    }

    IEnumerator LegUpdateCoroutine()
    {
        while (true)
        {
            legStepper.Move();
            yield return null;
        }
    }

    private IEnumerator AdjustBodyTransform()
    {
        while (true)
        {
            if(!disableControl)
            {
                Vector3 tipCenter = Vector3.zero;
                Vector3 bodyUp = Vector3.zero;

                RaycastHit hit;
                if (Physics.Raycast(Body.position, Body.up * -1, out hit, 10.0f))
                {
                    bodyUp += hit.normal;
                }

                bodyUp.Normalize();

                Vector3 bodyPos = hit.point + bodyUp * BodyHeightBase;
                Body.position = Vector3.Lerp(Body.position, bodyPos, BodyAdjustSpeed);

                Vector3 forward = Body.transform.forward;
                if (Mathf.Abs(rot) > 0.0001f)
                {
                    forward = Quaternion.AngleAxis(rot * 100, transform.up) * forward;
                    rot = 0.0f;
                }

                var look = Quaternion.LookRotation(forward, transform.up);
                Body.rotation = Quaternion.Slerp(Body.rotation, look, BodyAdjustRotationSpeed);
            }
            else
            {
                Body.position = Vector3.Lerp(Body.position, movePos, BodyAdjustSpeed);
                //Body.up = Vector3.Lerp(Body.up, moveUp, BodyAdjustSpeed);
                Body.rotation = Quaternion.Slerp(Body.rotation, moveQuat, BodyAdjustRotationSpeed);

                if ((Body.position - movePos).magnitude < 0.01f)
                {
                    disableControl = false;
                }
            }


            yield return new WaitForFixedUpdate();
        }
    }

}
