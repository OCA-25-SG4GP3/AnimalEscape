using UnityEngine;

//EnemyStateInfiniteChaseSO.cs 


[CreateAssetMenu(fileName = "EnemyStateInfiniteChaseSO", menuName = "State/EnemyState/EnemyStateInfiniteChaseSO")]
public class EnemyStateInfiniteChaseSO : EnemyStateBaseSO
{
    [SerializeField] private float _catchRange = 3.5f;
    private Vector3 lastChaseTargetPos;
    ColorPanelRoomTimer colorPanelRoomTimer;
    public override void EnterState()
    {
        colorPanelRoomTimer = GameObject.FindAnyObjectByType<ColorPanelRoomTimer>();
        _logicController.AlertMark.SetActive(true);
        animator.SetBool("IsWalking", true);
        _logicController.rbNavMesh.Resume();

        //  _logicController.CurrentTarget = FindClosestUnTargetedTarget(); //Obtain once
    }
    bool isCarrying = false; //運んでいますか
    public override void UpdateState()
    {
        if (isCarrying) return;

        // Find the closest uncaught target and set it as the chase target
        GameObject closestTarget = FindClosestUncaughtTarget(); //Always check for closest one.
        if (closestTarget) _logicController.SetChaseTarget(closestTarget);

        if (IsValidChaseTarget()) //ターゲットが存在する
        {
            //GameObject untargetedTarget = _logicController.CurrentTarget;

            SetChaseTargetPos();
            if (IsWithinCatchRange(closestTarget))
            {
                isCarrying = true;
                //For now we us both because we dont have miss
                animator.SetBool("IsDiving", true); //今回はまだスキップする。
                animator.SetBool("IsCatching", true); //TODO move this to Caught State for better animation flow
                closestTarget.GetComponent<AnimalControlSimple>().SetCaughtState();
                closestTarget.GetComponent<CatchPosition>().SetCatch(this);
                _logicController.rbNavMesh.ClearPath();

                closestTarget.GetComponent<PlayerInfo>().SetCaught();

                ColorPanelRoomTimer colorPanelRoomTimer = FindAnyObjectByType<ColorPanelRoomTimer>();
                if (colorPanelRoomTimer) colorPanelRoomTimer.SetGameOverByOneCaught();

                //TryGameOver();

                //_logicController.SetState(_logicController.LoiterStateInstance);

                //Destroy(closestTarget);
                // Handle catch logic
                return;
            }
        }
        else //ターゲットそもそも存在しない
        {
#if UNITY_EDITOR
            Debug.Log("No Animal found");
#endif
            // Optional: no targets in scene
            // Find the closest uncaught target regardless of cone or distance
            //GameObject closestTarget = FindClosestUncaughtTarget();
            //if (IsValidChaseTarget()) _logicController.CurrentTarget = closestTarget;
            //else _logicController.SetState(_logicController.LoiterStateInstance);
            _logicController.SetState(_logicController.LoiterStateInstance);
        }
    }

    private bool IsValidChaseTarget()
    {
        return _logicController.CurrentTarget != null && _logicController.CurrentTarget.activeSelf; //Because When Finish, !activeSelf
    }

    private void TryGameOver()
    {
        PlayerDistanceManager playerDistanceManager = GameObject.FindAnyObjectByType<PlayerDistanceManager>();
        if (!playerDistanceManager.HaveAllPlayersCaught()) return;

        colorPanelRoomTimer.SetGameOverByAllCaught();
    }

    public override void ExitState()
    {
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsDiving", false);
        animator.SetBool("IsCatching", false);
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
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var t in targets)
        {
            PlayerInfo info = t.GetComponent<PlayerInfo>();
            //if (info != null && info.IsFallingDown()) continue;

            float dist = Vector3.Distance(_logicController.transform.position, t.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }

        return closest;
    }

    private GameObject FindClosestUnTargetedTarget()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        AILogicController[] allEnemies = Object.FindObjectsByType<AILogicController>(FindObjectsSortMode.None);

        GameObject closestUntargeted = null;
        float minDist = float.MaxValue;

        foreach (var player in allPlayers)
        {
            PlayerInfo info = player.GetComponent<PlayerInfo>();

            // Skip caught players
            if (info != null && info.hasCaught) continue;

            // Skip falling players
            //if (info != null && info.IsFallingDown()) continue;

            // Check if this player is already targeted by another enemy
            bool isTargetedByOther = false;
            foreach (var enemy in allEnemies)
            {
                // Skip checking this enemy (self)
                if (enemy == _logicController) continue;

                // If another enemy is targeting this player, skip
                if (enemy.CurrentTarget == player)
                {
                    isTargetedByOther = true;
                    break;
                }
            }

            if (isTargetedByOther) continue;

            // Find the closest untargeted player
            float dist = Vector3.Distance(_logicController.transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestUntargeted = player;
            }
        }

        return closestUntargeted;
    }
}
