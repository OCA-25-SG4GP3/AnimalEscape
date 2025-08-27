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
        Vector3 direction = new(_playerBase.MoveInput.x, 0, _playerBase.MoveInput.y);
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
