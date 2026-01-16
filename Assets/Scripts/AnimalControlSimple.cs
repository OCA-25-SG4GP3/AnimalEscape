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
    [SerializeField] public PlayerInputKeys inputKeys = new(); //Player 1, Player 2 
    Animator animator;
    [SerializeField] public float baseMoveSpeed = 5f;
    [SerializeField] public float moveSpeed = 5f; public void SetMoveSpeed(float _moveSpeed) { moveSpeed = _moveSpeed; }
    [SerializeField] private float moveSpeedOnFinishMult = 1.3f;
    [SerializeField] public float jumpForce = 5f;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public float groundCheckRadius = 0.1f;

    [SerializeField] float fallGravityMultiplier = 2.5f;
    [SerializeField] float lowJumpGravityMultiplier = 2.0f;
    [SerializeField, Header("Not a prefab")] private GameObject starPopEffect;

    // SE（効果音）用
    public AudioClip jumpSound;      // ジャンプ音
    public AudioClip landingSound;   // 着地音
    public AudioClip walkSound;      // 歩行音

    [SerializeField] public float soundPitch = 2.5f;
    [NonSerializedAttribute] public AudioSource audioSource; // AudioSource用の変数

    Rigidbody rb;
    Vector3 inputDir;
    JumpChecker jumpChecker;
    Camera mainCamera;

    public GameObject moveEffect;
    private bool moveEffectFlag = false;
    private int moveEffectFrame = 0;

    public GameObject smokeEffect;
    bool isAIControlled = false; public bool IsAIControlled => isAIControlled;
    public void SetAIControlled(bool value) { isAIControlled = value; }
    bool isInputLocked = false;
    public void LockInput() { isInputLocked = true; }
    public void UnlockInput() { isInputLocked = false; }
    [NonSerializedAttribute] public bool isJumping = false;
    //PlayerInput playerInput;
    OptionMenu optionMenu;

    // ジャンプホールド用の追加設定
    [SerializeField] public float holdJumpForce = 10f;      // ボタンを押している間の追加力
    public float maxJumpHoldTime = 0.2f;   // 最大で押し続けられる時間
    public float jumpHoldCounter;   // ジャンプホールドの残り時間
    public bool jumpHeld;   // 現在ジャンプボタンを押し続けているかどうか
    bool isLastGateDone = false;
    private bool allowExternalForce = false;
    private float externalForceTimer = 0f;

    /// <summary>
    /// Apply an external force to the player and temporarily disable movement override
    /// </summary>
    public void ApplyExternalForce(Vector3 force, float duration = 0.5f)
    {
        rb.AddForce(force, ForceMode.Impulse);
        allowExternalForce = true;
        externalForceTimer = duration;
    }

    private bool wasGrounded = true; // 前フレームで地面にいたか


    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        animator = GetComponentInChildren<Animator>();
        jumpChecker = GetComponentInChildren<JumpChecker>();
        rb = GetComponent<Rigidbody>();
        playerInfoSystem = GameObject.FindAnyObjectByType<PlayerInfoSystem>();
        //playerInput = GetComponent<PlayerInput>();
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


        bool isGroundedNow = jumpChecker.isGrounded;

        // 空中 → 着地した瞬間
        if (!wasGrounded && isGroundedNow)
        {
            // 着地後、少しの間ボタンを踏める
            GetComponent<PlayerInfo>().stepableTimer = 0.15f; // 好きに調整
        }

        wasGrounded = isGroundedNow;



        //DEBUG
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(smokeEffect, transform.position, transform.rotation);
        }
