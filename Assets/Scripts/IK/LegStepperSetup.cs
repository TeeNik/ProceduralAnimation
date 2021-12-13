using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepperSetup : MonoBehaviour
{
    [SerializeField] private float wantStepAtDistance;
    [SerializeField] private float moveDuration;
    [SerializeField] float stepOvershootFraction;

    [SerializeField] LayerMask groundRaycastMask = ~0;
    [SerializeField] float heightOverGround = 0.0f;

    public LegStepper[] CreateLegSteppers()
    {
        List<LegStepper> steppers = new List<LegStepper>();
        GameObject legStepperGO = new GameObject();
        Leg[] legs = GetComponentsInChildren<Leg>();
        foreach (var leg in legs)
        {
            Transform home = leg.transform.parent.Find("Home");

            GameObject legStepperObject = Instantiate(legStepperGO, transform);
            LegStepper legStepper = legStepperObject.AddComponent<LegStepper>();
            legStepper.Setup(home, wantStepAtDistance, moveDuration, stepOvershootFraction, groundRaycastMask, heightOverGround);

            FabricIK ik = leg.transform.GetComponent<FabricIK>();
            ik.Target = legStepperObject.transform;

            steppers.Add(legStepper);
        }
        return steppers.ToArray();
    }
}
