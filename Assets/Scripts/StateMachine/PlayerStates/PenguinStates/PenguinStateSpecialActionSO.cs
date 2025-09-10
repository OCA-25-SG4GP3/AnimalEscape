using UnityEngine;

[CreateAssetMenu(fileName = "PenguinStateSpecialActionSO", menuName = "State/PlayerState/PenguinState/PenguinStateSpecialActionSO")]
public class PenguinStateSpecialActionSO : PlayerStateSpecialActionSO
{
    [Header("Slide Parameter")]
    [SerializeField] private float _slideSpeed = 8f;
    [SerializeField] private float _slideDuration = 0.5f;
    private Vector3 _movement;
    private Quaternion _originalRotation;
    private Quaternion _slideTiltRotation;

    public override void EnterState()
    {
        _movement = _playerBase.transform.forward;
        _playerBase.MoveSpeed = _slideSpeed;
        _originalRotation = _playerBase.transform.rotation;
        _slideTiltRotation = Quaternion.Euler(90.0f, _playerBase.transform.eulerAngles.y, _playerBase.transform.eulerAngles.z);
        _playerBase.transform.rotation = _slideTiltRotation;
        CanBeInterrupted = false;
        StartTimer();
    }

    public override void UpdateState()
    {
        if (ElapsedTime >= _slideDuration)
        {
            IsComplete = true;
            CanBeInterrupted = true;
        }
    }

    public override void FixedUpdateState()
    {
        _playerBase.Move(_movement);
    }

    public override void ExitState()
    {
        base.ExitState();
        _playerBase.MoveSpeed = _playerBase.WalkSpeed;
        _playerBase.transform.rotation = _originalRotation;
    }
}
