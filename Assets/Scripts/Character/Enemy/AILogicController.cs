using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AILogicController : MonoBehaviour
{
    #region Serialized
    [Header("何かをしたい、その候補ターゲット (複数) (尾行、など)")]

    [HeaderAttribute("がインスペクタで設定されない場合、自動的にStartで設定されます。")][SerializeField] public GameObject[] Targets; //TODO move this to singular data in gamemanager
    [Header("ターゲ�?ト中オブジェク�?")]
    [SerializeField] public GameObject CurrentTarget; //ターゲ�?ト中オブジェク�?
    [Header("捕獲ネットのスロット場所")][SerializeField] public Transform CatchSlot;
    [SerializeField] public GameObject AlertMark; //"!!!" �?キス�?
    [SerializeField] public List<Transform> PatrolSpots;

       
     [HeaderAttribute("牢屋がインスペクタで設定されない場合、自動的にStartで設定されます。")] [SerializeField] public List<Jail> Jails; ///牢�?

    [Header("今�?�行動は")]
    [SerializeField] private EnemyStateBaseSO _currentState; public EnemyStateBaseSO CurrentState => _currentState;
    [SerializeField] public EnemyStateDetectingSO DetectingState;
    [SerializeField] public EnemyStateCarryCaughtSO CarryCaughtState;
    [SerializeField] public EnemyStateLoiterSO LoiterState;
    [SerializeField] public EnemyStatePatrolSO PatrolState;
    [SerializeField] public EnemyStateStunnedSO StunState;

    public EnemyStateDetectingSO DetectingStateInstance;
    public EnemyStateCarryCaughtSO CarryCaughtStateInstance;
    public EnemyStateLoiterSO LoiterStateInstance;
    public EnemyStatePatrolSO PatrolStateInstance;
    public EnemyStateStunnedSO StunStateInstance;

    // private Cooldown _aiTick = new(0.2f); //毎フレー�?をチェ�?クではなく、決めた時間にチェ�?ク
    [Header("視野関�?")]
    [SerializeField] private float _maxConeDistance = 20.0f;
    [SerializeField] private float _coneAngle = 50.0f;
    #endregion

    public NavMeshAgent Agent;

    #region Unity
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        DetectingStateInstance = Instantiate(DetectingState);
        CarryCaughtStateInstance = Instantiate(CarryCaughtState);
        LoiterStateInstance = Instantiate(LoiterState);
        PatrolStateInstance = Instantiate(PatrolState);
        StunStateInstance = Instantiate(StunState);
    }

    private void Start()
    {
        Targets = GameObject.FindGameObjectsWithTag("Player");
        SetState(LoiterStateInstance);
        var jailobjs = GameObject.FindGameObjectsWithTag("Jail");
        foreach (var jailobj in jailobjs)
        {
            Jails.Add(jailobj.GetComponent<Jail>());
        }
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
    [System.Obsolete("Already replaced in Start()")]
    void FindTargets()
    {
        //if (Targets.Count == 0) //auto assign when not assigned manually.
        //{
        //    PlayerInfo[] playerInfos = FindObjectsByType<PlayerInfo>(FindObjectsSortMode.None);
        //    foreach (var info in playerInfos)
        //    {
        //        Targets.Add(info.gameObject);
        //    }
        //}
    }
    void Update()
    {
        if (!Agent) Debug.LogWarning("NavMeshAgentが持ってな�?オブジェクトです�?");

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
        //前�?�AIを終わらせ�?
        if (_currentState != null) _currentState.ExitState();

        //新しいAIがエンター
        newState.SetLogicController(this);
        newState.EnterState();

        //前�?�AIを上書�?
        _currentState = newState;
    }

    public GameObject CheckUncaughtTargetsInCone() //捕まえらな�?も�?�をチェ�?ク
    {
        Func<GameObject, bool> isIgnore = (obj) => //すでに牢屋に入ったら、チェックしない。
        {
             var playerInfo = obj.GetComponent<PlayerInfo>();
             if (!playerInfo) Debug.LogWarning("This [" + obj.name + "] has no PlayerInfo!");
             return playerInfo.hasCaught;
        };
        if (Targets.Length > 0)
            return ConeHelper.CheckClosestTargetInCone //視野角に、チェ�?ク
          (
            GetConeInfo(),
            Targets,
            isIgnore //捕まえたも�?�を除外す�?
          );
        else
            return null;
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

    bool IsOnSight(Vector3 targetPos) //直線に�?る、ものがな�?か�? (障害物がある�?)
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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider);
        if (collision.collider.CompareTag("Shot"))
        {
            Destroy(gameObject);
        }
    }
}