#endif

    }
    [SerializeField] private float jumpBufferTime = 0.15f; // store input
    [SerializeField] private float coyoteTime = 0.1f;      // allow jump after leaving ground

    // Tracks remaining time to buffer a jump input (allows jump shortly after pressing button)
    private float jumpBufferCounter = 0f;
    // Tracks remaining time for "coyote time" (allows jump shortly after leaving ground)
    private float coyoteCounter = 0f;
    // Stores whether jump button was pressed this frame (reset to false each Update)
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

        //編集:江頭
        //ポーズ時限定の処理にしました
        if (Time.timeScale != 0)
        {
            TurnToLookDir(inputDir);
        }
        UpdateAnimator();
        UpdateStunedState();
        UpdateJumpHold();
    }

    private void UpdateInputJump()
    {
        if (!isStuned && jumpPressed && !isInputLocked)
        {
            jumpBufferCounter = jumpBufferTime; // store input
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime; // countdown every frame
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

        //Stop when close
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
            //Last AI MOVE to exit point
            rb.isKinematic = true;
            SetMoveSpeed(baseMoveSpeed * moveSpeedOnFinishMult);
            SetMoveTo(transform.position + Vector3.right * 1000.0f); //Move to far away

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



            // ジャンプをした事実を記録（着地判定用）
            GetComponent<PlayerInfo>().stepableTimer = 0f;


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

        // 歩行SE処理
        if (inputDir.sqrMagnitude > 0.001f) // 動いている場合
        {
            // ループ用の歩行音を設定
            if (!audioSource.isPlaying && walkSound != null)
            {
                audioSource.clip = walkSound;
                audioSource.pitch = soundPitch;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            // 止まった場合、ループを停止
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
    void UpdateAnimator() //for now we use this to prevent warnings on monkey
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
    [SerializeField] private float stunedDuration = 1.5f; // How long the freeze lasts
    private float stunedTimer = 0f;
    private bool isStuned = false;
    private bool stunedComplete = false;

    public bool IsStunedComplete => stunedComplete; // public read-only flag
    public void SetCaughtState()
    {
        //animator.SetBool
        animator.Play("LPn01_struggle");
    }
    public void SetStunnedState(float stunedDurationOverride = -1f)
    {
        if (stunedDurationOverride > 0f)
        {
            stunedDuration = stunedDurationOverride;
        }

        stunedTimer = 0f;
        isStuned = true;
        stunedComplete = false;

        // Stop player movement completely
        moveSpeed = 0f;

        starPopEffect.SetActive(true);

        // Set animation flag if exists
        if (animator && animator.HasParameterOfType("IsStuned", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsStuned", true);

        Debug.Log("STUNED");

    }

    public void UpdateStunedState()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
#if UNITY_EDITOR
            SetStunnedState();
#endif
        }

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
        Vector3 vel = rb.linearVelocity;

        // Falling → faster fall (no hang time)
        if (vel.y < 0f)
        {
            vel.y += Physics.gravity.y * (fallGravityMultiplier - 1f) * Time.deltaTime;
        }
        // Rising but jump released → cut jump short
        else if (vel.y > 0f && !jumpHeld)
        {
            vel.y += Physics.gravity.y * (lowJumpGravityMultiplier - 1f) * Time.deltaTime;
        }

        rb.linearVelocity = vel;
    }
    /*private void UpdateJumpHold()
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
    }*/
    /// <summary>
    /// Converts raw input (h, v) to camera-relative direction on the ground plane.
    /// Handles eagle-eye camera looking down at an angle.
    /// </summary>
    private Vector3 GetCameraRelativeDirection(float horizontal, float vertical)
    {
        if (mainCamera == null)
        {
            // Fallback to world-space input if no camera found
            return new Vector3(horizontal, 0f, vertical).normalized;
        }

        // Get camera's forward and right directions
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Project camera directions onto the horizontal plane (Y = 0)
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize to ensure consistent movement speed
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction relative to camera
        Vector3 direction = (cameraForward * vertical + cameraRight * horizontal).normalized;

        return direction;
    }

    public void ExitStunedState()
    {
        if (!isStuned) return;

        isStuned = false;
        stunedTimer = 0f;

        // Restore normal move speed
        moveSpeed = baseMoveSpeed;

        // Reset animation flag
        if (animator && animator.HasParameterOfType("IsStuned", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsStuned", false);

        //Debug.Log($"{name} recovered from Frozen state!");
    }

    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return; // only trigger on performed
        optionMenu.ToggleOption();
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 navigationInput = context.ReadValue<Vector2>();
        optionMenu.NavigateMenu(navigationInput);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return; // only trigger on performed

        optionMenu.SubmitSelection();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!optionMenu.IsPaused) return;
        if (context.performed)
            optionMenu.ToggleOption();
    }
}
