using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    [Header("Legs")]
    [SerializeField] LegStepper frontLeftLegStepper;
    [SerializeField] LegStepper frontRightLegStepper;
    [SerializeField] LegStepper backLeftLegStepper;
    [SerializeField] LegStepper backRightLegStepper;

    void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
    }

    IEnumerator LegUpdateCoroutine()
    {
        while (true)
        {
            do
            {
                frontLeftLegStepper.Move();
                backRightLegStepper.Move();
                yield return null;
            } while (backRightLegStepper.Moving || frontLeftLegStepper.Moving);

            do
            {
                frontRightLegStepper.Move();
                backLeftLegStepper.Move();
                yield return null;
            } while (backLeftLegStepper.Moving || frontRightLegStepper.Moving);
        }
    }
}
