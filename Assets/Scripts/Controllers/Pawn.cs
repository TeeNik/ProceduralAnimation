using System;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [SerializeField] private bool IsUnderControl = false;

    public Transform CameraPoint;
    public ParticleSystem RippleFX;

    public void ProcessInput()
    {
        if(IsUnderControl)
        {
            InternalProcessInput();
        }
    }

    public void SetRippleActive(bool isActive)
    {
        RippleFX.gameObject.SetActive(isActive);
    }

    public void OnControlChanged(bool IsUnderPlayerControl)
    {
        IsUnderControl = IsUnderPlayerControl;
        InternalOnControlChanged(IsUnderPlayerControl);
    }

    public abstract Transform GetBodyTransform();
    protected abstract void InternalProcessInput();
    protected virtual void InternalOnControlChanged(bool IsUnderPlayerControl) { }
}
