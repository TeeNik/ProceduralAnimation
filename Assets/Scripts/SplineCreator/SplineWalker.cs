using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour
{
    public enum SplineWalkingMode
    {
        Once,
        Loop,
        PingPong
    }

    public BezierSpline Spline;
    public float Duration;
    public bool LookForward;
    public SplineWalkingMode Mode;

    private float progress;
    private bool goingForward = true;

    void Update()
    {
        if(goingForward)
        {
            progress += Time.deltaTime / Duration;
            if (progress > 1.0f)
            {
                if(Mode == SplineWalkingMode.Once)
                {
                    progress = 1.0f;
                }
                else if(Mode == SplineWalkingMode.Loop)
                {
                    progress -= 1.0f;
                }
                else
                {
                    progress = 2.0f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime / Duration;
            if (progress < 0.0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }

        Vector3 position = Spline.GetPoint(progress); ;
        transform.localPosition = position;
        if(LookForward)
        {
            transform.LookAt(position + Spline.GetDirection(progress));
        }
    }
}
