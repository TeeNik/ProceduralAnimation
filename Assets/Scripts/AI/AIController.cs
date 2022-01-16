using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AIController : MonoBehaviour
{
    [SerializeField] private List<Transform> PointsOfInterest;
    [SerializeField] private List<Transform> CentipedePathPoints;

    public static AIController Instance => _instance;
    private static AIController _instance;

    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Assert.IsTrue(false, "There is more than one instance of AIController!");
        }
    }

    public Transform GetTargetForWatcher(Transform prevTarget)
    {
        while(true)
        {
            Transform target = PointsOfInterest[Random.Range(0, PointsOfInterest.Count)];
            if(target != prevTarget)
            {
                return target;
            }
        }
    }

    public Transform GetTargetForCentipede(Transform prevTarget)
    {
        //while (true)
        //{
        //    Transform target = CentipedePathPoints[Random.Range(0, CentipedePathPoints.Count)];
        //    if (target != prevTarget)
        //    {
        //        return target;
        //    }
        //}

        int index = CentipedePathPoints.IndexOf(prevTarget);
        ++index;
        if(index >= CentipedePathPoints.Count)
        {
            index = 0;
        }
        return CentipedePathPoints[index];
    }

    public Transform GetTargetInFrontOfCentipede(Vector3 pos, Vector3 lookDir)
    {
        float minAngle = float.MaxValue;
        Transform target = null;
        foreach(var point in CentipedePathPoints)
        {
            Vector3 dir = point.position - pos;
            float angle = Vector3.Angle(lookDir, dir);
            if(angle < minAngle)
            {
                minAngle = angle;
                target = point;
            }
        }
        return target;
    }
}
