using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class PawnController : MonoBehaviour
{
    public BaseController InitialPawn;
    public CinemachineFreeLook CinemachineCamera;
    public Camera Camera;
    public float Radius = 10.0f;

    private BaseController CurrentPawn = null;

    [SerializeField] private BaseController[] Pawns;


    private BaseController SwitchTarget = null;
    private bool IsSwitching = false;
    private float CurrentSwitchTime = 0.0f; 
    private const float SwitchTime = 3.0f;

    public Slider SwitchSlider;
    public BotController Bot;


    private void Start()
    {
        TakeControl(InitialPawn);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(CurrentPawn is BotController && SwitchTarget != null)
            {
                IsSwitching = true;
            }
        }
        else if(Input.GetMouseButtonUp(1))
        {
            if (!(CurrentPawn is BotController))
            {
                TakeControl(InitialPawn);
            }
        }

        HighlightSwitchTarget();

        if(IsSwitching)
        {
            CurrentSwitchTime += Time.deltaTime;

            if(CurrentSwitchTime >= SwitchTime)
            {
                CurrentSwitchTime = 0.0f;
                SwitchPawns();
                IsSwitching = false;
            }

            Bot.SetSwitchProgress(CurrentSwitchTime / SwitchTime);
        }

    }

    private void SwitchPawns()
    {
        CurrentPawn.SetRippleActive(false);
        SwitchTarget.SetRippleActive(false);
        TakeControl(SwitchTarget);
        SwitchTarget = null;
    }

    private void HighlightSwitchTarget()
    {
        if (CurrentPawn is BotController)
        {
            BaseController closest = FindClosestVisiblePawnsInRadius(CurrentPawn, Radius);
            print(closest);
            if (closest != null)
            {
                if (SwitchTarget == null)
                {
                    CurrentPawn.SetRippleActive(true);
                }
                else if (closest != SwitchTarget)
                {
                    SwitchTarget.SetRippleActive(false);
                }
                SwitchTarget = closest;
                SwitchTarget.SetRippleActive(true);
            }
            else if (closest == null)
            {
                if (SwitchTarget != null)
                {
                    SwitchTarget.SetRippleActive(false);
                }
                SwitchTarget = null;
                CurrentPawn.SetRippleActive(false);
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
                float dist = Vector3.SqrMagnitude(player.GetBodyTransform().position - pawn.GetBodyTransform().position);
                if (dist < radius * radius)
                {
                    if (Utils.IsTargetVisible(Camera, pawn.GetBodyTransform().gameObject))
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
