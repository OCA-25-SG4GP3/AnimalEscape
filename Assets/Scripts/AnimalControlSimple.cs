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
    [SerializeField] public PlayerInputKeys inputKeys = new(); //Player 1, Player 2 ?・ｽ・ｽﾊ々?・ｽ・ｽﾉ鯉ｿｽ?・ｽ・ｽﾟゑｿｽ

    Animator animator;
    [SerializeField] public float baseMoveSpeed = 5f;
    [SerializeField] public float moveSpeed = 5f; public void SetMoveSpeed(float _moveSpeed) { moveSpeed = _moveSpeed; }
    [SerializeField] public float jumpForce = 5f;
    [SerializeField] public LayerMask groundMask;
    [SerializeField] public float groundCheckRadius = 0.1f;
    [SerializeField, Header("Not a prefab")] private GameObject starPopEffect;

    //・ｽ・ｽ・ｽn・ｽ・ｽSE・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ
    public AudioClip jumpSound;      // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾌフ・ｽ@・ｽC・ｽ・ｽ
    public AudioClip landingSound; // ・ｽ・ｽ・ｽn・ｽ・ｽ
    [NonSerializedAttribute] public AudioSource audioSource; // AudioSource・ｽ・ｽ・ｽg・ｽ・ｽ・ｽ・ｽ・ｽﾟの変撰ｿｽ

    Rigidbody rb;
    Vector3 inputDir;
    JumpChecker jumpChecker;

    public GameObject moveEffect;
    private bool moveEffectFlag = false;
    private int moveEffectFrame = 0;

    public GameObject smokeEffect;
    bool isAIControlled = false; public bool IsAIControlled => isAIControlled;
    [NonSerializedAttribute] public bool isJumping = false;  // ?・ｽ・ｽW?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽv?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽﾇゑｿｽ?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽﾇ跡ゑｿｽ?・ｽ・ｽ?・ｽ・ｽt?・ｽ・ｽ?・ｽ・ｽ?・ｽ・ｽO
    PlayerInput playerInput;
    OptionMenu optionMenu;

    //・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽz・ｽ[・ｽ・ｽ・ｽh・ｽp・ｽﾇ会ｿｽ
    [SerializeField] public float holdJumpForce = 10f;      // ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽﾔの追会ｿｽ・ｽ・ｽ
    public float maxJumpHoldTime = 0.2f;   // ・ｽﾅ托ｿｽﾅ会ｿｽ・ｽ・ｽ・ｽ骼橸ｿｽ・ｽ
    public float jumpHoldCounter;   // ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽc・ｽ闔橸ｿｽ・ｽ
    public bool jumpHeld;   // ・ｽ・ｽ・ｽﾝジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ{・ｽ^・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾄゑｿｽ・ｽ驍ｩ・ｽﾇゑｿｽ・ｽ・ｽ

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
    //・ｽ・ｽ・ｽ・ｽ・ｽA・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽm
    public void OnJump(InputAction.CallbackContext context)
    {
        if (isAIControlled)
        {
            jumpPressed = false;  // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾍゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽL・ｽ^・ｽi・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽo・ｽb・ｽt・ｽ@・ｽp・ｽj
            jumpHeld = false;   // ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾔゑｿｽ・ｽ・ｽ・ｽ・ｽ
            return;
        }

        if (context.performed)
        {
            jumpPressed = true;  // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾍゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽL・ｽ^・ｽi・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽo・ｽb・ｽt・ｽ@・ｽp・ｽj
            jumpHeld = true;    // ・ｽ{・ｽ^・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽ・ｽﾔにゑｿｽ・ｽ・ｽ
        }

        if (context.canceled)
        {
            jumpHeld = false;   // ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾔゑｿｽ・ｽ・ｽ・ｽ・ｽ
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

        //編集:江頭
        //ポーズ時にプレイヤーを回転させなくする処理です
        if (Time.timeScale != 0)
        {
            TurnToLookDir(inputDir);
        }
        UpdateAnimator();
        UpdateStunedState();

        // ===============================
        // ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽi・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽj
        // ===============================

        // ・ｽ・ｽ・ｽ・ｽ・ｽF
        // ・ｽE・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ{・ｽ^・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽ
        // ・ｽE・ｽ・ｽ・ｽﾝジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ
        // ・ｽE・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾂ能・ｽﾈ趣ｿｽ・ｽﾔゑｿｽ・ｽc・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽ
        if (jumpHeld && isJumping && jumpHoldCounter > 0f)
        {
            // ・ｽ・ｽ・ｽﾝの托ｿｽ・ｽx・ｽ・ｽ・ｽ謫ｾ
            Vector3 vel = rb.linearVelocity;

            // ・ｽ・ｽ・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽﾔ、・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾂ擾ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽx・ｽｫゑｿｽ
            // Time.deltaTime ・ｽ・ｽ・ｽ|・ｽ・ｽ・ｽ驍ｱ・ｽﾆでフ・ｽ・ｽ・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽ[・ｽg・ｽﾋ托ｿｽ・ｽ・ｽh・ｽ・ｽ
            vel.y += holdJumpForce * Time.deltaTime;

            // ・ｽ・ｽ・ｽx・ｽｽ映
            rb.linearVelocity = vel;

            // ・ｽc・ｽ・ｽﾌ会ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾔゑｿｽ・ｽ・ｽ・ｽ轤ｷ
            jumpHoldCounter -= Time.deltaTime;
        }
        // ===============================
        // ・ｽ繽ｸ・ｽ・ｽ・ｽI・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽI・ｽ・ｽ
        // ===============================
        // ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽx・ｽ・ｽ0・ｽﾈ会ｿｽ・ｽﾉなゑｿｽ・ｽ・ｽ・ｽ・ｽ
        // ・ｽi・ｽ繽ｸ・ｽ・ｽ・ｽI・ｽ・ｽ・ｽA・ｽ・ｽ・ｽ・ｽ・ｽﾉ転・ｽ・ｽ・ｽ・ｽ・ｽj
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
    //・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽJ・ｽn・ｽ・ｽ・ｽ・ｽ(・ｽﾅ抵ｿｽW・ｽ・ｽ・ｽ・ｽ・ｽv)
    void Jump()
    {
        // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽo・ｽb・ｽt・ｽ@ & ・ｽR・ｽ・ｽ・ｽ[・ｽe・ｽ^・ｽC・ｽ・ｽ・ｽﾌ暦ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽL・ｽ・ｽ・ｽﾈとゑｿｽ・ｽﾌみジ・ｽ・ｽ・ｽ・ｽ・ｽv
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            // ・ｽ・ｽ・ｽﾝゑｿｽ Rigidbody ・ｽﾌ托ｿｽ・ｽx・ｽ・ｽ・ｽ謫ｾ
            Vector3 vel = rb.linearVelocity;
            // ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽiY・ｽj・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽp・ｽ・ｽ・ｽx・ｽﾉ変更
            // ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾌ托ｿｽ・ｽx・ｽﾍ維趣ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ
            vel.y = jumpForce;
            // ・ｽﾏ更・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽx・ｽ・ｽ Rigidbody ・ｽﾉ費ｿｽ・ｽf
            rb.linearVelocity = vel;
            // ・ｽ{・ｽ^・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽp・ｽﾌタ・ｽC・ｽ}・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽZ・ｽb・ｽg
            jumpHoldCounter = maxJumpHoldTime;
            // ・ｽ・ｽ・ｽﾍとコ・ｽ・ｽ・ｽ[・ｽe・ｽ^・ｽC・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽi1・ｽ・ｽﾌジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽﾅ使・ｽ・ｽ・ｽﾘゑｿｽj
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            // ・ｽn・ｽﾊにゑｿｽ・ｽ髞ｻ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽI・ｽﾉ会ｿｽ・ｽ・ｽ・ｽi・ｽ・ｽ・ｽ・ｽ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽh・ｽ~・ｽj
            jumpChecker.isGrounded = false;
            // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽﾔに難ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽL・ｽ^
            isJumping = true;

            // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ1・ｽｾゑｿｽ・ｽﾄ撰ｿｽ
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