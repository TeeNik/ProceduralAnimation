using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegOrientation : MonoBehaviour
{
    [SerializeField] Transform PointA;
    [SerializeField] Transform PointB;

    public void UpdateOrientation()
    {
        if(PointA && PointB)
        {
            Vector3 middle = (PointB.position + PointA.position) / 2;
            transform.position = middle;
            transform.forward = (PointB.position - PointA.position).normalized;
        }
    }
}
