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

    private void OnDrawGizmos()
    {
        var nav = Agent;
        if (nav == null || nav.path == null)
            return;

        var line = this.GetComponent<LineRenderer>();
        if (line == null)
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default")) { color = Color.yellow };
            line.SetWidth(0.5f, 0.5f);
            line.SetColors(Color.yellow, Color.yellow);
        }

        var path = nav.path;

        line.SetVertexCount(path.corners.Length);

        for (int i = 0; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }
    }
}
