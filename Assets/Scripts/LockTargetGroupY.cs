using UnityEngine;

/// <summary>
/// Locks the TargetGroup's Y position so the camera doesn't follow vertical player movement
/// Attach this to the TargetGroup GameObject
/// </summary>
public class LockTargetGroupY : MonoBehaviour
{
    [SerializeField] private float lockedYPosition = 0f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = lockedYPosition;
        transform.position = pos;
    }
}
