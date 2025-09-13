using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AILogicController : MonoBehaviour
{
    #region Serialized
    [Header("何かをしたい、その候補ターゲット (複数) (尾行、など)")]
    [HeaderAttribute("がインスペクタで設定されない場合、自動的にStartで設定されます。")] [SerializeField] public List<GameObject> Targets; //TODO move this to singular data in gamemanager
    [Header("ターゲット中オブジェクト")]
    [SerializeField] public GameObject CurrentTarget; //ターゲット中オブジェクト
    [Header("捕獲ネットのスロット場所")]
    [SerializeField] public Transform CatchSlot;
    [SerializeField] public GameObject AlertMark; //"!!!" テキスト
    [SerializeField] public List<Transform> PatrolSpots;
    [HeaderAttribute("牢屋がインスペクタで設定されない場合、自動的にStartで設定されます。")] [SerializeField] public List<Jail> Jails; ///牢屋

    [Header("今の行動は")]
    [SerializeField] private EnemyStateBaseSO _currentState; public EnemyStateBaseSO CurrentState => _currentState;
    [SerializeField] public EnemyStateDetectingSO DetectingState;
    [SerializeField] public EnemyStateCarryCaughtSO CarryCaughtState;
    [SerializeField] public EnemyStateLoiterSO LoiterState;
    [SerializeField] public EnemyStatePatrolSO PatrolState;
    [SerializeField] public EnemyStateStunnedSO StunState;

    // private Cooldown _aiTick = new(0.2f); //毎フレームをチェックではなく、決めた時間にチェック
    [Header("視野関係")]
    [SerializeField] private float _maxConeDistance = 20.0f;
    [SerializeField] private float _coneAngle = 50.0f;
    #endregion

    [NonSerializedAttribute] public NavMeshAgent Agent;

    #region Unity
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        FindTargets();
        FindJails();
        SetState(PatrolState);
    }

    void FindJails()
    {
         if (Jails.Count == 0) //auto assign when not assigned manually.
        {
            Jail[] jails = FindObjectsByType<Jail>(FindObjectsSortMode.None);
            foreach (var jail in jails)
            {
                Jails.Add(jail);
            }
        }
    }
    void FindTargets()
    {
        if (Targets.Count == 0) //auto assign when not assigned manually.
        {
            PlayerInfo[] playerInfos = FindObjectsByType<PlayerInfo>(FindObjectsSortMode.None);
            foreach (var info in playerInfos)
            {
                Targets.Add(info.gameObject);
            }
        }
    }
    void Update()
    {
        if (!Agent) Debug.LogWarning("NavMeshAgentが持ってないオブジェクトです。");

        _currentState.UpdateState();
    }
    private void OnDrawGizmos()
    {
        if (_currentState)
        {
            _currentState.DrawStateGizmo();
        }
    }
    #endregion


    public void SetState(EnemyStateBaseSO newState)
    {
        //前のAIを終わらせる
        if (_currentState != null) _currentState.ExitState();

        //新しいAIがエンター
        newState.SetLogicController(this);
        newState.EnterState();

        //前のAIを上書き
        _currentState = newState;
    }

    public GameObject CheckUncaughtTargetsInCone() //捕まえらないものをチェック
    {
        Func<GameObject, bool> isIgnore = (obj) =>
        {
            var playerInfo = obj.GetComponent<PlayerInfo>();
            if (!playerInfo) Debug.LogWarning("This [" + obj.name + "] has no PlayerInfo!");

            if (playerInfo.hasCaught) return true; //Ignore if already caught.
            if (!IsOnSight(obj.transform.position)) return true; //Ignore if there is a wall

            return false; //Do not ignore. Process the check
        };



        return ConeHelper.CheckClosestTargetInCone //視野角に、チェック
      (
        GetConeInfo(),
        Targets,
        isIgnore //すでに捕まえたら、無視する
      );
    }
    public ConeInfo GetConeInfo()
    {
        ConeInfo coneInfo = new ConeInfo(
            transform.forward,
            transform.position,
            _maxConeDistance,
            _coneAngle
            );

        return coneInfo;
    }

    bool IsOnSight(Vector3 targetPos) //直線にいる、ものがないか？ (障害物がある？)
    {
        Vector3 dir = targetPos - transform.position;
        Ray ray = new Ray(transform.position, dir);
        float maxDist = _maxConeDistance;
        if (Physics.Raycast(ray, out RaycastHit hit, maxDist))
        {
            return hit.transform.position == targetPos; //true if only the first collider hit is target
        }

        return false;
    }
}