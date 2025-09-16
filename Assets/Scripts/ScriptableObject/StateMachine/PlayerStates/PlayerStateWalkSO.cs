using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateWalkSO", menuName = "State/PlayerState/PlayerStateWalkSO")]
public class PlayerStateWalkSO : PlayerStateBaseSO
{
    public override void EnterState()
    {
        _playerBase.MoveSpeed = _playerBase.WalkSpeed;
    }

    public override void FixedUpdateState()
    {
        Vector2 direction = _playerBase.MoveInput;
        _playerBase.Move(direction);
        _playerBase.Rotate(direction);
    }

    public override void UpdateState()
    {
        if (_playerBase.MoveInput == Vector2.zero || !_playerBase.IsGrounded)
        {
            IsComplete = true;
        }
    }
}
