using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{

    [SerializeField] private Transform homeTransform;
    [SerializeField] private float wantStepAtDistance;
    [SerializeField] private float moveDuration;
    [SerializeField] float stepOvershootFraction;

    [SerializeField] LayerMask groundRaycastMask = ~0;
    [SerializeField] float heightOverGround = 0.0f;


    public bool Moving { get; private set; }

    public Vector3 EndPoint;

    private void Awake()
    {
        transform.position = homeTransform.position;
        EndPoint = transform.position;
    }

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        if (!Moving)
        {
            Vector3 dir = transform.position - homeTransform.position;
            dir.y = 0.0f;
            float distFromHome = Vector3.Distance(transform.position, homeTransform.position);
            if (dir.magnitude > wantStepAtDistance)
            {
                StartCoroutine(MoveToHome());
            }
        }
    }

    IEnumerator MoveToHome()
    {
        Moving = true;

        Quaternion startRot = transform.rotation;
        Vector3 startPoint = transform.position;

        Vector3 endPoint;
        Vector3 endNormal;
        bool result = GetGroundedEndPosition(out endPoint, out endNormal);
        EndPoint = endPoint;

        Quaternion endRot = Quaternion.LookRotation(
            Vector3.ProjectOnPlane(homeTransform.forward, endNormal), endNormal);

        Vector3 centerPoint = (startPoint + endPoint) / 2;
        centerPoint += homeTransform.up * Vector3.Distance(startPoint, endPoint) / 2;

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = Easing.EaseInOutCubic(timeElapsed / moveDuration);

            transform.position = Vector3.Lerp(
                Vector3.Lerp(startPoint, centerPoint, normalizedTime),
                Vector3.Lerp(centerPoint, endPoint, normalizedTime),
                normalizedTime);

            transform.rotation = Quaternion.Lerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        Moving = false;
    }

    bool GetGroundedEndPosition(out Vector3 position, out Vector3 normal)
    {
        Vector3 towardHome = (homeTransform.position - transform.position).normalized;

        // Limit overshoot to a fraction of the step distance.
        // This prevents infinite step cycles when a foot end point ends up outside its home position radius bounds.
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;

        Vector3 raycastOrigin = homeTransform.position + overshootVector + homeTransform.up * 2f;

        if (Physics.Raycast(
            raycastOrigin,
            -homeTransform.up,
            out RaycastHit hit,
            Mathf.Infinity,
            groundRaycastMask
        ))
        {
            position = hit.point + homeTransform.up * heightOverGround;
            normal = hit.normal;
            return true;
        }
        position = Vector3.zero;
        normal = Vector3.zero;
        return false;
    }
}
