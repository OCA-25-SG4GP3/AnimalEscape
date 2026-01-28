using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 敵AI：無限追跡状態
/// ターゲットを視野に関係なく永遠に追いかける状態
/// </summary>
[CreateAssetMenu(fileName = "EnemyStateInfiniteChaseSO", menuName = "State/EnemyState/EnemyStateInfiniteChaseSO")]
public class EnemyStateInfiniteChaseSO : EnemyStateBaseSO
{
    [SerializeField] private float _catchRange = 1.5f; // 捕獲可能距離
    private Vector3 lastChaseTargetPos; // 前回の追跡目標位置（パス更新の最適化用）
    ColorPanelRoomTimer colorPanelRoomTimer; // ゲームオーバー管理用タイマー

    #region 状態の開始・終了

    /// <summary>
    /// 無限追跡状態に入った時の初期化
    /// </summary>
    public override void EnterState()
    {
        // ゲームオーバー管理システムを取得
        colorPanelRoomTimer = GameObject.FindAnyObjectByType<ColorPanelRoomTimer>();

        // アニメーション設定：歩行開始
        animator.SetBool("IsWalking", true);

        // NavMeshエージェントを再開
        _logicController.rbNavMesh.Resume();
    }

    /// <summary>
    /// 無限追跡状態から抜ける時の処理
    /// </summary>
    public override void ExitState()
    {
        // すべてのアニメーションフラグをリセット
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsDiving", false);
        animator.SetBool("IsCatching", false);

        // 移動パスをクリア
        _logicController.rbNavMesh.Resume();
        _logicController.rbNavMesh.ClearPath();
    }

    #endregion

    #region メイン更新ループ

    private bool isCarrying = false; // 現在ターゲットを運搬中かどうか

    bool isDiving = false;
    /// <summary>
    /// 毎フレームの更新処理
    /// </summary>
    public override void UpdateState()
    {
        // 運搬中は追跡を停止
        if (isCarrying) return;

        // 最も近い未捕獲のターゲットを検索して設定
        GameObject closestTarget = FindClosestUncaughtTarget();
        if (closestTarget)
            _logicController.SetChaseTarget(closestTarget);

        // 有効な追跡ターゲットが存在する場合
        if (IsValidChaseTarget())
        {
            // ターゲットに向かって移動
            SetChaseTargetPos();

            // 捕獲範囲内に入った場合
            if (IsWithinCatchRange(closestTarget) && !isDiving)
            {
                HandleTargetCatch(closestTarget);
            }

        }
        else // ターゲットが存在しない場合
        {
#if UNITY_EDITOR
            Debug.Log("No Animal found");
#endif
            // ターゲットがいないので、うろうろ状態に移行
            _logicController.SetState(_logicController.LoiterStateInstance);
        }
    }

    #endregion

    #region 捕獲処理

    /// <summary>
    /// ターゲットを捕獲した時の処理
    /// </summary>
    private void HandleTargetCatch(GameObject target)
    {
        // 捕獲アニメーションを再生
        animator.SetBool("IsDiving", true);    // 飛び込みアニメーション

        // 移動を停止
        _logicController.rbNavMesh.Pause();
        _logicController.rbNavMesh.ClearPath();
        isDiving = true;

    }

    #endregion
    // クラスの上部に定数を追加
    [SerializeField] float CATCH_SPHERE_FINAL_RADIUS = 1.5f;//黄色
    private const float CATCH_MAX_DISTANCE = 3.0f;
    private const float CATCH_ORIGIN_HEIGHT = 0.5f;

    //アニメーションから呼ぶ
    public void TryCatchAnimal()
    {
        RaycastHit hit;
        Vector3 origin = Owner.transform.position + Vector3.up * CATCH_ORIGIN_HEIGHT;
        Vector3 direction = _logicController.ModelObj.transform.forward;

        Debug.DrawRay(origin, direction * CATCH_MAX_DISTANCE, Color.yellow, 0.5f);

        if (Physics.SphereCast(origin, CATCH_SPHERE_FINAL_RADIUS, direction, out hit, CATCH_MAX_DISTANCE))
        {
            GameObject target = null;
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Animal"))
            {
                target = hit.collider.gameObject;
                isCarrying = true;
                animator.SetBool("IsCatchingSuccess", true);

                target.GetComponent<AnimalControlSimple>().SetCaughtState();
                var catchComp = target.GetComponent<CatchPosition>();
                if (!catchComp) Debug.Log("CATCHがないです");
                catchComp.SetCatch(this);
                target.GetComponent<PlayerInfo>().SetCaught();

                if (colorPanelRoomTimer)
                    colorPanelRoomTimer.SetGameOverByOneCaught();
            }
        }
    }

