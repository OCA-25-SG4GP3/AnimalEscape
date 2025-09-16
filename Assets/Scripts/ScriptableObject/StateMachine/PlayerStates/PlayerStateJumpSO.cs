using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateJumpSO", menuName = "State/PlayerState/PlayerStateJumpSO")]
public class PlayerStateJumpSO : PlayerStateBaseSO
{
    [SerializeField] private float _gravityMultiplier;
    private bool _isJumping = false;
    public override void EnterState()
    {
        if (_isJumping)
        {
            return;
        }
        _playerBase.Rigidbody.AddForce(Vector3.up * _playerBase.JumpPower, ForceMode.Impulse);
        _isJumping = true;
        // _playerBase.Animator.SetBool("IsJumping", true);
        if(_playerBase.Animator) _playerBase.Animator.Play("Jump");
        StartTimer();
    }

    public override void ExitState()
    {
        _isJumping = false;
        if(_playerBase.Animator) _playerBase.Animator.Play("Idle");
        // _playerBase.Animator.SetBool("IsJumping", true);
    }

    public override void FixedUpdateState()
    {
        Vector2 direction = _playerBase.MoveInput;
        _playerBase.Move(direction);
        _playerBase.Rotate(direction);

        if (_playerBase.Rigidbody.linearVelocity.y < 0)
        {
            _playerBase.Rigidbody.linearVelocity += _gravityMultiplier * Time.fixedDeltaTime * Physics.gravity.y * Vector3.up;
        }

        if (_playerBase.IsGrounded && ElapsedTime > 0.05f)
        {
            IsComplete = true;
        }
    }
}
