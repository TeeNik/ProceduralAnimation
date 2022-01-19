using UnityEngine;
using UnityEngine.AI;
using System;

public class AIMovement : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent Agent;
    [SerializeField] protected Transform Body;

    [SerializeField] protected Transform Target;
    protected Action OnDestinationReached;
    protected bool IsMoving = false;

    private void Awake()
    {
        Agent.updatePosition = false;
    }

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
            transform.position = Body.position;
            transform.rotation = Body.rotation;
            Agent.enabled = true;

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
        Agent.enabled = false;
    }

    protected virtual void Update()
    {
        if (!Agent.pathPending && IsMoving)
        {
            Vector3 nextPos = Agent.nextPosition;
            transform.position = nextPos;
            Body.position = nextPos;
            Body.rotation = Quaternion.Euler(new Vector3(Body.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Body.rotation.eulerAngles.z));

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
