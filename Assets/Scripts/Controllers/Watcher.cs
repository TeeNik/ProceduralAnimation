using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : Pawn
{
    [Header("Movement")]
    public Transform Body;
    public float MovementSpeed;
    public float RotationSpeed;

    [Header("Light")]
    [SerializeField] private Transform Spotlight;

    private void Start()
    {
        PlaySpotlightAnimation(null);
    }

    private void Update()
    {
        ProcessInput();
    }

    protected override void InternalProcessInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Body.position = Vector3.MoveTowards(Body.position, Body.position + Body.forward * 10, MovementSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Body.position = Vector3.MoveTowards(Body.position, Body.position - Body.forward * 10, MovementSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Body.Rotate(Vector3.up * -RotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Body.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
        }
    }

    public void PlaySpotlightAnimation(Action onAnimFinished)
    {
        Spotlight.gameObject.SetActive(true);

        const float duration = 1.0f;
        const int numOfLoops = 10;

        Spotlight.rotation = Quaternion.AngleAxis(15.0f, Spotlight.right);
        Spotlight.DORotateQuaternion(Quaternion.AngleAxis(-15.0f, Spotlight.right), duration).SetEase(Ease.Linear).SetLoops(numOfLoops, LoopType.Yoyo).OnComplete(() =>
        {
            Spotlight.gameObject.SetActive(false);
            onAnimFinished?.Invoke();
        });
    }
    public override Transform GetBodyTransform()
    {
        return Body;
    }
}
