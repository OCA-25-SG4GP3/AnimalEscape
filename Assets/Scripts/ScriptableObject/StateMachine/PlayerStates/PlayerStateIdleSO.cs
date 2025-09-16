using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "PlayerStateIdleSO", menuName = "State/PlayerState/PlayerStateIdleSO")]
public class PlayerStateIdleSO : PlayerStateBaseSO
{
    public override void UpdateState()
    {
        if (_playerBase.MoveInput != Vector2.zero || !_playerBase.IsGrounded)
        {
            IsComplete = true;
        }
    }
}
