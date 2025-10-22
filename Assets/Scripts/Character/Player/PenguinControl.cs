using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PenguinControl : PlayerBase
{
    [SerializeField] private Transform _penguinModel;
    [SerializeField] private float _slideRotationAngle = 90f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private GameObject _impactCollider;
    [SerializeField] private GameObject _vfxObjPrefab;
    [SerializeField] private TextMeshProUGUI _chargeText;

    [SerializeField] private float _maxChargeTime = 2f;
    private float _chargeTimer;
    private bool _isCharging;
    private float _cooldownTimer;

    private bool _isSliding = false;
    [SerializeField] float stopSpeed = 0.5f;
    [SerializeField] float stopHoldTime = 0.25f;
    private float _slowTime;

    public GameObject ImpactCollider => _impactCollider;

    protected override void Update()
    {
        base.Update();
        ProcessSlide(); //does not work for now because Update() switchstate is disabled. it needs to be disabled due to conflict with rotating platform
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        HandleWalking();
    }

    private void HandleWalking()
    {
        Vector3 moveDir = new Vector3(_moveInput.x, 0f, _moveInput.y);

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Vector3 targetPos = _rigidbody.position + moveDir.normalized * _walkSpeed * Time.fixedDeltaTime;
            _rigidbody.MovePosition(targetPos);

            // Rotate model toward movement direction
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            _penguinModel.rotation = Quaternion.Slerp(_penguinModel.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void ProcessSlide()
    {
        if (_cooldownTimer > 0f) _cooldownTimer -= Time.deltaTime;

        if (_isCharging)
        {
            _chargeTimer += Time.deltaTime;
            if (_chargeTimer > _maxChargeTime) _chargeTimer = _maxChargeTime;
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

    private void HandleSlideRotation()
    {
        if (_penguinModel == null) return;

        Quaternion targetRotation = _isSliding
            ? Quaternion.Euler(_slideRotationAngle, 0f, 0f)
            : Quaternion.identity;

        _penguinModel.localRotation = Quaternion.Lerp(
            _penguinModel.localRotation,
            targetRotation,
            Time.deltaTime * _rotationSpeed
        );
    }

    private void HandleChargeUI()
    {
        if (_chargeText == null) return;

        if (_cooldownTimer > 0f)
            _chargeText.text = $"Cooldown: {_cooldownTimer:0.0}s";
        else if (_isCharging)
            _chargeText.text = $"Charge: {Mathf.Min(_chargeTimer, _maxChargeTime):0.0}s";
        else
            _chargeText.text = "";
    }

    protected void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ActivateVFX();
        }
    }

    private void ActivateVFX()
    {
        GameObject inst = Instantiate(_vfxObjPrefab, transform.position, transform.rotation);
        inst.SetActive(true);
    }

    public bool CanDash => _cooldownTimer <= 0f;
    public float CooldownRemaining => Mathf.Max(0f, _cooldownTimer);
}
