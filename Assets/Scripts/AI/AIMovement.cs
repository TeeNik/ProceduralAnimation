using UnityEngine;
using UnityEngine.AI;
using System;

public class AIMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent Agent;

    [SerializeField] protected Transform Target;
    protected Action OnDestinationReached;
    protected bool IsMoving = false;

    private void Start()
    {
        if (Target != null)
        {
            Agent.SetDestination(Target.position);
        }
    }

    protected void StartMovement(Transform target)
    {
        if (target != null)
        {
            Target = target;
            Agent.SetDestination(Target.position);
            Agent.isStopped = false;
            IsMoving = true;
        }
    }

    protected void StopMovement()
    {
        IsMoving = false;
        Agent.isStopped = true;
    }

    protected virtual void Update()
    {
        if (!Agent.pathPending && IsMoving)
        {
            if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f)
                {
                    print("target reached");
                    StopMovement();
                    OnDestinationReached?.Invoke();
                }
            }
        }
    }
}
