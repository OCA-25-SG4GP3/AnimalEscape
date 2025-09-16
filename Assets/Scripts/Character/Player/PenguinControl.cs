using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PenguinControl : PlayerBase
{

    // [SerializeField] private float _minDashForce = 5f;   // ?��`?��?��?��[?��W?��ŏ�
    // [SerializeField] private float _maxDashForce = 20f;  // ?��`?��?��?��[?��W?��ő�
    [SerializeField] private float _maxChargeTime = 2f;  // ?��ő�`?��?��?��[?��W?��?��?��?��
    // [SerializeField] private float _dashCooldown = 5f; // ? cooldown in seconds
    [SerializeField] private Transform _penguinModel;  // drag child object here in Inspector
    [SerializeField] private float _slideRotationAngle = 90f; // tilt penguin
    [SerializeField] private float _rotationSpeed = 5f;       // smooth rotation speed
    [SerializeField] private GameObject _impactCollider;
    [SerializeField] private GameObject _vfxObjPrefab;
    // private bool _hitOnce = false;
    private bool _isSliding = false;

    public GameObject ImpactCollider => _impactCollider;

    [SerializeField] private TextMeshProUGUI _chargeText;

    private float _chargeTimer;
    private bool _isCharging;
    private float _cooldownTimer = 0f; // tracks remaining cooldown

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

        // ProcessSlide();
        // ProcessRopeMechanic();
    }

    void ProcessSlide()
    {
        // Countdown cooldown
        if (_cooldownTimer > 0) _cooldownTimer -= Time.deltaTime;

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
                _impactCollider.SetActive(false);

                _isSliding = false;
                _slowTime = 0f;
            }
        }
    }

    protected void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // _hitOnce = true;
            ActivateVFX();
        }
    }

    void ActivateVFX()
    {
        GameObject inst = Instantiate(_vfxObjPrefab,transform.position,transform.rotation);
        inst.SetActive(true);
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

    // public new void OnSpecialAction(InputAction.CallbackContext context)
    // {
    //     if (_cooldownTimer > 0f)
    //     {
    //         return;
    //     }


    //     if (context.started)
    //     {
    //         // ?��`?��?��?��[?��W?��J?��n
    //         _isCharging = true;
    //         _chargeTimer = 0f;
    //         Debug.Log("Penguin charging!");
    //     }

    //     if (context.canceled)
    //     {
    //         // ?��`?��?��?��[?��W?��?��?��?�� ?��?�� ?��ˌ�
    //         _isCharging = false;
    //         _isSliding = true;
    //         float chargeRatio = _chargeTimer / _maxChargeTime;
    //         float dashForce = Mathf.Lerp(_minDashForce, _maxDashForce, chargeRatio);

    //         Vector3 dashDir = new Vector3(_moveInput.x, 0, _moveInput.y);
    //         if (dashDir == Vector3.zero)
    //             dashDir = transform.forward; // ?��?��?��͂�?��?��?��?��?��?��ΑO?��?��?��?��

    //         _rigidbody.AddForce(dashDir.normalized * dashForce, ForceMode.Impulse);
    //         _cooldownTimer = _dashCooldown;
    //         Debug.Log($"Penguin dashed with force {dashForce}!");
    //         _impactCollider.SetActive(true);
    //         _hitOnce = false;
    //     }
    // }
    public bool CanDash => _cooldownTimer <= 0f;
    public float CooldownRemaining => Mathf.Max(0f, _cooldownTimer);
}
