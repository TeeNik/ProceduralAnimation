using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Transform Target;

    void Update()
    {
        if(Target != null)
        {
            Agent.SetDestination(Target.position);
        }
    }
}
