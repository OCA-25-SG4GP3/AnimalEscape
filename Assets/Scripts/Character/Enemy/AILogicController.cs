using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AILogicController : MonoBehaviour
{
    [Header("何かをしたい、その候補ターゲット (複数) (尾行、など)")]
    [SerializeField] public List<GameObject> Targets; //TODO move this to singular data in gamemanager
    [Header("ターゲット中オブジェクト")]
    [SerializeField] public GameObject CurrentTarget; //ターゲット中オブジェクト
    [Header("捕獲ネットのスロット場所")]
    [SerializeField] public Transform CatchSlot;
    [SerializeField] public GameObject AlertMark; //"!!!" テキスト
    [SerializeField] public List<Transform> PatrolSpots;
    [SerializeField] public List<Jail> Jails; ///牢屋
    public NavMeshAgent Agent;

    [Header("今の行動は")]
    [SerializeField] private EnemyStateBaseSO _currentState; 
    [SerializeField] public EnemyStateDetectingSO DetectingState;
    [SerializeField] public EnemyStateCarryCaughtSO CarryCaughtState;
    [SerializeField] public EnemyStateLoiterSO LoiterState;
    [SerializeField] public EnemyStatePatrolSO PatrolState;

    // private Cooldown _aiTick = new(0.2f); //毎フレームをチェックではなく、決めた時間にチェック
    [Header("視野関係")]
    [SerializeField] private float _maxConeDistance = 20.0f;
    [SerializeField] private float _coneAngle = 50.0f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetState(PatrolState);
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

    public GameObject CheckUncaughtTargetsInCone() //捕まえてないものをチェック
    {
        Func<GameObject, bool> hasCaught = (obj) => { return obj.GetComponent<PlayerInfo>().hasCaught; };
        return ConeHelper.CheckClosestTargetInCone //視野角に、チェック
      (
        GetConeInfo(),
        Targets,
        hasCaught //捕まえたものを除外する
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
}