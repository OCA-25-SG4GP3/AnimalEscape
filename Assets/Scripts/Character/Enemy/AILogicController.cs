using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵AIの状態管理と行動制御を行うコントローラー
/// </summary>
public class AILogicController : MonoBehaviour
{
    [SerializeField] public GameObject[] Targets; // 検出対象（プレイヤー）の配列 TODO: GameManagerに移動予定

    [SerializeField] public GameObject CurrentTarget; // 現在追跡中のターゲット
    [SerializeField] public Transform CatchSlot; // 捕獲時のスロット（おそらく不要）
    [SerializeField] public List<Transform> PatrolSpots; // 巡回ポイントのリスト
    Rigidbody rb; // Rigidbodyコンポーネント

    /// <summary>
    /// AIの状態タイプ
    /// </summary>
    public enum SelectedState
    {
        Empty,          // 空（状態なし）
        Standby,        // 待機
        Detecting,      // 検知・追跡
        Loiter,         // うろうろ
        Patrol,         // 巡回
        Stun,           // スタン（気絶）
        Flee,           // 逃走
        InfiniteChase   // 無限追跡
    }

    [SerializeField][Header("開始行動")] SelectedState selectedState = SelectedState.Empty;

    // 現在の状態
    [SerializeField] private EnemyStateBaseSO _currentState;
    public EnemyStateBaseSO CurrentState => _currentState;

    // 各状態のScriptableObjectテンプレート
    [SerializeField] public EnemyStateDetectingSO DetectingState;
    [SerializeField] public EnemyStateStandbySO StandbyState;
    [SerializeField] public EnemyStateLoiterSO LoiterState;
    [SerializeField] public EnemyStatePatrolSO PatrolState;
    [SerializeField] public EnemyStateStunnedSO StunState;
    [SerializeField] public EnemyStateFleeSO FleeState;
    [SerializeField] public EnemyStateInfiniteChaseSO InfiniteChase;

    // 各状態の実行時インスタンス
    public EnemyStateStandbySO StandbyStateInstance;
    public EnemyStateDetectingSO DetectingStateInstance;
    public EnemyStateLoiterSO LoiterStateInstance;
    public EnemyStatePatrolSO PatrolStateInstance;
    public EnemyStateStunnedSO StunStateInstance;
    public EnemyStateFleeSO FleeStateInstance; // ターゲットの反対方向に逃走
    public EnemyStateInfiniteChaseSO InfiniteChaseInstance;

    // 視野設定
    [SerializeField] private float _maxConeDistance = 20.0f; // 視野距離
    [SerializeField] private float _coneAngle = 50.0f;       // 視野角度

    // NavMeshとRigidbodyを組み合わせたコンポーネント
    [NonSerializedAttribute] public RigidbodyNavMesh rbNavMesh;
    [SerializeField] private GameObject modelObj; // 敵のモデルオブジェクト
    public GameObject ModelObj => modelObj;

    #region Unity ライフサイクル

    private void Awake()
    {
        // コンポーネント取得
        rb = GetComponent<Rigidbody>();
        rbNavMesh = GetComponent<RigidbodyNavMesh>();

        // 各状態のインスタンスを生成
        StandbyStateInstance = Instantiate(StandbyState);
        DetectingStateInstance = Instantiate(DetectingState);
        LoiterStateInstance = Instantiate(LoiterState);
        PatrolStateInstance = Instantiate(PatrolState);
        StunStateInstance = Instantiate(StunState);
        FleeStateInstance = Instantiate(FleeState);
        InfiniteChaseInstance = Instantiate(InfiniteChase);

        // 初期状態を設定
        RefreshStateFromEnum();
    }

    /// <summary>
    /// 追跡ターゲットを設定
    /// </summary>
    public void SetChaseTarget(GameObject target)
    {
        CurrentTarget = target;
    }

    /// <summary>
    /// デバッグ用：オブジェクトの階層構造を表示
    /// </summary>
    void ShowBones(Transform parent, int depth)
    {
        string indent = new string(' ', depth * 2);
        Debug.Log(indent + parent.name);
        foreach (Transform child in parent)
        {
            ShowBones(child, depth + 1);
        }
    }

