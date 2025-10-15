using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class GodmodeCheat : MonoBehaviour
{
    [Header("Toggle Key")]
    public KeyCode toggleKey = KeyCode.P;

    [Header("Movement Speed")]
    public float speed = 20f;

    private Rigidbody rb;
    private Collider col;
    private bool godmode = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        // Toggle Godmode
        if (Input.GetKeyDown(toggleKey))
        {
            godmode = !godmode;
            rb.useGravity = !godmode;
            rb.isKinematic = godmode;      // disables physics simulation
            col.enabled = !godmode;        // disables collisions
        }

        if (godmode)
            HandleMovement();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float v = Input.GetAxis("Vertical");   // W/S or Up/Down

        Vector3 move = new Vector3(h, 0, v);
        move = Camera.main.transform.TransformDirection(move); // relative to camera
        move.y = 0; // keep horizontal

        transform.position += move * speed * Time.deltaTime;
    }
}
