using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PawnController : MonoBehaviour
{
    public BaseController InitialPawn;
    public CinemachineFreeLook CinemachineCamera;
    public Camera Camera;
    public float Radius = 10.0f;

    private BaseController CurrentPawn = null;

    public BaseController TestPawn;

    [SerializeField] private BaseController[] Pawns;


    private BaseController RadioTarget;


    private void Start()
    {
        TakeControl(InitialPawn);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            TakeControl(TestPawn);
        }


        if(Input.GetMouseButtonDown(0))
        {
            if(CurrentPawn is BotController && RadioTarget != null)
            {
                TakeControl(RadioTarget);
                RadioTarget = null;
            }
        }
        else if(Input.GetMouseButtonUp(1))
        {
            if (!(CurrentPawn is BotController))
            {
                TakeControl(InitialPawn);
            }
        }

        if(CurrentPawn is BotController)
        {
            BaseController closest = FindClosestVisiblePawnsInRadius(CurrentPawn, Radius);
            print(closest);
            if(closest != null)
            {
                if(RadioTarget == null)
                {
                    CurrentPawn.Ripple_FX.gameObject.SetActive(true);
                }
                else if(closest != RadioTarget)
                {
                    RadioTarget.Ripple_FX.gameObject.SetActive(false);
                }
                RadioTarget = closest;
                RadioTarget.Ripple_FX.gameObject.SetActive(true);
            }
            else if (closest == null)
            {
                if (RadioTarget != null)
                {
                    RadioTarget.Ripple_FX.gameObject.SetActive(false);
                }
                RadioTarget = null;
                CurrentPawn.Ripple_FX.gameObject.SetActive(false);
            }
        }
    }

    private BaseController FindClosestVisiblePawnsInRadius(BaseController player, float radius)
    {
        float minDist = float.MaxValue;
        BaseController target = null;
        foreach (var pawn in Pawns)
        {
            if(pawn != player)
            {
                float dist = Vector3.SqrMagnitude(player.transform.position - pawn.transform.position);
                if (dist < radius * radius)
                {
                    if (Utils.IsTargetVisible(Camera, pawn.gameObject))
                    {
                        if(dist < minDist)
                        {
                            minDist = dist;
                            target = pawn;
                        }
                    }
                }
            }
        }
        return target;
    }

    private void TakeControl(BaseController controller)
    {
        if(CurrentPawn)
        {
            CurrentPawn.IsUnderControl = false;
        }
        controller.IsUnderControl = true;
        CurrentPawn = controller;

        CinemachineCamera.Follow = CurrentPawn.CameraPoint;
        CinemachineCamera.LookAt = CurrentPawn.CameraPoint;
    }
}
