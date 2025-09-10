using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PenguinControl : PlayerBase
{
 
    [SerializeField] private float _minDashForce = 5f;   // チャージ最小
    [SerializeField] private float _maxDashForce = 20f;  // チャージ最大
    [SerializeField] private float _maxChargeTime = 2f;  // 最大チャージ時間
    [SerializeField] private float _dashCooldown = 5f; // ⏳ cooldown in seconds

    /*  [SerializeField] private Transform _penguinModel;  // drag child object here in Inspector
      [SerializeField] private float _slideRotationAngle = 90f; // tilt penguin
      [SerializeField] private float _rotationSpeed = 5f;       // smooth rotation speed
      private bool _isSliding = false;*/

    [SerializeField] private TextMeshProUGUI _chargeText;

    private float _chargeTimer;
    private bool _isCharging;
    private float _cooldownTimer = 0f; // tracks remaining cooldown

   
/*    private void HandleSlideRotation()
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
    }*/
    protected override void Update()
    {
        base.Update();
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
        /*  if (_isSliding && _rigidbody.linearVelocity.magnitude < 0.5f)
          {
              _isSliding = false;
          }
          HandleSlideRotation();*/
        HandleChargeUI();
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
    public new void OnSpecialAction(InputAction.CallbackContext context)
    {
        if (_cooldownTimer > 0f) return;

     
        if (context.started)
        {
            // チャージ開始
            _isCharging = true;
            _chargeTimer = 0f;
            Debug.Log("Penguin charging!");
        }

        if (context.canceled)
        {
            // チャージ完了 → 突撃
            _isCharging = false;
            float chargeRatio = _chargeTimer / _maxChargeTime;
            float dashForce = Mathf.Lerp(_minDashForce, _maxDashForce, chargeRatio);

            Vector3 dashDir = new Vector3(_moveInput.x, 0, _moveInput.y);
            if (dashDir == Vector3.zero)
                dashDir = transform.forward; // 入力が無ければ前方向

            _rigidbody.AddForce(dashDir.normalized * dashForce, ForceMode.Impulse);
            _cooldownTimer = _dashCooldown;
            Debug.Log($"Penguin dashed with force {dashForce}!");
        }
    }
    public bool CanDash => _cooldownTimer <= 0f;
    public float CooldownRemaining => Mathf.Max(0f, _cooldownTimer);
}
