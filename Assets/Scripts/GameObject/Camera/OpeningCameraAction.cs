using System;
using UnityEngine;

public class OpeningCameraAction : MonoBehaviour
{
    [SerializeField] private Camera _scenarioCamera;
    [SerializeField] private VoidEventSO _onEnterGameEvent;
    [SerializeField] private VoidEventSO _onFinishIntroEvent;

    [SerializeField] private Vector3 _offsetPosition;

    private bool _isStarted = false;

    [SerializeField] public float cameraSpeed;
    [SerializeField] public float waitTimer;

    public GameObject pointA;
    public GameObject pointB;
    public GameObject pointC;
    public GameObject player;

    private float waitTimerCount;
    private int targetNumber = 0;
    private bool _isFinished = false;

    private void Awake()
    {
        _scenarioCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        _onEnterGameEvent.OnEventInvoked += StartIntro;
    }

    private void OnDisable()
    {
        _onEnterGameEvent.OnEventInvoked -= StartIntro;
    }

    void Update()
    {
        if (_isStarted)
        {
            SetCameraTarget();
        }        
    }

    private void StartIntro()
    {
        _isStarted = true;
        _scenarioCamera.enabled = true;
    }

    private void SetCameraTarget()
    {     
        switch (targetNumber)
        {
            case 0:
                MoveToTarget(pointA);
                break;

            case 1:
                MoveToTarget(pointB);
                break;

            case 2:
                MoveToTarget(pointC);
                break;

            case 3:               
                MoveToTarget(player);
                break;
        }
    }

    private void MoveToTarget(GameObject targetPoint)
    {
        Vector3 current = transform.position;
        Vector3 target = targetPoint.transform.position + _offsetPosition;

        transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

        //目標に到達したら
        if (transform.position == target )
        {
            waitTimerCount += Time.deltaTime;
            //止まるカウントを過ぎたら
            if (waitTimerCount >= waitTimer)
            {
                targetNumber++;
                waitTimerCount = 0.0f;
            }
        }
    }
}