using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeCamera : MonoBehaviour
{
    [SerializeField] private Transform FollowTarget;
    [SerializeField] private float HeightOverGround = 5.0f;
    [SerializeField] private float Speed = 1.0f;

    private Vector3 Offset;

    private void Awake()
    {
        Offset = FollowTarget.InverseTransformPoint(transform.position);
    }

    void Update()
    {
        Vector3 targetPos = FollowTarget.TransformPoint(Offset);
        RaycastHit hit;
        if (Physics.Raycast(targetPos, -Vector3.up, out hit))
        {
            targetPos = hit.point + Vector3.up * HeightOverGround;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Speed * Time.deltaTime);
    }
}
