using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{

    [Header("Movement")]
    public Transform Body;
    public float MovementSpeed;
    public float RotationSpeed;

    public float BodyHeightBase = 0.2f;
    public float BodyAdjustSpeed = 0.05f;
    public float BodyAdjustRotationSpeed = 0.05f;

    void Start()
    {
        StartCoroutine(AdjustBodyTransform());
    }

    void Update()
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

            RaycastHit hit;
            if (Physics.Raycast(Body.position, Body.up * -1, out hit, 10.0f))
            {
                bodyUp += hit.normal;
            }

            bodyUp.Normalize();

            Vector3 bodyPos = tipCenter + bodyUp * BodyHeightBase;
            Body.position = Vector3.Lerp(Body.position, bodyPos, BodyAdjustSpeed);

            /*
            var look = Quaternion.LookRotation(forward, up);
            look.eulerAngles = new Vector3(look.eulerAngles.x, Body.rotation.eulerAngles.y, look.eulerAngles.z);
            //if (Mathf.Abs(rot) > 0.0001f)
            //{
            //    look = Quaternion.AngleAxis(rot * 100, up) * look;
            //    rot = 0.0f;
            //}

            Body.rotation = Quaternion.Slerp(Body.rotation, look, BodyAdjustRotationSpeed);

            Debug.DrawLine(Body.position, Body.position + up * 5, Color.yellow);
            Debug.DrawLine(Body.position, Body.position + bodyUp * 10, Color.cyan);
            Debug.DrawLine(frontPoint, backPoint, Color.red);
            Debug.DrawLine(rightPoint, leftPoint, Color.blue);
            */

            yield return new WaitForFixedUpdate();
        }
    }
}
