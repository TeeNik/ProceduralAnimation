using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeAIController : AIMovement
{
    void Start()
    {
        OnDestinationReached = OnDestinationReachedCallback;
        ChooseNextTarget();
    }

    protected override void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StopMovement();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            Target = AIController.Instance.GetTargetInFrontOfCentipede(transform.position, transform.forward);
            StartMovement(Target);
        }

        base.Update();
    }

    void OnDestinationReachedCallback()
    {
        ChooseNextTarget();
    }

    private void ChooseNextTarget()
    {
        Target = AIController.Instance.GetTargetForCentipede(Target);
        StartMovement(Target);
    }

    public void OnControlChanged(bool IsUnderPlayerControl)
    {
        if (IsUnderPlayerControl)
        {
            StopMovement();
        }
        else
        {
            ChooseNextTarget();
        }
    }
}
