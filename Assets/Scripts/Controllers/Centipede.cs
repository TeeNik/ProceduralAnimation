using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Centipede : Pawn
{
    [Header("Movement")]
    public Transform Body;
    public float MovementSpeed;
    public float RotationSpeed;
    
    [Header("AI")]
    CentipedeAIController AI;

    private CentipedeBodyPart[] BodyParts;

    private void Awake()
    {
        BodyParts = GetComponentsInChildren<CentipedeBodyPart>();
    }

    void Update()
    {
        ProcessInput();
        UpdateBodyParts();
    }

    protected override void InternalProcessInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Body.position = Vector3.MoveTowards(Body.position, Body.position + Body.forward * 10, MovementSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.A))
            {
                Body.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Body.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
            }
        }
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    Body.position = Vector3.MoveTowards(Body.position, Body.position - Body.forward * 10, MovementSpeed * Time.deltaTime);
        //}

    }

    private void UpdateBodyParts()
    {
        foreach (var bodyPart in BodyParts)
        {
            bodyPart.UpdatePosition();
        }
    }

    protected override void InternalOnControlChanged(bool IsUnderPlayerControl)
    {
        if (AI)
        {
            AI.OnControlChanged(IsUnderPlayerControl);
        }
    }

    public override Transform GetBodyTransform()
    {
        return Body;
    }
}
