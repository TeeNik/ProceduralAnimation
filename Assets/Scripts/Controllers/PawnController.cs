using UnityEngine;
using Cinemachine;

public class PawnController : MonoBehaviour
{
    [SerializeField] private Bot Bot;
    [SerializeField] private CinemachineFreeLook CinemachineCamera;
    [SerializeField] private Camera Camera;
    [SerializeField] private float Radius = 10.0f;
    [SerializeField] private Pawn[] Pawns;

    private Pawn CurrentPawn = null;
    private Pawn SwitchTarget = null;
    private bool IsSwitching = false;
    private float CurrentSwitchTime = 0.0f; 
    private const float SwitchTime = 3.0f;


    private void Start()
    {
        TakeControl(Bot);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(CurrentPawn is Bot && SwitchTarget != null)
            {
                IsSwitching = true;
            }
        }
        else if(Input.GetMouseButtonUp(1))
        {
            if (!(CurrentPawn is Bot))
            {
                TakeControl(Bot);
            }
        }

        HighlightSwitchTarget();

        if(IsSwitching)
        {
            CurrentSwitchTime += Time.deltaTime;
            Bot.SetSwitchProgress(CurrentSwitchTime / SwitchTime);

            if (CurrentSwitchTime >= SwitchTime)
            {
                CurrentSwitchTime = 0.0f;
                SwitchPawns();
                IsSwitching = false;
            }
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
        if (CurrentPawn is Bot)
        {
            Pawn closest = FindClosestVisiblePawnsInRadius(CurrentPawn, Radius);
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

    private Pawn FindClosestVisiblePawnsInRadius(Pawn player, float radius)
    {
        float minDist = float.MaxValue;
        Pawn target = null;
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

    private void TakeControl(Pawn controller)
    {
        if(CurrentPawn)
        {
            CurrentPawn.OnControlChanged(false);
        }
        CurrentPawn = controller;
        CurrentPawn.OnControlChanged(true);

        CinemachineCamera.Follow = CurrentPawn.CameraPoint;
        CinemachineCamera.LookAt = CurrentPawn.CameraPoint;
    }
}
