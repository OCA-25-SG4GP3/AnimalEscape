using UnityEngine;

public class MovingPlatformAttacher : MonoBehaviour
{
    [SerializeField] private GameObject movingObject;

    private Rigidbody playerRb;
    private Vector3 lastPos;
    private Quaternion lastRot;

    void Awake()
    {
        if (movingObject == null) movingObject = this.gameObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = collision.gameObject.GetComponent<Rigidbody>();
            lastPos = movingObject.transform.position;
            lastRot = movingObject.transform.rotation;
        }
    }

    void FixedUpdate()
    {
        if (playerRb)
        {
            Transform plat = movingObject.transform;

            // Movement delta
            Vector3 deltaPos = plat.position - lastPos;
            playerRb.MovePosition(playerRb.position + deltaPos);

            // Rotation delta around platform pivot
            Quaternion deltaRot = plat.rotation * Quaternion.Inverse(lastRot);
            Vector3 dir = playerRb.position - plat.position;
            dir = deltaRot * dir;
            playerRb.MovePosition(plat.position + dir);

            lastPos = plat.position;
            lastRot = plat.rotation;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerRb = null;
        }
    }
}
