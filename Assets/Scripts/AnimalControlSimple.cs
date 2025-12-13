using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Splines;
using UnityEngine.UIElements;

[System.Serializable]
public struct PlayerInputKeys
{
    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode specialAction; //Slide / Throw
    public KeyCode horizontalAxis; //Slide / Throw
    public KeyCode verticalAxis; //Slide / Throw
}
[RequireComponent(typeof(Rigidbody))]
public class AnimalControlSimple : MonoBehaviour
{
    PlayerInfoSystem playerInfoSystem;
    [SerializeField] public PlayerInputKeys inputKeys = new(); //Player 1, Player 2 別々に決める
    Animator animator;
    [SerializeField] public float baseMoveSpeed = 5f;
    [SerializeField] public float moveSpeed = 5f; public void SetMoveSpeed(float _moveSpeed) { moveSpeed = _moveSpeed; }
    [SerializeField] public float jumpForce = 5f;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public float groundCheckRadius = 0.1f;
    [SerializeField, Header("Not a prefab")] private GameObject starPopEffect;

    //着地のSEを入れる
    public AudioClip jumpSound;      // ジャンプ音のファイル
    public AudioClip landingSound; // 着地音
    [NonSerializedAttribute] public AudioSource audioSource; // AudioSourceを使うための変数

    Rigidbody rb;
    Vector3 inputDir;
    JumpChecker jumpChecker;

    public GameObject jumpEffect;
    public GameObject slideEffect;
    public GameObject smokeEffect;
    bool isAIControlled = false; public bool IsAIControlled => isAIControlled;
    [NonSerializedAttribute] public bool isJumping = false;  // ジャンプ中かどうかを追跡するフラグ
    PlayerInput playerInput;
    OptionMenu optionMenu;

    //ジャンプホールド用追加
    [SerializeField] public float holdJumpForce = 10f;      // 押し続けている間の追加力
    public float maxJumpHoldTime = 0.2f;   // 最大で押せる時間
    public float jumpHoldCounter;   // 押し続けられる残り時間
    public bool jumpHeld;   // 現在ジャンプボタンが押されているかどうか

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        jumpChecker = GetComponentInChildren<JumpChecker>();
        rb = GetComponent<Rigidbody>();
        playerInfoSystem = GameObject.FindAnyObjectByType<PlayerInfoSystem>();
        playerInput = GetComponent<PlayerInput>();
        optionMenu = FindAnyObjectByType<OptionMenu>();
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
    //public void OnJump(InputAction.CallbackContext context)
    //{
    //    if (!context.performed) return; // only trigger on performed

    //    jumpPressed = true;
    //}
    //押す、離すを検知
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;  // ジャンプ入力があったことを記録（ジャンプバッファ用）
            jumpHeld = true;    // ボタンを押している状態にする
        }

        if (context.canceled)
        {
            jumpHeld = false;   // 押し続け状態を解除
        }
    }



    Vector3 aiMoveTarget;
    public void SetMoveTo(Vector3 newMoveTarget)
    {
        aiMoveTarget = newMoveTarget;
        isAIControlled = true;
    }

    private void UpdateInput()
    {
        float h = 0f;
        float v = 0f;

        if (!isStuned)
        {
            h += moveInput.x;
            v += moveInput.y;

            if (Input.GetKey(inputKeys.forward)) v += 1f;
            if (Input.GetKey(inputKeys.backward)) v -= 1f;
            if (Input.GetKey(inputKeys.left)) h -= 1f;
            if (Input.GetKey(inputKeys.right)) h += 1f;

            if (Input.GetKeyDown(inputKeys.jump)) jumpPressed = true;
        }

        inputDir = new Vector3(h, 0f, v).normalized;

        // Vキーが押された瞬間にエフェクト再生
        //DEBUG
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.V))
        {
            Instantiate(slideEffect, transform.position, transform.rotation);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(smokeEffect, transform.position, transform.rotation);
        }
