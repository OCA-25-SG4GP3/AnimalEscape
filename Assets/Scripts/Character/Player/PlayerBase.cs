using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEditor.VersionControl.Asset;

public class PlayerBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float _walkSpeed;
    [SerializeField] protected float _rotateSpeed;
    [SerializeField] protected float _jumpPower;
    [SerializeField] protected float _groundCheckDistance = 0.01f;
    [SerializeField] private float _groundCheckOffset = 0.1f;

    public float MoveSpeed { get; set; }
    public float WalkSpeed => _walkSpeed;
    public float RotateSpeed => _rotateSpeed;
    public bool IsJumping { get; set; }
    public float JumpPower => _jumpPower;
    public bool IsSpecialAction { get; set; }

    [Header("States")]
    [SerializeField] protected PlayerStateBaseSO _currentState;
    [SerializeField] protected PlayerStateJumpSO _jumpStateSO;
    [SerializeField] protected PlayerStateWalkSO _walkStateSO;
    [SerializeField] protected PlayerStateIdleSO _idleStateSO;
    [SerializeField] protected PlayerStateSpecialActionSO _specialActionStateSO;

    protected PlayerStateJumpSO _jumpStateInstance;
    protected PlayerStateWalkSO _walkStateInstance;
    protected PlayerStateIdleSO _idleStateInstance;
    protected PlayerStateSpecialActionSO _specialActionStateInstance;

    protected InputSystem _inputSystem;
    protected Vector2 _moveInput;
    public Vector2 MoveInput => _moveInput;

    protected Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    public bool IsGrounded => CheckIsGround();

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        MoveSpeed = _walkSpeed;
        _inputSystem = new InputSystem();

        _jumpStateInstance = Instantiate(_jumpStateSO);
        _walkStateInstance = Instantiate(_walkStateSO);
        _idleStateInstance = Instantiate(_idleStateSO);
        _specialActionStateInstance = Instantiate(_specialActionStateSO);

        _currentState = _idleStateInstance;
    }

    protected virtual void OnEnable()
    {
        _inputSystem.Enable();
    }

    protected virtual void OnDisable()
    {
        _inputSystem.Disable();
    }

    protected virtual void Start()
    {
        _jumpStateInstance.SetPlayer(this);
        _walkStateInstance.SetPlayer(this);
        _idleStateInstance.SetPlayer(this);
        _specialActionStateInstance.SetPlayer(this);

        _currentState.EnterState();
    }

    protected virtual void Update()
    {
        SwitchState();
        _currentState.UpdateState();
    }

    protected virtual void FixedUpdate()
    {
        _currentState.FixedUpdateState();
    }

    protected virtual void LateUpdate()
    {
        _currentState.LateUpdateState();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Jump();
            IsJumping = true;
        }
        if (context.canceled)
        {
            IsJumping = false;
        }
    }

    public void OnSpecialAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //SpecialAction();
            IsSpecialAction = true;
            Debug.Log("Special action started!");
        }
        if (context.canceled)
        {
            IsSpecialAction = false;
        }
    }

    public void OnControlChanged(PlayerInput input)
    {
        Debug.Log($"Controls changed for player {input.playerIndex}. Active control: {input.currentControlScheme}");
    }

    public void OnDeviceLost(PlayerInput input)
    {
        Debug.Log($"Device lost for player {input.playerIndex}");
    }

    public void OnDeviceRegained(PlayerInput input)
    {
        Debug.Log($"Device regained for player {input.playerIndex}");
    }

    protected virtual void SwitchState()
    {
        StateBaseSO previousState = _currentState;

        if (!_currentState.CanBeInterrupted)
        {
            return;
        }

        if (CheckIsGround())
        {
            SwitchGroundState();
        }
        else
        {
            SwitchAirState();
        }

        if (previousState != _currentState || previousState.IsComplete)
        {
            //Debug.Log($"Switching state from {previousState.name} to {_currentState.name}");
            previousState.ExitState();
            _currentState.Initialize();
            _currentState.EnterState();
        }
    }

    protected virtual bool CheckIsGround()
    {
        Vector3 origin = transform.position + Vector3.up * _groundCheckOffset;
        return Physics.Raycast(origin, Vector3.down, _groundCheckDistance);
    }

    public virtual void Move(Vector3 direction)
    {
        direction.Normalize();
        _rigidbody.MovePosition(transform.position + MoveSpeed * Time.fixedDeltaTime * direction);
    }

    public virtual void Rotate(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.deltaTime);
    }

    protected virtual void Jump()
    {
        _rigidbody.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
    }

    protected virtual void SpecialAction()
    {
        Debug.Log("Special action triggered!");
    }

    protected virtual void SwitchGroundState()
    {
       
        if (IsSpecialAction)
        {
            _currentState = _specialActionStateInstance;
            IsSpecialAction = false;
        }
        else if (IsJumping)
        {
            _currentState = _jumpStateInstance;
        }
        else if (_moveInput != Vector2.zero)
        {
            _currentState = _walkStateInstance;
        }
        else
        {
            _currentState = _idleStateInstance;
        }
    }

    protected virtual void SwitchAirState()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Key"))
        {
            var key = collision.collider.gameObject.GetComponent<Key>();
            key.Use();
            Destroy(key.gameObject);
        }
    }
}
