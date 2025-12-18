using UnityEngine;

//EnemyStateDetectingSO.cs 


[CreateAssetMenu(fileName = "EnemyStateDetectingSO", menuName = "State/EnemyState/EnemyStateDetectingSO")]
public class EnemyStateDetectingSO : EnemyStateBaseSO
{
    [SerializeField] private float _catchRange = 3.5f;
    [SerializeField] private float maxChaseDistance = 4.0f;
    bool infiniteDetectionRange = false;

    public override void EnterState()
    {
        _logicController.AlertMark.SetActive(true);
    }
    public void SetInfiniteDetectionRange(bool isEnabled)
    {
        infiniteDetectionRange = isEnabled;
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

        if (_logicController.CurrentTarget && (IsTargetClose(maxChaseDistance) || infiniteDetectionRange))
        {
            SetChaseTargetPos(); //追�?かけ�?
            if (IsWithinCatchRange(_logicController.CurrentTarget))///捕獲の距離に入るかど�?�?
            {
                ColorPanelRoomTimer colorPanelRoomTimer = FindAnyObjectByType<ColorPanelRoomTimer>();
                if (colorPanelRoomTimer) colorPanelRoomTimer.SetGameOverByOneCaught();
#if UNITY_EDITOR
                Debug.Log("GAME OVER!");
#endif
                //_logicController.SetState(_logicController.CarryCaughtStateInstance);
                // _logicController.CarryCaughtState.CatchObject(_logicController.currentTargetObj);
                return;
            }
        }
        else
        {
            _logicController.SetState(_logicController.LoiterStateInstance); //
            //SetAILogic(logicCon._aiLogicLoiter); //
        }
    }

    public override void ExitState()
    {
        _logicController.AlertMark.SetActive(false);
        //AgentHelper.ClearPath(_logicController.Agent); //Stop chasing after losing target
        _logicController.rbNavMesh.ClearPath();
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

    private Vector3 lastChaseTargetPos;

    private void SetChaseTargetPos()
    {
        Vector3 targetPos = _logicController.CurrentTarget.transform.position;

        // Only recalc path if target moved significantly
        if ((targetPos - lastChaseTargetPos).sqrMagnitude > 0.1f)
        {
            _logicController.rbNavMesh.MoveTo(targetPos);
            lastChaseTargetPos = targetPos;
        }
    }

    private bool IsTargetClose(float maxDistance)
    {
        float dist = Vector3.Distance(_logicController.transform.position, _logicController.CurrentTarget.transform.position);
        return dist <= maxDistance;
    }
}