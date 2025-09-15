using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateDetectingSO", menuName = "State/EnemyState/EnemyStateDetectingSO")]
public class EnemyStateDetectingSO : EnemyStateBaseSO
{
    [SerializeField] private float _catchRange = 3.5f; //遠すぎたら、辞める。徘徊に戻す
    [SerializeField] private float maxChaseDistance = 10.0f; //遠すぎたら、辞める。徘徊に戻す

    public override void EnterState()
    {
        _logicController.AlertMark.SetActive(true);
    }

    public override void UpdateState()
    {
        //////////////////////////////////
        //他の候補したオブジェクトの中、もっと近いターゲットがいれば、それを今のターゲットにする
        GameObject closerFoundObject = _logicController.CheckUncaughtTargetsInCone();

        if (closerFoundObject)
        {
            _logicController.CurrentTarget = closerFoundObject;
        }
        //////////////////////////////////

        if (_logicController.CurrentTarget && IsTargetClose(maxChaseDistance))
        {
            ChaseTarget(); //追いかける
            if (IsWithinCatchRange(_logicController.CurrentTarget))///捕獲の距離に入るかどうか
            {
                _logicController.SetState(_logicController.CarryCaughtStateInstance);
                // _logicController.CarryCaughtState.CatchObject(_logicController.currentTargetObj);
                return;
            }
        }
        else
        {
            _logicController.SetState(_logicController.LoiterStateInstance); //やめる。また巡回する。
            //SetAILogic(logicCon._aiLogicLoiter); //やめる。徘徊する。
        }
    }

    public override void ExitState()
    {
        _logicController.AlertMark.SetActive(false);
        AgentHelper.ClearPath(_logicController.Agent); //Stop chasing after losing target
    }

    public override void DrawStateGizmo()
    {
        if (_logicController.CurrentTarget == null) return;

        Vector3 center = _logicController.transform.position;
        float radius = maxChaseDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius); //ターゲットが逃げる距離

        ConeHelper.DrawConeGizmo(_logicController.GetConeInfo());
    }


    bool IsWithinCatchRange(GameObject objectToCheck)
    {
        return Vector3.Distance(objectToCheck.transform.position, _logicController.transform.position) <= _catchRange;
    }

    private void ChaseTarget()
    {
        Vector3 targetPos = _logicController.CurrentTarget.transform.position;
        AgentHelper.MoveTo(_logicController.Agent, targetPos);
    }

    private bool IsTargetClose(float maxDistance)
    {
        float dist = Vector3.Distance(_logicController.transform.position, _logicController.CurrentTarget.transform.position);
        return dist <= maxDistance;
    }
}