using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : BaseController
{
    public Transform Model;

    [Header("Movement")]
    public Transform Body;
    public float MovementSpeed;
    public float RotationSpeed;

    public float BodyHeightBase = 0.2f;
    public float BodyAdjustSpeed = 0.05f;
    public float BodyAdjustRotationSpeed = 0.05f;

    public Transform Camera;
    float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    [Header("Battery")]
    public Material NetralMaterial;
    public Material SwitchMaterial;
    public int NumberOfSlots = 6;
    public Renderer Renderer;


    void Start()
    {
        StartCoroutine(AdjustBodyTransform());
    }

    void Update()
    {
        ProcessInput();
    }

    public void SetSwitchProgress(float value)
    {
        int highlightedSlots = (int)(value * NumberOfSlots);
        Material[] materials = Renderer.materials;
        for(int i = 0; i < NumberOfSlots; ++i)
        {
            materials[NumberOfSlots - i] = i < highlightedSlots ? SwitchMaterial : NetralMaterial;
        }
        Renderer.materials = materials;
    }

    private IEnumerator AdjustBodyTransform()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(Body.position, Body.up * -1, out hit, 10.0f))
            {
                Vector3 bodyPos = hit.point + Vector3.up * BodyHeightBase;
                Body.position = Vector3.Lerp(Body.position, bodyPos, BodyAdjustSpeed);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    protected override void InternalProcessInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        float targetAngle = Body.eulerAngles.y;

        if (direction.magnitude >= 0.1f)
        {
            targetAngle = Camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Body.forward * vertical + Body.right * horizontal;
            Body.position = Vector3.MoveTowards(Body.position, Body.position + moveDir.normalized, MovementSpeed * Time.deltaTime);
        }

        Vector3 lean = new Vector3(15.0f * vertical, targetAngle, -15.0f * horizontal);
        Model.rotation = Quaternion.RotateTowards(Model.rotation, Quaternion.Euler(lean), BodyAdjustRotationSpeed * Time.deltaTime);
    }

    public override Transform GetBodyTransform()
    {
        return Body;
    }
}