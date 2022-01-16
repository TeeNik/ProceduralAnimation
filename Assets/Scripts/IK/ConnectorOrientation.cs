using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorOrientation : MonoBehaviour
{
    [SerializeField] public Transform PointA;
    [SerializeField] public Transform PointB;

    private void Update()
    {
        UpdateOrientation();
    }

    public void UpdateOrientation()
    {
        if(PointA && PointB)
        {
            Vector3 middle = (PointB.position + PointA.position) / 2;
            transform.position = middle;

            var forward = -(PointA.up + PointB.up) / 2;
            var up = (PointB.position - PointA.position).normalized;

            transform.rotation = Quaternion.LookRotation(forward, up);
        }
    }
}
