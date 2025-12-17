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
    [SerializeField] public PlayerInputKeys inputKeys = new(); //Player 1, Player 2 ??ｿｽ?ｿｽﾊ々??ｿｽ?ｿｽﾉ鯉ｿｽ??ｿｽ?ｿｽﾟゑｿｽ
    Animator animator;
    [SerializeField] public float baseMoveSpeed = 5f;
    [SerializeField] public float moveSpeed = 5f; public void SetMoveSpeed(float _moveSpeed) { moveSpeed = _moveSpeed; }
    [SerializeField] public float jumpForce = 5f;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public float groundCheckRadius = 0.1f;
    [SerializeField, Header("Not a prefab")] private GameObject starPopEffect;

    //着地のSEを入れる
    public AudioClip jumpSound;      // ジャンプ音のファイル
    public AudioClip landingSound;   // 着地音
    public AudioClip walkSound;      // 歩く
    [NonSerializedAttribute] public AudioSource audioSource; // AudioSourceを使うための変数


    Rigidbody rb;
    Vector3 inputDir;
    JumpChecker jumpChecker;

    public GameObject moveEffect;
    private bool moveEffectFlag = false;
    private int moveEffectFrame = 0;

    public GameObject smokeEffect;
    bool isAIControlled = false; public bool IsAIControlled => isAIControlled;
    [NonSerializedAttribute] public bool isJumping = false;  // ??ｿｽ?ｿｽW??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽv??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾇゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽﾇ跡ゑｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽt??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽO
    PlayerInput playerInput;
    OptionMenu optionMenu;

    //?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽz?ｿｽ[?ｿｽ?ｿｽ?ｿｽh?ｿｽp?ｿｽﾇ会ｿｽ
    [SerializeField] public float holdJumpForce = 10f;      // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽﾔの追会ｿｽ?ｿｽ?ｿｽ
    public float maxJumpHoldTime = 0.2f;   // ?ｿｽﾅ托ｿｽﾅ会ｿｽ?ｿｽ?ｿｽ?ｿｽ骼橸ｿｽ?ｿｽ
    public float jumpHoldCounter;   // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽc?ｿｽ闔橸ｿｽ?ｿｽ
    public bool jumpHeld;   // ?ｿｽ?ｿｽ?ｿｽﾝジ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽ{?ｿｽ^?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ驍ｩ?ｿｽﾇゑｿｽ?ｿｽ?ｿｽ

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
    //?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽA?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽm
    public void OnJump(InputAction.CallbackContext context)
    {
        if (isAIControlled)
        {
            jumpPressed = false;  // ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽ?ｿｽ?ｿｽﾍゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽL?ｿｽ^?ｿｽi?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽo?ｿｽb?ｿｽt?ｿｽ@?ｿｽp?ｿｽj
            jumpHeld = false;   // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾔゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
            return;
        }

        if (context.performed)
        {
            jumpPressed = true;  // ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽ?ｿｽ?ｿｽﾍゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽL?ｿｽ^?ｿｽi?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽo?ｿｽb?ｿｽt?ｿｽ@?ｿｽp?ｿｽj
            jumpHeld = true;    // ?ｿｽ{?ｿｽ^?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽ?ｿｽﾔにゑｿｽ?ｿｽ?ｿｽ
        }

        if (context.canceled)
        {
            jumpHeld = false;   // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾔゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
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
                    Instantiate(moveEffect, transform.position, transform.rotation);
                    moveEffectFrame = 0;
                    moveEffectFlag = false;
                }
            }
        }

        inputDir = new Vector3(h, 0f, v).normalized;

        // V??ｿｽ?ｿｽL??ｿｽ?ｿｽ[??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ??ｿｽ?ｿｽ黷ｽ??ｿｽ?ｿｽu??ｿｽ?ｿｽﾔにエ??ｿｽ?ｿｽt??ｿｽ?ｿｽF??ｿｽ?ｿｽN??ｿｽ?ｿｽg??ｿｽ?ｿｽﾄ撰ｿｽ
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
        // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽi?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽj
        // ===============================

        // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽF
        // ?ｿｽE?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽ{?ｿｽ^?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽ
        // ?ｿｽE?ｿｽ?ｿｽ?ｿｽﾝジ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽ?ｿｽ
        // ?ｿｽE?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾂ能?ｿｽﾈ趣ｿｽ?ｿｽﾔゑｿｽ?ｿｽc?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽ
        if (jumpHeld && isJumping && jumpHoldCounter > 0f)
        {
            // ?ｿｽ?ｿｽ?ｿｽﾝの托ｿｽ?ｿｽx?ｿｽ?ｿｽ?ｿｽ謫ｾ
            Vector3 vel = rb.linearVelocity;

            // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾄゑｿｽ?ｿｽ?ｿｽﾔ、?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾂ擾ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽx?ｿｽ?ｫゑｿｽ
            // Time.deltaTime ?ｿｽ?ｿｽ?ｿｽ|?ｿｽ?ｿｽ?ｿｽ驍ｱ?ｿｽﾆでフ?ｿｽ?ｿｽ?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ[?ｿｽg?ｿｽﾋ托ｿｽ?ｿｽ?ｿｽh?ｿｽ?ｿｽ
            vel.y += holdJumpForce * Time.deltaTime;

            // ?ｿｽ?ｿｽ?ｿｽx?ｿｽ?ｽ映
            rb.linearVelocity = vel;

            // ?ｿｽc?ｿｽ?ｿｽﾌ会ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾔゑｿｽ?ｿｽ?ｿｽ?ｿｽ轤ｷ
            jumpHoldCounter -= Time.deltaTime;
        }
        // ===============================
        // ?ｿｽ繽ｸ?ｿｽ?ｿｽ?ｿｽI?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽI?ｿｽ?ｿｽ
        // ===============================
        // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽx?ｿｽ?ｿｽ0?ｿｽﾈ会ｿｽ?ｿｽﾉなゑｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
        // ?ｿｽi?ｿｽ繽ｸ?ｿｽ?ｿｽ?ｿｽI?ｿｽ?ｿｽ?ｿｽA?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾉ転?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽj
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
    //?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽJ?ｿｽn?ｿｽ?ｿｽ?ｿｽ?ｿｽ(?ｿｽﾅ抵ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv)
    void Jump()
    {
        // ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽo?ｿｽb?ｿｽt?ｿｽ@ & ?ｿｽR?ｿｽ?ｿｽ?ｿｽ[?ｿｽe?ｿｽ^?ｿｽC?ｿｽ?ｿｽ?ｿｽﾌ暦ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽL?ｿｽ?ｿｽ?ｿｽﾈとゑｿｽ?ｿｽﾌみジ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            // ?ｿｽ?ｿｽ?ｿｽﾝゑｿｽ Rigidbody ?ｿｽﾌ托ｿｽ?ｿｽx?ｿｽ?ｿｽ?ｿｽ謫ｾ
            Vector3 vel = rb.linearVelocity;
            // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽiY?ｿｽj?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽp?ｿｽ?ｿｽ?ｿｽx?ｿｽﾉ変更
            // ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾌ托ｿｽ?ｿｽx?ｿｽﾍ維趣ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ
            vel.y = jumpForce;
            // ?ｿｽﾏ更?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽx?ｿｽ?ｿｽ Rigidbody ?ｿｽﾉ費ｿｽ?ｿｽf
            rb.linearVelocity = vel;
            // ?ｿｽ{?ｿｽ^?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽp?ｿｽﾌタ?ｿｽC?ｿｽ}?ｿｽ[?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽZ?ｿｽb?ｿｽg
            jumpHoldCounter = maxJumpHoldTime;
            // ?ｿｽ?ｿｽ?ｿｽﾍとコ?ｿｽ?ｿｽ?ｿｽ[?ｿｽe?ｿｽ^?ｿｽC?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽi1?ｿｽ?ｿｽﾌジ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽﾅ使?ｿｽ?ｿｽ?ｿｽﾘゑｿｽj
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            // ?ｿｽn?ｿｽﾊにゑｿｽ?ｿｽ髞ｻ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽI?ｿｽﾉ会ｿｽ?ｿｽ?ｿｽ?ｿｽi?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽh?ｿｽ~?ｿｽj
            jumpChecker.isGrounded = false;
            // ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽ?ｿｽﾔに難ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽﾆゑｿｽ?ｿｽL?ｿｽ^
            isJumping = true;

            // ?ｿｽW?ｿｽ?ｿｽ?ｿｽ?ｿｽ?ｿｽv?ｿｽ?ｿｽ?ｿｽ?ｿｽ1?ｿｽ?ｾゑｿｽ?ｿｽﾄ撰ｿｽ
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
        //audioSource.PlayOneShot(walkSound);

        if (inputDir.sqrMagnitude > 0.001f) // 動いている場合
        {
            // ループ用の歩行音を設定
            if (!audioSource.isPlaying && walkSound != null)
            {
                audioSource.clip = walkSound;
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