using UnityEngine;

/// <summary>
/// Follows a player's horizontal (X and Z) position but stays at a fixed Y height
/// This prevents the camera from following jump movements
/// </summary>
public class FollowPlayerXZ : MonoBehaviour
{
    public Transform playerTransform;
    [SerializeField] private float fixedYHeight = 0f;
    private bool followZ = true;
    private float lockedZPosition;
    private bool hasInitialized = false;

    public void SetFollowZ(bool value)
    {
        followZ = value;
    }

    void Start()
    {
        // Store the initial Z position
        lockedZPosition = transform.position.z;
        hasInitialized = true;
    }

    private int logCounter = 0;
    void LateUpdate()
    {
        if (playerTransform == null || !hasInitialized) return;

        Vector3 targetPos = playerTransform.position;
        targetPos.y = fixedYHeight;

        if (!followZ)
        {
            targetPos.z = lockedZPosition; // Keep the locked Z position
        }

        transform.position = targetPos;
        logCounter++;
    }
}
