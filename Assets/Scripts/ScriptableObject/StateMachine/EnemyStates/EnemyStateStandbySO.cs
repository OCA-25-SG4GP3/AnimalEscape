using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateStandbySO", menuName = "State/EnemyState/EnemyStateStandbySO")]
public class EnemyStateStandbySO : EnemyStateBaseSO ///決めた場所にパトロール / 巡�?
{
    public override void EnterState()
    {

    }

    public override void UpdateState()
    {
        GameObject closestTarget = _logicController.CheckUncaughtTargetsInCone();
        if (closestTarget || _logicController.infiniteDetectionRange)
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
