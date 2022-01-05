using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public bool IsUnderControl = false;
    public Transform CameraPoint;

    public void ProcessInput()
    {
        if(IsUnderControl)
        {
            InternalProcessInput();
        }
    }

    protected abstract void InternalProcessInput();
}
