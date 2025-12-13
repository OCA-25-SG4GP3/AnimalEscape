using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AILogicController : MonoBehaviour
{
    #region Serialized

    [SerializeField] public GameObject[] Targets; //TODO move this to singular data in gamemanager

    [SerializeField] public GameObject CurrentTarget; //ターゲ?��?ト中オブジェク?��?
    [SerializeField] public Transform CatchSlot; //Probably not needed anymore
    [SerializeField] public GameObject AlertMark; //"!!!" ?��?キス?��?
    [SerializeField] public List<Transform> PatrolSpots;
    Rigidbody rb;
    [SerializeField] public bool infiniteDetectionRange = false;

    public enum SelectedState
    {
        Empty, Standby, Detecting, Loiter, Patrol, Stun, Flee, InfiniteChase
    }

    [SerializeField][Header("開始行動")] SelectedState selectedState = SelectedState.Empty;

    [SerializeField] private EnemyStateBaseSO _currentState; public EnemyStateBaseSO CurrentState => _currentState;
    [SerializeField] public EnemyStateDetectingSO DetectingState;
    //[SerializeField] public EnemyStateCarryCaughtSO CarryCaughtState;
    [SerializeField] public EnemyStateStandbySO StandbyState;
    [SerializeField] public EnemyStateLoiterSO LoiterState;
    [SerializeField] public EnemyStatePatrolSO PatrolState;
    [SerializeField] public EnemyStateStunnedSO StunState;
    [SerializeField] public EnemyStateFleeSO FleeState;
    [SerializeField] public EnemyStateInfiniteChaseSO InfiniteChase;

    public EnemyStateStandbySO StandbyStateInstance;
    public EnemyStateDetectingSO DetectingStateInstance;
    //public EnemyStateCarryCaughtSO CarryCaughtStateInstance;
    public EnemyStateLoiterSO LoiterStateInstance;
    public EnemyStatePatrolSO PatrolStateInstance;
    public EnemyStateStunnedSO StunStateInstance;
    public EnemyStateFleeSO FleeStateInstance; //Will flee on the opposite direction from target (player), with a cone tolerance.
    public EnemyStateInfiniteChaseSO InfiniteChaseInstance;

    [SerializeField] private float _maxConeDistance = 20.0f;
    [SerializeField] private float _coneAngle = 50.0f;
    #endregion

    //public NavMeshAgent Agent; //We want rigidbody so we won't directly use this
    [NonSerializedAttribute] public RigidbodyNavMesh rbNavMesh;
    [SerializeField] private GameObject modelObj; public GameObject ModelObj => modelObj;

    #region Unity
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rbNavMesh = GetComponent<RigidbodyNavMesh>();

        StandbyStateInstance = Instantiate(StandbyState);
        DetectingStateInstance = Instantiate(DetectingState);
        //CarryCaughtStateInstance = Instantiate(CarryCaughtState);
        LoiterStateInstance = Instantiate(LoiterState);
        PatrolStateInstance = Instantiate(PatrolState);
        StunStateInstance = Instantiate(StunState);
        FleeStateInstance = Instantiate(FleeState);
        InfiniteChaseInstance = Instantiate(InfiniteChase);

        RefreshStateFromEnum();
        //ShowBones(ModelObj.transform, 0);

    }
    void ShowBones(Transform parent, int depth)
    {
        string indent = new string(' ', depth * 2);
        Debug.Log(indent + parent.name);
        foreach (Transform child in parent)
        { ShowBones(child, depth + 1); }
    }
    public void SetInfiniteDetectionRange(bool isEnabled)
    {
        infiniteDetectionRange = isEnabled;
        CurrentTarget = PlayerInfoSystem.GetAny().gameObject;
    }
    private void Start()
    {
        Targets = GameObject.FindGameObjectsWithTag("Player");
        //SetInfiniteDetectionRange(infiniteDetectionRange);
    }

    void Update()
    {
        if (_currentState) _currentState.UpdateState();

        Vector3 moveDir = rbNavMesh.GetNextDirection();
        RotateYTo(moveDir);
    }

    private void RotateYTo(Vector3 moveDir)
    {
        moveDir.y = 0f; // ignore vertical
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            modelObj.transform.rotation = Quaternion.Slerp(modelObj.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmos()
    {
        if (_currentState)
        {
            _currentState.DrawStateGizmo();
        }
    }
    #endregion

    public void SetStateByEnum(SelectedState newSelectedState)
    {
        selectedState = newSelectedState;
        RefreshStateFromEnum();
    }

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

    public void SetState(EnemyStateBaseSO newState)
    {
        //前�??��AIを終わらせ?��?
        if (_currentState != null) _currentState.ExitState();

        if (!newState) return;
        //新しいAIがエンター
        newState.SetLogicController(this);
        newState.EnterState();

        //前�??��AIを上書?��?
        _currentState = newState;
    }

    public GameObject CheckUncaughtTargetsInCone() //捕まえらな?��?も�??��をチェ?��?ク
    {
        Func<GameObject, bool> isIgnore = (obj) => //すでに牢屋に入ったら、チェ�?クしな�?�?
        {
            var playerInfo = obj.GetComponent<PlayerInfo>();
            if (!playerInfo) Debug.LogWarning("This [" + obj.name + "] has no PlayerInfo!");
            return playerInfo.hasCaught;
        };
        if (Targets.Length > 0)
            return ConeHelper.CheckClosestTargetInCone //視野角に、チェ?��?ク
          (
            GetConeInfo(),
            Targets,
            isIgnore //捕まえたも�??��を除外す?��?
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

    bool IsOnSight(Vector3 targetPos) //直線に?��?る、ものがな?��?か�? (障害物がある�?)
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
        if (collision.collider.CompareTag("Shot"))
        {
            Debug.Log(collision.collider);
            Destroy(gameObject);
        }
    }
}