using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Splines;
using UnityEngine.UIElements;


[RequireComponent(typeof(Rigidbody))]
public class AnimalControlSimple : MonoBehaviour
{
    PlayerInfoSystem playerInfoSystem;

    [SerializeField] public PlayerInputKeys inputKeys = new();

    Animator animator;
    [SerializeField] public float baseMoveSpeed = 5f;
    [SerializeField] public float moveSpeed = 5f; public void SetMoveSpeed(float _moveSpeed) { moveSpeed = _moveSpeed; }
    [SerializeField] private float moveSpeedOnFinishMult = 1.3f;
    [SerializeField] public float jumpForce = 5f;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public float groundCheckRadius = 0.1f;
    [SerializeField, Header("Not a prefab")] private GameObject starPopEffect;

    // Audio
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip walkSound;
    [NonSerialized] public AudioSource audioSource;

    Rigidbody rb;
    Vector3 inputDir;
    JumpChecker jumpChecker;
    Camera mainCamera;

    public GameObject moveEffect;
    private bool moveEffectFlag = false;
    private int moveEffectFrame = 0;

    public GameObject smokeEffect;
    bool isAIControlled = false; public bool IsAIControlled => isAIControlled;

    [NonSerialized] public bool isJumping = false;
    PlayerInput playerInput;
    OptionMenu optionMenu;

    // Jump hold settings
    [SerializeField] public float holdJumpForce = 10f;
    public float maxJumpHoldTime = 0.2f;
    public float jumpHoldCounter;
    public bool jumpHeld;

    bool isLastGateDone = false;
    private bool allowExternalForce = false;
    private float externalForceTimer = 0f;

    public void SetAIControlled(bool value) { isAIControlled = value; }
    bool isInputLocked = false;
    public void LockInput() { isInputLocked = true; }
    public void UnlockInput() { isInputLocked = false; }

