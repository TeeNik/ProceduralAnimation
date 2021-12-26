using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegOrientation : MonoBehaviour
{
    [SerializeField] Transform PointA;
    [SerializeField] Transform PointB;

    public bool SetForward = true;

    private Vector3 Right;
    private Vector3 Forward;

    private void Awake()
    {
        Right = transform.right;
        Forward = transform.forward;
    }

    public void UpdateOrientation()
    {
        if(PointA && PointB)
        {
            Vector3 middle = (PointB.position + PointA.position) / 2;
            transform.position = middle;

            if(SetForward)
            {
                transform.forward = (PointB.position - PointA.position).normalized;
                //transform.right = Right;
            }
            else
            {
                transform.right = (PointB.position - PointA.position).normalized;
                //transform.forward = Forward;
            }
        }
    }
}
