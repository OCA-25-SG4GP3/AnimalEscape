using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateJumpSO", menuName = "State/PlayerState/PlayerStateJumpSO")]
public class PlayerStateJumpSO : PlayerStateBaseSO
{
    private bool _isJumping = false;
    public override void EnterState()
    {
        if (_isJumping)
        {
            return;
        }
        _playerBase.Rigidbody.AddForce(Vector3.up * _playerBase.JumpPower, ForceMode.Impulse);
        _isJumping = true;
        StartTimer();
    }

    public override void ExitState()
    {
        _isJumping = false;
    }

    public override void FixedUpdateState()
    {
        Vector3 direction = new(_playerBase.MoveInput.x, 0, _playerBase.MoveInput.y);
        _playerBase.Move(direction);
        _playerBase.Rotate(direction);

        if (_playerBase.IsGrounded && ElapsedTime > 0.05f)
        {
            IsComplete = true;
        }
    }
}
