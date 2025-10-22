using UnityEngine;
using System.Collections.Generic;

public class MovingPlatformAttacher : MonoBehaviour
{
    [SerializeField] private GameObject movingObject;
    private List<Rigidbody> riders = new();
    private Vector3 lastPos;
    private Quaternion lastRot;

    void Awake()
    {
        if (movingObject == null) movingObject = gameObject;
        lastPos = movingObject.transform.position;
        lastRot = movingObject.transform.rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null && !riders.Contains(rb))
                riders.Add(rb);
        }
    }

    void FixedUpdate()
    {
        if (riders.Count == 0) 
        {
            // still update last transform in case platform moved without riders
            lastPos = movingObject.transform.position;
            lastRot = movingObject.transform.rotation;
            return;
        }

        Transform plat = movingObject.transform;
        Vector3 platPos = plat.position;
        Quaternion platRot = plat.rotation;

        foreach (var rb in riders.ToArray()) // ToArray avoids modification issues
        {
            if (rb == null)
            {
                riders.Remove(rb);
                continue;
            }

            // rider world pos at start of frame
            Vector3 riderWorld = rb.position;

            // express rider in platform-local coords based on last frame
            Vector3 local = Quaternion.Inverse(lastRot) * (riderWorld - lastPos);

            // new world position = current platform pos + current platform rotation * local
            Vector3 newWorld = platPos + (platRot * local);

            // move once
            rb.MovePosition(newWorld);
        }

        lastPos = platPos;
        lastRot = platRot;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if (rb != null) riders.Remove(rb);
        }
    }
}
