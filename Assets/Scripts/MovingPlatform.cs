using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] List<Transform> points;
    [SerializeField] float speed = 2f;
    [SerializeField] bool loop = true;
    [SerializeField] float stopTime = 1f; // seconds to wait at each point

    Rigidbody rb;
    int currentIndex = 0;
    float waitTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        if (points.Count == 0) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Transform target = points[currentIndex];
        Vector3 direction = target.position - rb.position;
        float distance = direction.magnitude;

        if (distance < 0.001f) // close enough
        {
            waitTimer = stopTime; // start waiting
            currentIndex++;
            if (currentIndex >= points.Count)
            {
                if (loop) currentIndex = 0;
                else return;
            }
            return;
        }

        Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
        if (move.magnitude > distance) move = direction;

        rb.MovePosition(rb.position + move);
    }
}
