using UnityEngine;

//EnemyStateInfiniteChaseSO.cs ¨ “G‚ª–³ŒÀ‚É’ÇÕ‚ð‘±‚¯‚éó‘Ô


[CreateAssetMenu(fileName = "EnemyStateInfiniteChaseSO", menuName = "State/EnemyState/EnemyStateInfiniteChaseSO")]
public class EnemyStateInfiniteChaseSO : EnemyStateBaseSO
{
    [SerializeField] private float _catchRange = 3.5f;
    private Vector3 lastChaseTargetPos;

    public override void EnterState()
    {
        _logicController.AlertMark.SetActive(true);
        animator.SetBool("IsWalking", true);
    }

    public override void UpdateState()
    {
        // Find the closest uncaught target regardless of cone or distance
        GameObject closestTarget = FindClosestUncaughtTarget();
        if (closestTarget != null)
        {
            _logicController.CurrentTarget = closestTarget;
            SetChaseTargetPos();

            if (IsWithinCatchRange(closestTarget))
            {
                Debug.Log("‚Â‚©‚Ü‚Á‚½!");
                Destroy(closestTarget);
                // Handle catch logic
                return;
            }
        }
        else
        {
            // Optional: no targets in scene
            _logicController.CurrentTarget = null;
            _logicController.SetState(_logicController.LoiterStateInstance);
        }
    }

    public override void ExitState()
    {
        animator.SetBool("IsWalking", false);
        _logicController.AlertMark.SetActive(false);
        _logicController.rbNavMesh.ClearPath();
    }

    public override void DrawStateGizmo()
    {
        if (_logicController.CurrentTarget == null) return;

        Vector3 center = _logicController.transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, _catchRange);

        ConeHelper.DrawConeGizmo(_logicController.GetConeInfo());
    }

    private void SetChaseTargetPos()
    {
        if (_logicController.CurrentTarget == null) return;

        Vector3 targetPos = _logicController.CurrentTarget.transform.position;
        if ((targetPos - lastChaseTargetPos).sqrMagnitude > 0.1f)
        {
            _logicController.rbNavMesh.MoveTo(targetPos);
            lastChaseTargetPos = targetPos;
        }
    }

    private bool IsWithinCatchRange(GameObject objectToCheck)
    {
        return Vector3.Distance(objectToCheck.transform.position, _logicController.transform.position) <= _catchRange;
    }

    private GameObject FindClosestUncaughtTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player"); // or whatever targets
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var t in targets)
        {
            PlayerInfo info = t.GetComponent<PlayerInfo>();
            if (info != null && info.IsFallingDown()) continue; // skip falling players if needed

            float dist = Vector3.Distance(_logicController.transform.position, t.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }

        return closest;
    }
}
