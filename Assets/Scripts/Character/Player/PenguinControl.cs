using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PenguinControl : PlayerBase
{
    [Header("Slide Parameter")]
    private bool _isSliding = false;
    [SerializeField] private float _slideSpeed = 8f;
    [SerializeField] private float _slideDuration = 0.5f;
    private float _slideTimer = 0f;
    private Vector3 _slideDirection;
    private Quaternion _originalRotation;
    private Quaternion _slideTiltRotation;

    public RopeHit targetBox;
    public RopeAction2 Rope;
    public LiftAction Lift;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isSliding)
        {
            float x = _inputSystem.Penguin.Move.ReadValue<Vector2>().x;
            float z = _inputSystem.Penguin.Move.ReadValue<Vector2>().y;

            _movement = new Vector3(x, 0, z).normalized;

            if (_inputSystem.Penguin.Jump.triggered)
            {
                StartSlide();
            }

            if (_movement != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(_movement);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed);
            }
        }
        else
        {
            _slideTimer -= Time.deltaTime;
            if (_slideTimer <= 0f)
            {
                EndSlide();
            }
        }

        //アクションボタンを押している間
        //if (_inputSystem.Penguin.<Keyboard>/ e.triggered )
        //{
            //ロープを引いてもう片方を上げる
            if (targetBox.playerInside)
            {
                Lift.lift_flag = true;
                Rope.rope_flag = true;
                //Debug.Log("Eキー押下：中にプレイヤーがいるのでログを出します");
            }
            else
            {
                Lift.lift_flag = false;
                Rope.rope_flag = false;
            // Debug.Log("Eキー押下：中に誰もいません");
        }
        //}
    }

    private void StartSlide()
    {
        _isSliding = true;
        _slideTimer = _slideDuration;
        //_slideDirection = transform.forward;
        _movement = transform.forward;
        _moveSpeed = _slideSpeed;
        _originalRotation = transform.rotation;
        _slideTiltRotation = Quaternion.Euler(90.0f, transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = _slideTiltRotation;
    }

    private void EndSlide()
    {
        _isSliding = false;
        _moveSpeed = _walkSpeed;
        transform.rotation = _originalRotation;
    }
}
