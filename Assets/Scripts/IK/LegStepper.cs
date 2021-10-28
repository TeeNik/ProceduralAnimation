using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{

    [SerializeField] private Transform homeTransform;
    [SerializeField] private float wantStepAtDistance;
    [SerializeField] private float moveDuration;
    [SerializeField] float stepOvershootFraction;

    public bool Moving { get; private set; }

    private void Update()
    {
        //Move();
    }

    public void Move()
    {
        if (!Moving)
        {
            float distFromHome = Vector3.Distance(transform.position, homeTransform.position);
            if (distFromHome > wantStepAtDistance)
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

        Quaternion endRot = homeTransform.rotation;

        Vector3 towardHome = homeTransform.position - transform.position;
        float overshootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overshootVector = towardHome * overshootDistance;
        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        Vector3 endPoint = homeTransform.position + overshootVector;

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
}
