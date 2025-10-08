using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PenguinControlSimple : MonoBehaviour
{
    Animator animator;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public LayerMask groundMask;
    public float groundCheckRadius = 0.3f;
    Rigidbody rb;
    bool isGrounded;
    Vector3 inputDir;
    bool jumpRequested;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        inputDir = new Vector3(h, 0, v).normalized;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpRequested = true;

        TurnToLookDir(inputDir);

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
        Jump();
    }

    void CheckGround()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, groundCheckRadius, groundMask);
        isGrounded = false;

        foreach (Collider hit in hits)
        {
            if (hit.gameObject != gameObject) // ignore self
            {
                isGrounded = true;
                break;
            }
        }
    }

    void Move()
    {
        Vector3 vel = rb.linearVelocity;
        vel.x = inputDir.x * moveSpeed;
        vel.z = inputDir.z * moveSpeed;
        rb.linearVelocity = vel;
    }

    void Jump()
    {
        if (jumpRequested && isGrounded)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = jumpForce;
            rb.linearVelocity = vel;
        }
        jumpRequested = false;
    }

    void TurnToLookDir(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.fixedDeltaTime);
        }
    }

    void UpdateAnimator()
    {
        bool isMoving = inputDir.sqrMagnitude > 0.001f;
        animator.SetBool("IsWalking", isMoving && isGrounded);
        animator.SetBool("IsIdle", !isMoving && isGrounded);
        animator.SetBool("IsJumping", !isGrounded);
    }
}
