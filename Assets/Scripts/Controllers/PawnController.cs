using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PawnController : MonoBehaviour
{
    public BaseController InitialPawn;
    public CinemachineFreeLook CinemachineCamera;

    private BaseController CurrentPawn = null;

    public BaseController TestPawn;


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