#endif

    }
    [SerializeField] private float jumpBufferTime = 0.15f; // store input
    [SerializeField] private float coyoteTime = 0.1f;      // allow jump after leaving ground

    private float jumpBufferCounter = 0f;
    private float coyoteCounter = 0f;
    bool jumpPressed = false;
    void Update()
    {
        if (!isAIControlled)
        {
            UpdateInput();
        }
        else
        {
            UpdateAIControlled();
        }

        if (!isStuned && jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime; // store input
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime; // countdown every frame
        }
        jumpPressed = false;

        coyoteCounter = jumpChecker.isGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;

        TurnToLookDir(inputDir);
        UpdateAnimator();
        UpdateStunedState();

        // ===============================
        // 押し続けジャンプ（高さ調整）
        // ===============================

        // 条件：
        // ・ジャンプボタンを押し続けている
        // ・現在ジャンプ中
        // ・押し続け可能な時間が残っている
        if (jumpHeld && isJumping && jumpHoldCounter > 0f)
        {
            // 現在の速度を取得
            Vector3 vel = rb.linearVelocity;

            // 押している間、少しずつ上方向速度を足す
            // Time.deltaTime を掛けることでフレームレート依存を防ぐ
            vel.y += holdJumpForce * Time.deltaTime;

            // 速度を反映
            rb.linearVelocity = vel;

            // 残りの押し続け時間を減らす
            jumpHoldCounter -= Time.deltaTime;
        }
        // ===============================
        // 上昇が終わったらジャンプ終了
        // ===============================
        // 上方向速度が0以下になったら
        // （上昇が終わり、落下に転じた）
        if (rb.linearVelocity.y <= 0f)
        {
            isJumping = false;
        }
    }

    private void UpdateAIControlled()
    {

        // Move straight toward target
        transform.position = Vector3.MoveTowards(transform.position, aiMoveTarget, moveSpeed * Time.deltaTime);

        Vector3 direction = (aiMoveTarget - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turningSpeed * Time.deltaTime);
        }


        //Stop when close
        if (Vector3.Distance(transform.position, aiMoveTarget) < 0.1f)
        {
            isAIControlled = false;
        }
    }

    void FixedUpdate()
    {
        moveSpeed = baseMoveSpeed;
        //moveSpeed = playerInfoSystem.GetDistanceAffectedPlayerSpeed(baseMoveSpeed);
        Move();
        Jump();
    }

    //void Jump()
    //{
    //    if (jumpBufferCounter > 0f && coyoteCounter > 0f)
    //    {
    //        Vector3 vel = rb.linearVelocity;
    //        vel.y = jumpForce;
    //        rb.linearVelocity = vel;

    //        jumpBufferCounter = 0f; // consume input
    //        coyoteCounter = 0f;     // consume coyote
    //        jumpChecker.isGrounded = false; // prevent infinite jump
    //        isJumping = true;

    //        // Play sound only here
    //        if (audioSource != null && jumpSound != null)
    //            audioSource.PlayOneShot(jumpSound);
    //    }
    //}
    //ジャンプ開始処理(最低ジャンプ)
    void Jump()
    {
        // ジャンプバッファ & コヨーテタイムの両方が有効なときのみジャンプ
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            // 現在の Rigidbody の速度を取得
            Vector3 vel = rb.linearVelocity;
            // 上方向（Y）だけをジャンプ用速度に変更
            // 横方向の速度は維持される
            vel.y = jumpForce;
            // 変更した速度を Rigidbody に反映
            rb.linearVelocity = vel;
            // ボタンを押し続けたとき用のタイマーをリセット
            jumpHoldCounter = maxJumpHoldTime;
            // 入力とコヨーテタイムを消費（1回のジャンプで使い切る）
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            // 地面にいる判定を強制的に解除（無限ジャンプ防止）
            jumpChecker.isGrounded = false;
            // ジャンプ状態に入ったことを記録
            isJumping = true;

            // ジャンプ音を1回だけ再生
            if (audioSource && jumpSound)
                audioSource.PlayOneShot(jumpSound);
        }
    }



    void Move()
    {
        if (rb.isKinematic) return;
        Vector3 vel = rb.linearVelocity;
        vel.x = inputDir.x * moveSpeed;
        vel.z = inputDir.z * moveSpeed;
        rb.linearVelocity = vel;
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

        bool isMoving = inputDir.sqrMagnitude > 0.001f;


        if (animator.HasParameterOfType("IsWalking", AnimatorControllerParameterType.Bool))
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


    public void EnterStunedState()
    {
        stunedTimer = 0f;
        isStuned = true;
        stunedComplete = false;

        // Stop player movement completely
        moveSpeed = 0f;

        starPopEffect.SetActive(true);

        // Set animation flag if exists
        if (animator && animator.HasParameterOfType("IsStuned", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsStuned", true);

    }

    public void UpdateStunedState()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
#if UNITY_EDITOR
            EnterStunedState();
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

    /* private void OnTriggerEnter(Collider other)
     {
         if (other.gameObject.CompareTag("Dart"))
         {
             EnterStunedState();
         }
     }*/

    //PAUSE MENU INPUT HANDLING

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
// Extension helper
public static class AnimatorExtensions
{
    public static bool HasParameterOfType(this Animator animator, string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.name == paramName && param.type == type) return true;
        }
        return false;
    }
}