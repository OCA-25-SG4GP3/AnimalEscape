using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatePatrolSO", menuName = "State/EnemyState/EnemyStatePatrolSO")]
public class EnemyStatePatrolSO : EnemyStateBaseSO ///決めた場所にパトロール / 巡回
{
    private Transform _currentPatrolSpotT;
    private int _mode = 0;
    [Header("現在のパトロールインデックス")] private int _patrolIndex = 0;
    public override void EnterState()
    {
        GetClosestPatrolSpot(out int nextIndex); //最も近いパトロール場所に巡回し始める
        _patrolIndex = nextIndex;
    }

    public override void UpdateState()
    {
        GameObject closestTarget = _logicController.CheckUncaughtTargetsInCone();
        if (closestTarget)
        {
            _logicController.CurrentTarget = closestTarget;
            _logicController.SetState(_logicController.DetectingState);
            return;
        }
        else
        {
            if (_logicController.PatrolSpots.Count == 0) { return; }

            switch (_mode)
            {
                case 0: //Moving 
                    if (AgentHelper.HasArrivedSuccess(_logicController.Agent))
                    {
                        _mode = 1;
                    }
                    break;

                case 1://Recalculate next spot
                    ChangeToNextPatrol();
                    AgentHelper.MoveTo(_logicController.Agent, _currentPatrolSpotT.position);
                    _mode = 0;
                    break;
            }
        }
    }

    public override void DrawStateGizmo()
    {
        ConeHelper.DrawConeGizmo(_logicController.GetConeInfo());
        DrawPatrolLines();
    }

    Vector3 GetClosestPatrolSpot(out int nextIndex)
    {
        nextIndex = -1;
        float closestDist = float.MaxValue;
        Vector3 closestPos = Vector3.zero;

        Vector3 myPos = _logicController.transform.position;

        for (int i = 0; i < _logicController.PatrolSpots.Count; i++)
        {
            if (_logicController.PatrolSpots[i] == null)  //Sometimes not needed when debug | デバッグの時にたまに要らない
            { Debug.Log("パトロールSpotsがないです。インスペクターにつけてください。チェックを無視します。"); continue; }

            float dist = Vector3.SqrMagnitude(_logicController.PatrolSpots[i].position - myPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPos = _logicController.PatrolSpots[i].position;
                nextIndex = i;
            }
        }

        return closestPos;
    }

    void ChangeToNextPatrol()
    {
        _patrolIndex++;
        if (_patrolIndex > _logicController.PatrolSpots.Count - 1) _patrolIndex = 0;
        _currentPatrolSpotT = _logicController.PatrolSpots[_patrolIndex];
    }

    private void DrawPatrolLines()
    {
        if (_logicController.PatrolSpots == null || _logicController.PatrolSpots.Count == 0)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < _logicController.PatrolSpots.Count; i++)
        {
            Transform current = _logicController.PatrolSpots[i];
            Transform next = _logicController.PatrolSpots[(i + 1) % _logicController.PatrolSpots.Count]; // loops back to 0

            if (current != null && next != null)
            {
                Gizmos.DrawLine(current.position, next.position);
            }
        }
    }
}
