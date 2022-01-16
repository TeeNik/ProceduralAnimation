using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bot : Pawn
{
    [Header("Movement")]
    [SerializeField] private Transform Body;
    [SerializeField] private Transform Model;
    [SerializeField] private float MovementSpeed;

    [SerializeField] private float BodyHeightBase = 0.2f;
    [SerializeField] private float BodyAdjustSpeed = 0.05f;
    [SerializeField] private float BodyAdjustRotationSpeed = 0.05f;

    [SerializeField] private Transform Camera;
    [SerializeField] private float TurnSmoothTime = 0.1f;

    [Header("Visuals")]
    [SerializeField] private Material NetralMaterial;
    [SerializeField] private Material SwitchMaterial;
    [SerializeField] private int NumberOfSlots = 6;
    [SerializeField] private Renderer BatteryRenderer;
    [SerializeField] private Renderer EyeRenderer;

    private Tween IdleTween;
    private float TurnSmoothVelocity;

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
        UpdateHighlightMaterials(value);
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
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, TurnSmoothTime);
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

    protected override void InternalOnControlChanged(bool IsUnderPlayerControl)
    {
        if(!IsUnderPlayerControl)
        {
            Model.eulerAngles = Body.eulerAngles;

            float value = 0.0f;
            float height = BodyHeightBase;
            IdleTween = DOTween.To(() => value, x =>
            {
                value = x;
                BodyHeightBase = height + 0.5f * Mathf.Sin(x);
            }, Mathf.PI * 2, 2.0f).SetEase(Ease.Linear).SetLoops(-1).OnKill(() => BodyHeightBase = height);
        }
        else
        {
            IdleTween?.Kill();
            UpdateHighlightMaterials(0.0f);
        }
    }

    private void UpdateHighlightMaterials(float value)
    {
        int highlightedSlots = (int)(value * NumberOfSlots);
        Material[] materials = BatteryRenderer.materials;
        for (int i = 0; i < NumberOfSlots; ++i)
        {
            materials[NumberOfSlots - i] = i < highlightedSlots ? SwitchMaterial : NetralMaterial;
        }
        BatteryRenderer.materials = materials;

        if(highlightedSlots == 0 || highlightedSlots == NumberOfSlots)
        {
            materials = EyeRenderer.materials;
            materials[1] = highlightedSlots == 0 ? NetralMaterial : SwitchMaterial;
            EyeRenderer.materials = materials;
        }
    }
}