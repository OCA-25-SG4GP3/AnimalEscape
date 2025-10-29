using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateStunnedSO", menuName = "State/PlayerState/PlayerStateStunnedSO")]
public class PlayerStateStunnedSO : PlayerStateBaseSO
{
    [SerializeField] private float stunDuration = 3f;
    private float timer;

    public override void EnterState()
    {
        timer = 0f;
        _playerBase.MoveSpeed = 0f;

        if (_playerBase.Animator)
            _playerBase.Animator.SetBool("IsStunned", true);

        //Debug.Log($"{_playerBase.name} entered Stunned state!");
    }

    public override void UpdateState()
    {
        timer += Time.deltaTime;

        // Player can't move or rotate during stun
        _playerBase.Move(Vector2.zero);

        if (timer >= stunDuration)
        {
            IsComplete = true;
        }
    }

    public override void ExitState()
    {
        _playerBase.MoveSpeed = _playerBase.WalkSpeed;

        if (_playerBase.Animator)
            _playerBase.Animator.SetBool("IsStunned", false);

        //Debug.Log($"{_playerBase.name} recovered from stun.");
    }
}