    /// <summary>
    /// Apply an external force to the player and temporarily disable movement override
    /// </summary>
    public void ApplyExternalForce(Vector3 force, float duration = 0.5f)
    {
        rb.AddForce(force, ForceMode.Impulse);
        allowExternalForce = true;
        externalForceTimer = duration;
    }


    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        animator = GetComponentInChildren<Animator>();
        jumpChecker = GetComponentInChildren<JumpChecker>();
        rb = GetComponent<Rigidbody>();
        playerInfoSystem = GameObject.FindAnyObjectByType<PlayerInfoSystem>();
        optionMenu = FindAnyObjectByType<OptionMenu>();
        mainCamera = Camera.main;
    }


    void Start()
    {
        moveSpeed = baseMoveSpeed;
        audioSource = GetComponent<AudioSource>();
    }

    private Vector2 moveInput;
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isAIControlled)
        {
            jumpPressed = false;
            jumpHeld = false;
            return;
        }

        if (context.performed)
        {
            jumpPressed = true;
            jumpHeld = true;
        }

        if (context.canceled)
        {
            jumpHeld = false;
        }
    }

    Vector3 aiMoveTarget;
    public void SetMoveTo(Vector3 newMoveTarget)
    {
        aiMoveTarget = newMoveTarget;
        isAIControlled = true;

        // Clear any existing player input to prevent sliding
        moveInput = Vector2.zero;
        inputDir = Vector3.zero;

        if (animator.HasParameterOfType("IsWalking", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsWalking", true);
    }

    public void SetMoveTo(Vector3 newMoveTarget, bool isLastGate)
    {
        SetMoveTo(newMoveTarget);
        if (isLastGate) isLastGateDone = true;
    }

    GameManager gameManager;

    private void UpdateInputMove()
    {
        float h = 0f;
        float v = 0f;

        if (!isStuned && !isInputLocked)
        {
            h += moveInput.x;
            v += moveInput.y;

            if (Input.GetKey(inputKeys.forward)) v += 1f;
            if (Input.GetKey(inputKeys.backward)) v -= 1f;
            if (Input.GetKey(inputKeys.left)) h -= 1f;
            if (Input.GetKey(inputKeys.right)) h += 1f;

            if (Input.GetKeyDown(inputKeys.jump)) jumpPressed = true;

            if (Input.GetKey(inputKeys.forward) || Input.GetKey(inputKeys.backward) ||
              Input.GetKey(inputKeys.left) || Input.GetKey(inputKeys.right))
            {
                moveEffectFlag = true;
            }

            if (moveEffectFlag)
            {
                moveEffectFrame++;
                if (moveEffectFrame >= 20)
                {
                    if (isJumping == false && jumpChecker.isGrounded)
                    {
                        Instantiate(moveEffect, transform.position, transform.rotation);
                    }
                    moveEffectFrame = 0;
                    moveEffectFlag = false;
                }
            }
        }

        // Convert input to camera-relative direction
        inputDir = GetCameraRelativeDirection(h, v);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(smokeEffect, transform.position, transform.rotation);
        }
#endif
    }

    [SerializeField] private float jumpBufferTime = 0.15f;
    [SerializeField] private float coyoteTime = 0.1f;

    private float jumpBufferCounter = 0f;
    private float coyoteCounter = 0f;
    bool jumpPressed = false;

    void Update()
    {
        if (!isAIControlled)
        {
            UpdateInputMove();
            UpdateInputJump();
        }
        else
        {
            UpdateAIControlled();
        }

        coyoteCounter = jumpChecker.isGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;

        // Handle external force timer
        if (allowExternalForce)
        {
            externalForceTimer -= Time.deltaTime;
            if (externalForceTimer <= 0f)
            {
                allowExternalForce = false;
            }
        }

        TurnToLookDir(inputDir);
        UpdateAnimator();
        UpdateStunedState();
        UpdateJumpHold();
    }

    private void UpdateInputJump()
    {
        if (!isStuned && jumpPressed && !isInputLocked)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        jumpPressed = false;
    }

    private void UpdateAIControlled()
    {
        // Move straight toward target
        transform.position = Vector3.MoveTowards(transform.position, aiMoveTarget, moveSpeed * Time.deltaTime);

        Vector3 direction = (aiMoveTarget - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            // Flatten direction to horizontal plane (ignore Y axis)
            direction.y = 0f;
            direction.Normalize();

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turningSpeed * Time.deltaTime);
        }

        // Stop when close
        if (Vector3.Distance(transform.position, aiMoveTarget) < 0.1f)
        {
            isAIControlled = false;
            if (animator.HasParameterOfType("IsWalking", AnimatorControllerParameterType.Bool))
                animator.SetBool("IsWalking", false);

            if (isLastGateDone)
            {
                OnAIReachLastGate();
            }
        }
    }

    private void OnAIReachLastGate()
    {
        GameClearManager gameClearManager = FindAnyObjectByType<GameClearManager>();
        if (gameClearManager)
        {
            gameClearManager.SetClearGameByFinish();
            // Last AI MOVE to exit point
            rb.isKinematic = true;
            SetMoveSpeed(baseMoveSpeed * moveSpeedOnFinishMult);
            SetMoveTo(transform.position + Vector3.right * 1000.0f);

            // Disable camera instead of nulling Follow to prevent teleportation
            gameManager.FrontCm.enabled = false;
        }
    }

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    void Jump()
    {
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = jumpForce;
            rb.linearVelocity = vel;

            jumpHoldCounter = maxJumpHoldTime;
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;

            jumpChecker.isGrounded = false;

            isJumping = true;

            if (audioSource && jumpSound)
                audioSource.PlayOneShot(jumpSound);
        }
    }

    void Move()
    {
        if (rb.isKinematic) return;

        // Don't override velocity if external force is active
        if (allowExternalForce) return;

        Vector3 vel = rb.linearVelocity;
        vel.x = inputDir.x * moveSpeed;
        vel.z = inputDir.z * moveSpeed;
        rb.linearVelocity = vel;

        if (inputDir.sqrMagnitude > 0.001f)
        {
            if (!audioSource.isPlaying && walkSound != null)
            {
                audioSource.clip = walkSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying && walkSound != null)
            {
                audioSource.Stop();
            }
        }
    }

    float turningSpeed = 10f;

    void TurnToLookDir(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turningSpeed * Time.fixedDeltaTime);
        }
    }

    private Cooldown idle2AnimCooldown = new(5.0f);

    void UpdateAnimator()
    {
        if (!animator) return;

        bool isMoving = inputDir.sqrMagnitude > 0.001f && !isAIControlled;

        // Don't override walking animation if AI is controlling it
        if (!isAIControlled && animator.HasParameterOfType("IsWalking", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsWalking", isMoving);

        if (animator.HasParameterOfType("IsJumping", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsJumping", !jumpChecker.isGrounded);

        if (animator.HasParameterOfType("Idle2", AnimatorControllerParameterType.Trigger))
        {
            if (!idle2AnimCooldown.IsCooldown)
            {
                animator.SetTrigger("Idle2");
                idle2AnimCooldown.SetRandomizedCooldown(5.0f, 8.0f);
                idle2AnimCooldown.StartCooldown();
            }
        }
    }

    [SerializeField] private float stunedDuration = 1.5f;
    private float stunedTimer = 0f;
    private bool isStuned = false;
    private bool stunedComplete = false;

    public bool IsStunedComplete => stunedComplete;

    public void SetStunnedState(float stunedDurationOverride = -1f)
    {
        if (stunedDurationOverride > 0f)
        {
            stunedDuration = stunedDurationOverride;
        }

        stunedTimer = 0f;
        isStuned = true;
        stunedComplete = false;

        moveSpeed = 0f;

        starPopEffect.SetActive(true);

        if (animator && animator.HasParameterOfType("IsStuned", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsStuned", true);

        Debug.Log("STUNED");
    }

    public void UpdateStunedState()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.H))
        {
            SetStunnedState();
        }
#endif

        if (!isStuned) return;

        stunedTimer += Time.deltaTime;

        if (stunedTimer >= stunedDuration)
        {
            stunedComplete = true;
            ExitStunedState();
        }
    }

    private void UpdateJumpHold()
    {
        // Apply additional upward force while holding jump button during a jump
        if (jumpHeld && isJumping && jumpHoldCounter > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y += holdJumpForce * Time.deltaTime;
            rb.linearVelocity = vel;
            jumpHoldCounter -= Time.deltaTime;
        }

        // Stop jumping state when falling
        if (rb.linearVelocity.y <= 0f)
        {
            isJumping = false;
        }
    }

    /// <summary>
    /// Converts raw input (h, v) to camera-relative direction on the ground plane.
    /// </summary>
    private Vector3 GetCameraRelativeDirection(float horizontal, float vertical)
    {
        if (mainCamera == null)
        {
            return new Vector3(horizontal, 0f, vertical).normalized;
        }

        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = (cameraForward * vertical + cameraRight * horizontal).normalized;

        return direction;
    }

    public void ExitStunedState()
    {
        if (!isStuned) return;

        isStuned = false;
        stunedTimer = 0f;

        moveSpeed = baseMoveSpeed;

        if (animator && animator.HasParameterOfType("IsStuned", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsStuned", false);
    }

    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        optionMenu.ToggleOption();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigationInput = context.ReadValue<Vector2>();
        optionMenu.NavigateMenu(navigationInput);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        optionMenu.SubmitSelection();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!optionMenu.IsPaused) return;
        if (context.performed)
            optionMenu.ToggleOption();
    }
}