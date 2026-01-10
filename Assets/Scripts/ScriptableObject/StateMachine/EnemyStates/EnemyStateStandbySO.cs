using UnityEngine;
//EnemyStateStandbySO.cs �� �G���ҋ@�i�X�^���o�C�j���̏��


[CreateAssetMenu(fileName = "EnemyStateStandbySO", menuName = "State/EnemyState/EnemyStateStandbySO")]
public class EnemyStateStandbySO : EnemyStateBaseSO ///決めた場所にパトロール / 巡�?
{
    public override void EnterState()
    {

    }

    public override void UpdateState()
    {
        GameObject closestTarget = _logicController.CheckUncaughtTargetsInCone();
        if (closestTarget)
        {
            if(closestTarget) _logicController.CurrentTarget = closestTarget;
            _logicController.SetState(_logicController.DetectingStateInstance);
            return;
        }
        else
        {

        }
    }

    public override void DrawStateGizmo()
    {
        ConeHelper.DrawConeGizmo(_logicController.GetConeInfo());
    }

   
}
