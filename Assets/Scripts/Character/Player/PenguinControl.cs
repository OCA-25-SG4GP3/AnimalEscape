using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PenguinControl : PlayerBase
{

    [SerializeField] private float _minDashForce = 5f;   // ?ｿｽ`?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽﾅ擾ｿｽ
    [SerializeField] private float _maxDashForce = 20f;  // ?ｿｽ`?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽﾅ托ｿｽ
    [SerializeField] private float _maxChargeTime = 2f;  // ?ｿｽﾅ托ｿｽ`?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ
    [SerializeField] private float _dashCooldown = 5f; // ? cooldown in seconds
    [SerializeField] private Transform _penguinModel;  // drag child object here in Inspector
    [SerializeField] private float _slideRotationAngle = 90f; // tilt penguin
    [SerializeField] private float _rotationSpeed = 5f;       // smooth rotation speed
    private bool _isSliding = false;

    [SerializeField] private TextMeshProUGUI _chargeText;

    private float _chargeTimer;
    private bool _isCharging;
    private float _cooldownTimer = 0f; // tracks remaining cooldown


    public RopeHit targetBox;
    public RopeAction2 Rope;
    public LiftAction Lift;


    private void HandleSlideRotation()
    {
        if (_penguinModel == null) return;

        Quaternion targetRotation = _isSliding
            ? Quaternion.Euler(_slideRotationAngle, 0f, 0f)   // tilt forward while sliding
            : Quaternion.identity;                            // return upright

        _penguinModel.localRotation = Quaternion.Lerp(
            _penguinModel.localRotation,
            targetRotation,
            Time.deltaTime * _rotationSpeed
        );
    }
    [SerializeField] float stopSpeed = 0.5f;
    [SerializeField] float stopHoldTime = 0.25f; // grace
    float _slowTime;
    protected override void Update()
    {
        base.Update();

        ProcessSlide();
        ProcessRopeMechanic();
    }

    void ProcessSlide()
    {
        // Countdown cooldown
        if (_cooldownTimer > 0)
            _cooldownTimer -= Time.deltaTime;
                // 長押し中にチャージ時間を加算
        if (_isCharging)
        {
            _chargeTimer += Time.deltaTime;
            if (_chargeTimer > _maxChargeTime)
                _chargeTimer = _maxChargeTime;
        }

        HandleSlideRotation();
        HandleChargeUI();

        float speedSqr = _rigidbody.linearVelocity.sqrMagnitude;
        float stopSpeedSqr = stopSpeed * stopSpeed;

        if (_isSliding)
        {
            if (speedSqr < stopSpeedSqr) _slowTime += Time.deltaTime;
            else _slowTime = 0f;

            if (_slowTime >= stopHoldTime)
            {
                _isSliding = false;
                _slowTime = 0f;
            }
        }

    }

    private void HandleChargeUI()
    {
        if (_chargeText == null) return;

        if (_cooldownTimer > 0f)
        {
            _chargeText.text = $"Cooldown: {_cooldownTimer:0.0}s";
        }
        else if (_isCharging)
        {
            _chargeText.text = $"Charge: {Mathf.Min(_chargeTimer, _maxChargeTime):0.0}s";
        }
        else
        {
            _chargeText.text = "";
        }


    }

    void ProcessRopeMechanic()
    {
        if (!Lift || !Rope) return; //?ｿｽ?ｿｽ?ｿｽﾝのプ?ｿｽ?ｿｽ?ｿｽg?ｿｽ^?ｿｽC?ｿｽv?ｿｽﾍ、?ｿｽ?ｿｽ?ｿｽﾎらくLift?ｿｽ?ｿｽ?ｿｽp?ｿｽ?ｿｽ?ｿｽﾜゑｿｽ?ｿｽ?ｿｽ
        //?A?N?V?????{?^?????????????
        //if (_inputSystem.Penguin.<Keyboard>/ e.triggered )
        //{
        //???[?v???????????ｿｽE????ｿｽO??
        if (targetBox.playerInside)
        {
            Lift.lift_flag = true;
            Rope.rope_flag = true;
            //Debug.Log("E?L?[?????F????v???C???[??????????O??o?????");
        }
        else
        {
            Lift.lift_flag = false;
            Rope.rope_flag = false;
            // Debug.Log("E?L?[?????F????N????????");
        }
        //}
    }
    public new void OnSpecialAction(InputAction.CallbackContext context)
    {
        if (_cooldownTimer > 0f) return;


        if (context.started)
        {
            // ?ｿｽ`?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽJ?ｿｽn
            _isCharging = true;
            _chargeTimer = 0f;
            Debug.Log("Penguin charging!");
        }

        if (context.canceled)
        {
            // ?ｿｽ`?ｿｽ?ｿｽ?ｿｽ[?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ ?ｿｽ?ｿｽ ?ｿｽﾋ鯉ｿｽ
            _isCharging = false;
            _isSliding = true;
            float chargeRatio = _chargeTimer / _maxChargeTime;
            float dashForce = Mathf.Lerp(_minDashForce, _maxDashForce, chargeRatio);

            Vector3 dashDir = new Vector3(_moveInput.x, 0, _moveInput.y);
            if (dashDir == Vector3.zero)
                dashDir = transform.forward; // ?ｿｽ?ｿｽ?ｿｽﾍゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾎ前?ｿｽ?ｿｽ?ｿｽ?ｿｽ

            _rigidbody.AddForce(dashDir.normalized * dashForce, ForceMode.Impulse);
            _cooldownTimer = _dashCooldown;
            Debug.Log($"Penguin dashed with force {dashForce}!");
        }
    }
    public bool CanDash => _cooldownTimer <= 0f;
    public float CooldownRemaining => Mathf.Max(0f, _cooldownTimer);
}
