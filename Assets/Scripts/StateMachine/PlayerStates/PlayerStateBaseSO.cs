using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateBaseSO", menuName = "State/PlayerStateBaseSO")]
public class PlayerStateBaseSO : StateBaseSO
{
    [SerializeField] protected PlayerBase _playerBase;
    public virtual void SetPlayer(PlayerBase playerBase)
    {
        _playerBase = playerBase;
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }
    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void LateUpdateState()
    {
    }

    public override void DrawStateGizmo()
    {
    }
}