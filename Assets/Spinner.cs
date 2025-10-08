using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] Vector3 rotationSpeed = new Vector3(0, 90, 0); // degrees per second
    Rigidbody rb;
    Quaternion targetRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        targetRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        // Calculate rotation this frame
        Quaternion delta = Quaternion.Euler(rotationSpeed * Time.fixedDeltaTime);
        targetRotation *= delta;

        // Use MoveRotation for physics interaction
        rb.MoveRotation(targetRotation);
    }
}
