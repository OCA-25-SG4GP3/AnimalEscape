using System;
using UnityEngine;

public class OpeningCameraAction : MonoBehaviour
{
    [SerializeField] private Camera _scenarioCamera;
    [SerializeField] private VoidEventSO _onEnterGameEvent;
    [SerializeField] private VoidEventSO _onFinishIntroEvent;

    private bool _isStarted = false;

    [SerializeField] private Vector3 _offsetPosition;

    public float cameraSpeed = 0.01f;
    public float waitTimer = 0.0f;
    private float waitTimerCount = 0.0f;
    private int targetNumber = 0;

    [SerializeField] GameObject pointA;
    [SerializeField] GameObject pointB;
    [SerializeField] GameObject pointC;
    [SerializeField] GameObject player;

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

    // Update is called once per frame
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

    void SetCameraTarget()
    {
        switch (targetNumber)
        {
            case 0:
                {
                    Vector3 current = transform.position;
                    Vector3 target = new(
                        pointA.transform.position.x + _offsetPosition.x,
                        pointA.transform.position.y +_offsetPosition.y,
                        pointA.transform.position.z + _offsetPosition.z
                    );
                    float step = 3.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

                    if (transform.position == target)
                    {
                        waitTimerCount += 0.1f;
                        if (waitTimerCount >= waitTimer)
                        {
                            targetNumber++;
                            waitTimerCount = 0.0f;
                        }

                    }
                }
                break;

            case 1:
                {
                    Vector3 current = transform.position;
                    Vector3 target = new(
                        pointB.transform.position.x + _offsetPosition.x,
                        pointB.transform.position.y +_offsetPosition.y,
                        pointB.transform.position.z + _offsetPosition.z
                    );
                    float step = 3.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

                    if (transform.position == target)
                    {
                        waitTimerCount += 0.1f;
                        if (waitTimerCount >= waitTimer)
                        {
                            targetNumber++;
                            waitTimerCount = 0.0f;
                        }
                    }
                }
                break;
            case 2:
                {
                    Vector3 current = transform.position;
                    Vector3 target = new(
                        pointC.transform.position.x + _offsetPosition.x,
                        pointC.transform.position.y +_offsetPosition.y,
                        pointC.transform.position.z + _offsetPosition.z
                    );
                    float step = 3.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

                    if (transform.position == target)
                    {
                        waitTimerCount += 0.1f;
                        if (waitTimerCount >= waitTimer)
                        {
                            targetNumber++;
                            waitTimerCount = 0.0f;
                        }
                    }
                }
                break;
            case 3:
                {
                    player.transform.position = Camera.main.transform.position;
                    Vector3 current = transform.position;
                    Vector3 target = player.transform.position;
                    float step = 3.0f * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(current, target, cameraSpeed * 0.005f);

                    if (transform.position == target)
                    {
                        _scenarioCamera.enabled = false;
                        Camera.main.enabled = true;
                        if (!_isFinished)
                        {
                            _onFinishIntroEvent.InvokeEvent();
                            _isFinished = true;
                        }
                    }
                }
                break;
        }
    }
}
