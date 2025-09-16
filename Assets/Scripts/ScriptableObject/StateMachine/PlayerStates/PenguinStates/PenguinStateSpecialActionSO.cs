using UnityEngine;

[CreateAssetMenu(fileName = "PenguinStateSpecialActionSO", menuName = "State/PlayerState/PenguinState/PenguinStateSpecialActionSO")]
public class PenguinStateSpecialActionSO : PlayerStateSpecialActionSO
{
    [Header("Slide Parameter")]
    [SerializeField] private float _slideSpeed = 8f;
    [SerializeField] private float _slideDuration = 0.3f;
    private Vector3 _movement;
    private Quaternion _originalRotation;
    private Quaternion _slideTiltRotation;

    private PlayerInfo _playerInfo;

    public override void EnterState()
    {
        _movement = _playerBase.Model.forward;
        _playerBase.MoveSpeed = _slideSpeed;
        _originalRotation = _playerBase.Model.rotation;
        _slideTiltRotation = Quaternion.Euler(90.0f, _playerBase.Model.eulerAngles.y, _playerBase.Model.eulerAngles.z);
        _playerBase.Model.rotation = _slideTiltRotation;
        CanBeInterrupted = false;
        _playerInfo = _playerBase.GetComponent<PlayerInfo>();
        _playerInfo.hasCaught = true;
        if (_playerBase is PenguinControl first)
        {
            first.ImpactCollider.SetActive(true);
        }
        StartTimer();
    }

    public override void UpdateState()
    {
        if (ElapsedTime >= _slideDuration)
        {
            IsComplete = true;
            CanBeInterrupted = true;
            _playerInfo.hasCaught = false;
            if (_playerBase is PenguinControl first)
            {
                first.ImpactCollider.SetActive(false);
            }
        }
    }

    public override void FixedUpdateState()
    {
        _playerBase.Move(new(_movement.x, _movement.z));
    }

    public override void ExitState()
    {
        base.ExitState();
        _playerBase.MoveSpeed = _playerBase.WalkSpeed;
        _playerBase.Model.rotation = _originalRotation;
    }
}
