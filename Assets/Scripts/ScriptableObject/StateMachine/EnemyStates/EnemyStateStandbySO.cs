using UnityEngine;
//EnemyStateStandbySO.cs → 敵が待機（スタンバイ）中の状態


[CreateAssetMenu(fileName = "EnemyStateStandbySO", menuName = "State/EnemyState/EnemyStateStandbySO")]
public class EnemyStateStandbySO : EnemyStateBaseSO ///豎ｺ繧√◆蝣ｴ謇縺ｫ繝代ヨ繝ｭ繝ｼ繝ｫ / 蟾｡蝗?
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
