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
    public Vector3 EndPoint { get; private set; }

    private void Start()
    {
        transform.position = homeTransform.position;
        EndPoint = transform.position;
    }

    private void Update()
    {
        //Move();
    }

    public void Setup(Transform homeTransform, float wantStepAtDistance, float moveDuration, float stepOvershootFraction, LayerMask groundRaycastMask, float heightOverGround)
    {
        this.homeTransform = homeTransform;
        this.wantStepAtDistance = wantStepAtDistance;
        this.moveDuration = moveDuration;
        this.stepOvershootFraction = stepOvershootFraction;
        this.groundRaycastMask = groundRaycastMask;
        this.heightOverGround = heightOverGround;
    }

    public void Move()
    {
        if (!Moving)
        {
            Vector3 pos = Vector3.ProjectOnPlane(transform.position, transform.up);
            Vector3 homePos = Vector3.ProjectOnPlane(homeTransform.position, transform.up);
            Vector3 dir = pos - homePos;

            if (dir.magnitude > wantStepAtDistance)
            {
                Vector3 endPos;
                Vector3 endNormal;
                if(GetGroundedEndPosition(out endPos, out endNormal))
                {
                    EndPoint = endPos;
                    Quaternion endRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(homeTransform.forward, endNormal), endNormal);
                    StartCoroutine(MoveToHome(endPos, endRot));
                }
            }
        }
    }

    IEnumerator MoveToHome(Vector3 endPoint, Quaternion endRot)
    {
        Moving = true;

        Quaternion startRot = transform.rotation;
        Vector3 startPoint = transform.position;

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
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;

        Vector3 raycastOrigin = homeTransform.position + overshootVector + homeTransform.up * 5f;

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
