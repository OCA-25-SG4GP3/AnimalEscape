using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�ړ��ݒ�")]
    public float moveSpeed = 5f;      // �ړ����x
    public float jumpForce = 5f;      // �W�����v��
    public float groundCheckDistance = 0.1f; // �ڒn����̋���

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal"); // A/D�L�[ or ��/��
        float v = Input.GetAxis("Vertical");   // W/S�L�[ or ��/��

        Vector3 direction = new Vector3(h, 0f, v);
        Vector3 move = transform.TransformDirection(direction) * moveSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

    void Jump()
    {
        // �ڒn�`�F�b�N
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * (groundCheckDistance + 0.1f));
    }
}
