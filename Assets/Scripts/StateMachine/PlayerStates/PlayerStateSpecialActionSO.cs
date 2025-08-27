using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateSpecialActionSO", menuName = "State/PlayerState/PlayerSpecialActionSO")]
public class PlayerStateSpecialActionSO : PlayerStateBaseSO
{
    public override void UpdateState()
    {
        IsComplete = true;
    }

    public override void ExitState()
    {
        _playerBase.IsSpecialAction = false;
    }
}
