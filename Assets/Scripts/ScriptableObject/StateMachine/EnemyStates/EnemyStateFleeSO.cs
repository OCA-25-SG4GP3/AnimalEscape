using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateFleeSO", menuName = "State/EnemyState/EnemyStateFleeSO")]
public class EnemyStateFleeSO : EnemyStateBaseSO
{
    [SerializeField] private float minRandomFleePointInRadius = 15.0f;
    [SerializeField] private float maxRandomFleePointInRadius = 45.0f;
    [SerializeField][Header("Controls randomness in that cone direction")] private float fleeConeAngle = 35f;
    public override void EnterState()
    {
        GameObject fleeMiddlePoint = GetClosestFleeTarget();

        Vector3 fleePoint = AgentHelper.GetFleePointFromTarget(
        Owner.transform.position,
        fleeMiddlePoint.transform.position,
        minRandomFleePointInRadius,
        maxRandomFleePointInRadius,
        fleeConeAngle); //Run against target direction, but with tolerance of cone

        SetFleePoint(fleePoint);
    }

    GameObject GetClosestFleeTarget()
    {
        PlayerInfo[] players = GameObject.FindObjectsByType<PlayerInfo>(FindObjectsSortMode.None);
        if (players == null || players.Length == 0)
            return null;

        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (var p in players)
        {
            float dist = Vector3.Distance(Owner.transform.position, p.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = p.gameObject;
            }
        }

        return closest;
    }
    public override void UpdateState()
    {
        //if (!stunDur.IsCooldown)
        //{
        //    _logicController.SetState(previousState);
        //    return;
        //}

        //if (AgentHelper.HasArrivedSuccess(_logicController.Agent)) //return to loiter on escape success or hit a wall (reach the end but not success)
        if (_logicController.rbNavMesh.HasArrived()) //return to loiter on escape success or hit a wall (reach the end but not success)
        {
            _logicController.SetState(_logicController.LoiterState);
        }
    }

    public override void ExitState()
    {
    }

    void SetFleePoint(Vector3 fleePoint)
    {
        //AgentHelper.MoveTo(_logicController.Agent, fleePoint);
        _logicController.rbNavMesh.MoveTo(fleePoint);
    }

}