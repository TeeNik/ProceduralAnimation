using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatcherAIController : AIMovement
{
    private Watcher Owner;

    private void Start()
    {
        Owner = GetComponentInParent<Watcher>();
        OnDestinationReached = OnDestinationReachedCallback;
        ChooseNextTarget();
    }

    private void OnDestinationReachedCallback()
    {
        print("OnDestinationReached");
        Owner.PlaySpotlightAnimation(ChooseNextTarget);
    }

    private void ChooseNextTarget()
    {
        Target = AIController.Instance.GetTargetForWatcher(Target);
        StartMovement(Target);
    }

    public void OnControlChanged(bool IsUnderPlayerControl)
    {
        if(IsUnderPlayerControl)
        {
            StopMovement();
        }
        else
        {
            ChooseNextTarget();
        }
    }
}
