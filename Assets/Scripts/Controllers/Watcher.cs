using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watcher : Pawn
{
    [Header("Movement")]
    [SerializeField] private Transform Body;
    [SerializeField] private float MovementSpeed;
    [SerializeField] private float RotationSpeed;

    [Header("Light")]
    [SerializeField] private Transform Spotlight;

    [Header("AI")]
    WatcherAIController AI;

    private void Start()
    {
        PlaySpotlightAnimation(null);
    }

    private void Update()
    {
        ProcessInput();
    }

    protected override void InternalOnControlChanged(bool IsUnderPlayerControl)
    {
        if(AI)
        {
            AI.OnControlChanged(IsUnderPlayerControl);
        }
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
    public override Transform GetBodyTransform()
    {
        return Body;
    }
    public void PlaySpotlightAnimation(Action onAnimFinished)
    {
        Spotlight.gameObject.SetActive(true);

        const float duration = 2.0f;
        const int numOfLoops = 10;

        float value = 0.0f;
        DOTween.To(() => value, x => { 
            value = x;
            Vector3 lean = new Vector3(15.0f * Mathf.Sin(x), Body.transform.eulerAngles.y, Body.transform.eulerAngles.z);
            Spotlight.rotation = Quaternion.RotateTowards(Spotlight.rotation, Quaternion.Euler(lean), 1.0f);

        }, Mathf.PI * 2, duration).SetEase(Ease.Linear).SetLoops(numOfLoops).OnComplete(() =>
        {
            Spotlight.gameObject.SetActive(false);
            onAnimFinished?.Invoke();
        });
    }

}
