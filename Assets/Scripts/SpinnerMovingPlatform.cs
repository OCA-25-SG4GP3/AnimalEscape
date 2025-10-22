using System.Collections.Generic;
using UnityEngine;

public class SpinnerMovingPlatform : MonoBehaviour
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
        Quaternion delta = Quaternion.Euler(rotationSpeed * Time.fixedDeltaTime);
        targetRotation *= delta;
        targetRotation.Normalize(); // keep it unit length
        rb.MoveRotation(targetRotation);
    }

}
