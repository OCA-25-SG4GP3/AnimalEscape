using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateDetectingSO", menuName = "State/EnemyState/EnemyStateDetectingSO")]
public class EnemyStateDetectingSO : EnemyStateBaseSO
{
    [SerializeField] private float _catchRange = 3.5f; //�?すぎたら、辞める。徘徊に戻�?
    [SerializeField] private float maxChaseDistance = 10.0f; //�?すぎたら、辞める。徘徊に戻�?

    public override void EnterState()
    {
        _logicController.AlertMark.SetActive(true);
    }

    public override void UpdateState()
    {
        //////////////////////////////////
        //他�?�候補したオブジェクト�?�中、もっと近いターゲ�?トが�?れ�?�、それを今�?�ターゲ�?トにする
        GameObject closerFoundObject = _logicController.CheckUncaughtTargetsInCone();

        if (closerFoundObject)
        {
            _logicController.CurrentTarget = closerFoundObject;
        }
        //////////////////////////////////

        if (_logicController.CurrentTarget && IsTargetClose(maxChaseDistance))
        {
            ChaseTarget(); //追�?かけ�?
            if (IsWithinCatchRange(_logicController.CurrentTarget))///捕獲の距離に入るかど�?�?
            {
                Debug.Log("GAME OVER!");
                //_logicController.SetState(_logicController.CarryCaughtStateInstance);
                // _logicController.CarryCaughtState.CatchObject(_logicController.currentTargetObj);
                return;
            }
        }
        else
        {
            _logicController.SetState(_logicController.LoiterStateInstance); //�?める。また巡回する�?
            //SetAILogic(logicCon._aiLogicLoiter); //�?める。徘徊する�?
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
        Gizmos.DrawWireSphere(center, radius); //ターゲ�?トが�?げる距離

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