using UnityEngine;
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
}
[RequireComponent(typeof(Rigidbody))]
public class AnimalControlSimple : MonoBehaviour
{
    PlayerInfoSystem playerInfoSystem;
    [SerializeField] public PlayerInputKeys inputKeys = new(); //Player 1, Player 2 別々に決める
    Animator animator;
    [SerializeField] public float baseMoveSpeed = 5f;
    public float moveSpeed = 5f; public void SetMoveSpeed(float _moveSpeed) { moveSpeed = _moveSpeed; }
    public float jumpForce = 7f;
    public LayerMask groundMask;
    public float groundCheckRadius = 0.3f;
    Rigidbody rb;
    bool isGrounded;
    Vector3 inputDir;

    public GameObject jumpEffect;
    public GameObject slideEffect;
    public GameObject smokeEffect;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        playerInfoSystem = GameObject.FindAnyObjectByType<PlayerInfoSystem>();
    }
    void Start()
    {
        moveSpeed = baseMoveSpeed;
    }


    private void UpdateInput()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(inputKeys.forward)) v += 1f;
        if (Input.GetKey(inputKeys.backward)) v -= 1f;
        if (Input.GetKey(inputKeys.left)) h -= 1f;
        if (Input.GetKey(inputKeys.right)) h += 1f;

        inputDir = new Vector3(h, 0f, v).normalized;


        // Vキーが押された瞬間にエフェクト再生
        if (Input.GetKeyDown(KeyCode.V))
        {
            Instantiate(slideEffect, transform.position, transform.rotation);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(smokeEffect, transform.position, transform.rotation);
        }

    }
    [SerializeField] private float jumpBufferTime = 0.15f; // store input
    [SerializeField] private float coyoteTime = 0.1f;      // allow jump after leaving ground

    private float jumpBufferCounter = 0f;
    private float coyoteCounter = 0f;

    void Update() //hayai
    {
        UpdateInput();
        // Jump input
        if (Input.GetKeyDown(inputKeys.jump))
        {
            jumpBufferCounter = jumpBufferTime;
            isHitGroundOnce = false;
        }
        else
            jumpBufferCounter -= Time.deltaTime;

        // Update coyote
        coyoteCounter = isGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;

        TurnToLookDir(inputDir);
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        moveSpeed = baseMoveSpeed;
        //moveSpeed = playerInfoSystem.GetDistanceAffectedPlayerSpeed(baseMoveSpeed);
        CheckGround();
        Move();
        Jump();
    }

    void Jump()
    {
        // Can jump if we pressed jump recently OR within coyote time
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = jumpForce;
            rb.linearVelocity = vel;

            jumpBufferCounter = 0f; // consume input
            coyoteCounter = 0f;     // consume coyote
        }
    }

    bool isHitGroundOnce = false;

    void CheckGround()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, groundCheckRadius, groundMask);

        isGrounded = false;
        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject && isHitGroundOnce == false) // ignore self
            {
                isGrounded = true;
                isHitGroundOnce = true;

                GameObject effect = Instantiate(jumpEffect, transform.position, transform.rotation);

                break;
            }
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


    void TurnToLookDir(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.fixedDeltaTime);
        }
    }

    void UpdateAnimator() //for now we use this to prevent warnings on monkey
    {
        if (!animator) return;

        bool isMoving = inputDir.sqrMagnitude > 0.001f;

        if (animator.HasParameterOfType("IsWalking", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsWalking", isMoving && isGrounded);

        if (animator.HasParameterOfType("IsIdle", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsIdle", !isMoving && isGrounded);

        if (animator.HasParameterOfType("IsJumping", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsJumping", !isGrounded);
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