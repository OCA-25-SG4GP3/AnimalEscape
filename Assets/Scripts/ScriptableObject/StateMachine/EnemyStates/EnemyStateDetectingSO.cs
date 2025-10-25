using UnityEngine;

//EnemyStateDetectingSO.cs → 敵がプレイヤーを発見・探知中の状態


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
        //莉悶?ｮ蛟呵｣懊＠縺溘が繝悶ず繧ｧ繧ｯ繝医?ｮ荳ｭ縲√ｂ縺｣縺ｨ霑代＞繧ｿ繝ｼ繧ｲ繝?繝医′縺?繧後?ｰ縲√◎繧後ｒ莉翫?ｮ繧ｿ繝ｼ繧ｲ繝?繝医↓縺吶ｋ
        GameObject closerFoundObject = _logicController.CheckUncaughtTargetsInCone();

        if (closerFoundObject)
        {
            _logicController.CurrentTarget = closerFoundObject;
        }
        //////////////////////////////////

        if (_logicController.CurrentTarget && (IsTargetClose(maxChaseDistance) || infiniteDetectionRange))
        {
            SetChaseTargetPos(); //霑ｽ縺?縺九￠繧?
            if (IsWithinCatchRange(_logicController.CurrentTarget))///謐慕佐縺ｮ霍晞屬縺ｫ蜈･繧九°縺ｩ縺?縺?
            {
                Debug.Log("GAME OVER!");
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
        Gizmos.DrawWireSphere(center, radius); //繧ｿ繝ｼ繧ｲ繝?繝医′騾?縺偵ｋ霍晞屬

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