    private void Start()
    {
        // シーン内のプレイヤーを全て取得
        Targets = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        // 現在の状態を更新
        if (_currentState)
            _currentState.UpdateState();

        // NavMeshから次の移動方向を取得し、その方向を向く
        Vector3 moveDir = rbNavMesh.GetNextDirection();
        RotateYTo(moveDir);
    }

    /// <summary>
    /// Y軸回転で指定方向を向く
    /// </summary>
    private void RotateYTo(Vector3 moveDir)
    {
        moveDir.y = 0f; // 垂直成分は無視
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            modelObj.transform.rotation = Quaternion.Slerp(
                modelObj.transform.rotation,
                targetRot,
                Time.deltaTime * 5f
            );
        }
    }

    /// <summary>
    /// エディタ上でギズモを描画
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_currentState)
        {
            _currentState.DrawStateGizmo();

        EnemyStateInfiniteChaseSO infiniteState = _currentState as EnemyStateInfiniteChaseSO;
        infiniteState.OnDrawGizmos();
        }
    }

    #endregion

    #region 状態管理

    /// <summary>
    /// 列挙型から状態を設定
    /// </summary>
    public void SetStateByEnum(SelectedState newSelectedState)
    {
        selectedState = newSelectedState;
        RefreshStateFromEnum();
    }

    /// <summary>
    /// 選択された列挙型に応じて実際の状態を設定
    /// </summary>
    private void RefreshStateFromEnum()
    {
        switch (selectedState)
        {
            case SelectedState.Empty:
                SetState(null);
                break;
            case SelectedState.Standby:
                SetState(StandbyStateInstance);
                break;
            case SelectedState.Detecting:
                SetState(DetectingStateInstance);
                break;
            case SelectedState.Loiter:
                SetState(LoiterStateInstance);
                break;
            case SelectedState.Patrol:
                SetState(PatrolStateInstance);
                break;
            case SelectedState.Stun:
                SetState(StunStateInstance);
                break;
            case SelectedState.Flee:
                SetState(FleeStateInstance);
                break;
            case SelectedState.InfiniteChase:
                SetState(InfiniteChaseInstance);
                break;
        }
    }

    /// <summary>
    /// 状態を切り替える
    /// </summary>
    public void SetState(EnemyStateBaseSO newState)
    {
        // 現在の状態を終了
        if (_currentState != null)
            _currentState.ExitState();

        if (!newState) return;

        // 新しい状態を開始
        newState.SetLogicController(this);
        newState.EnterState();

        // 現在の状態を更新
        _currentState = newState;
    }

    #endregion

    #region ターゲット検出

    /// <summary>
    /// 視野角内にいる、まだ捕まっていないターゲットを検出
    /// </summary>
    public GameObject CheckUncaughtTargetsInCone()
    {
        // 既に捕まっているプレイヤーを除外する条件
        Func<GameObject, bool> isIgnore = (obj) =>
        {
            var playerInfo = obj.GetComponent<PlayerInfo>();
            if (!playerInfo)
                Debug.LogWarning("This [" + obj.name + "] has no PlayerInfo!");
            return playerInfo.hasCaught;
        };

        if (Targets.Length > 0)
        {
            // 視野角内で最も近い未捕獲のターゲットを返す
            return ConeHelper.CheckClosestTargetInCone(
                GetConeInfo(),
                Targets,
                isIgnore
            );
        }
        else
            return null;
    }

    /// <summary>
    /// 視野情報を取得
    /// </summary>
    public ConeInfo GetConeInfo()
    {
        ConeInfo coneInfo = new ConeInfo(
            transform.forward,      // 視線方向
            transform.position,     // 視点位置
            _maxConeDistance,       // 視野距離
            _coneAngle              // 視野角
        );

        return coneInfo;
    }

    /// <summary>
    /// ターゲットが視界内にいるか（障害物チェック）
    /// </summary>
    bool IsOnSight(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        Ray ray = new Ray(transform.position, dir);
        float maxDist = _maxConeDistance;

        if (Physics.Raycast(ray, out RaycastHit hit, maxDist))
        {
            // 最初に当たったコライダーがターゲット自身ならtrue
            return hit.transform.position == targetPos;
        }

        return false;
    }

    #endregion

    #region 衝突処理

    /// <summary>
    /// ショット（弾）に当たったら破壊される
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Shot"))
        {
            Debug.Log(collision.collider);
            Destroy(gameObject);
        }
    }

    #endregion
}