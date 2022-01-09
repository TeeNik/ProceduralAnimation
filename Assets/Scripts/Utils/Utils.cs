using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool IsTargetVisible(Camera c, GameObject go)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);
        var pos = go.transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(pos) < 0)
            {
                return false;
            }
        }

        RaycastHit hit;
        if (Physics.Linecast(c.transform.position, pos, out hit))
        {
            if (hit.transform.gameObject != go)
            {
                return false;
            }
        }
        return true;
    }
}
