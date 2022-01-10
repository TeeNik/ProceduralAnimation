using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public bool IsUnderControl = false;
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

    public abstract Transform GetBodyTransform();

    protected abstract void InternalProcessInput();
}
