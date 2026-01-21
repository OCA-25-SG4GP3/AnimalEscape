using UnityEngine;

/// <summary>
/// Movement settings for animals - 移動担当用
/// Create assets: Right-click → Create → Animal → MovementSettings
/// </summary>
[CreateAssetMenu(fileName = "AnimalMovementSettings", menuName = "Animal/MovementSettings")]
public class AnimalMovementSettings : ScriptableObject
{
    [Header("移動 (Movement)")]
    public float baseMoveSpeed = 5f;
    public float moveSpeedOnFinishMult = 1.3f;

    [Header("ジャンプ (Jump)")]
    public float jumpForce = 5f;
    public float holdJumpForce = 10f;
    public float maxJumpHoldTime = 0.2f;

    [Header("物理 (Physics)")]
    public float groundCheckRadius = 0.1f;
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2.0f;

    [Header("タイミング (Timing)")]
    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.1f;
    public float stunedDuration = 1.5f;
}