    public void OnDrawGizmos()
    {
        Vector3 origin = Owner.transform.position + Vector3.up * CATCH_ORIGIN_HEIGHT;
        Vector3 direction = _logicController.ModelObj.transform.forward;

        // 開始位置の球

        // 終了位置の球
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin + direction * CATCH_MAX_DISTANCE, CATCH_SPHERE_FINAL_RADIUS);

        // 中心線
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + direction * CATCH_MAX_DISTANCE);
    }

    #region ターゲット検出と判定

    /// <summary>
    /// 現在の追跡ターゲットが有効かチェック
    /// </summary>
    private bool IsValidChaseTarget()
    {
        return _logicController.CurrentTarget != null &&
               _logicController.CurrentTarget.activeSelf;
    }

    /// <summary>
    /// ターゲットが捕獲範囲内にいるかチェック
    /// </summary>
    private bool IsWithinCatchRange(GameObject objectToCheck)
    {
        return Vector3.Distance(
            objectToCheck.transform.position,
            _logicController.transform.position
        ) <= _catchRange;
    }

    /// <summary>
    /// 最も近い未捕獲のターゲットを検索
    /// 他の敵のターゲット状況は考慮しない
    /// </summary>
    private GameObject FindClosestUncaughtTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        GameObject closest = null;
        float minDist = float.MaxValue;

        foreach (var t in targets)
        {
            PlayerInfo info = t.GetComponent<PlayerInfo>();
            // 必要に応じて倒れているプレイヤーをスキップ
            // if (info != null && info.IsFallingDown()) continue;

            float dist = Vector3.Distance(_logicController.transform.position, t.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = t;
            }
        }

        return closest;
    }

    /// <summary>
    /// 他の敵にターゲットされていない、最も近いプレイヤーを検索
    /// ※現在は未使用
    /// </summary>
    private GameObject FindClosestUnTargetedTarget()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        AILogicController[] allEnemies = Object.FindObjectsByType<AILogicController>(FindObjectsSortMode.None);

        GameObject closestUntargeted = null;
        float minDist = float.MaxValue;

        foreach (var player in allPlayers)
        {
            PlayerInfo info = player.GetComponent<PlayerInfo>();

            // 既に捕獲されたプレイヤーをスキップ
            if (info != null && info.hasCaught) continue;

            // 倒れているプレイヤーをスキップ（オプション）
            // if (info != null && info.IsFallingDown()) continue;

            // このプレイヤーが他の敵にターゲットされているかチェック
            bool isTargetedByOther = false;
            foreach (var enemy in allEnemies)
            {
                // 自分自身はスキップ
                if (enemy == _logicController) continue;

                // 他の敵がこのプレイヤーをターゲット中ならスキップ
                if (enemy.CurrentTarget == player)
                {
                    isTargetedByOther = true;
                    break;
                }
            }

            if (isTargetedByOther) continue;

            // ターゲットされていない最も近いプレイヤーを見つける
            float dist = Vector3.Distance(_logicController.transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestUntargeted = player;
            }
        }

        return closestUntargeted;
    }

    #endregion

    #region 移動制御

    /// <summary>
    /// 追跡ターゲットの位置に向かって移動
    /// 位置が大きく変わった時のみパスを更新（最適化）
    /// </summary>
    private void SetChaseTargetPos()
    {
        if (_logicController.CurrentTarget == null) return;

        Vector3 targetPos = _logicController.CurrentTarget.transform.position;

        // ターゲットが前回から0.1m以上移動していたらパスを更新
        if ((targetPos - lastChaseTargetPos).sqrMagnitude > 0.1f)
        {
            _logicController.rbNavMesh.MoveTo(targetPos);
            lastChaseTargetPos = targetPos;
        }
    }

    #endregion

    #region ゲームオーバー判定（未使用）

    /// <summary>
    /// 全プレイヤーが捕獲されたかチェックしてゲームオーバー
    /// ※現在はコメントアウトされている
    /// </summary>
    private void TryGameOver()
    {
        PlayerDistanceManager playerDistanceManager = GameObject.FindAnyObjectByType<PlayerDistanceManager>();
        if (!playerDistanceManager.HaveAllPlayersCaught()) return;

        colorPanelRoomTimer.SetGameOverByAllCaught();
    }

    #endregion

    #region デバッグ表示

    /// <summary>
    /// エディタ上でギズモを描画（捕獲範囲と視野角）
    /// </summary>
    public override void DrawStateGizmo()
    {
        if (_logicController.CurrentTarget == null) return;

        // 捕獲範囲を赤い円で表示
        Vector3 center = _logicController.transform.position;
        Vector3 direction = _logicController.ModelObj.transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(direction * CATCH_MAX_DISTANCE, _catchRange);

        // 視野角を表示
        ConeHelper.DrawConeGizmo(_logicController.GetConeInfo());
    }

    #endregion
}
