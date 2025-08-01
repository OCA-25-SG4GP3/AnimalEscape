using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AILogicController : MonoBehaviour
{
    [SerializeField][Header("何かをしたい、その候補ターゲット (複数) (尾行、など)")] public List<GameObject> targetCandidateObjects; //TODO move this to singular data in gamemanager
    [SerializeField][Header("ターゲット中オブジェクト")] public GameObject currentTargetObj; //ターゲット中オブジェクト
    [SerializeReference][Header("今の行動は")] public AILogic aiLogic; //行動パターン抽象データー
    [SerializeField] public AILogicLoiter aiLogicLoiter = new(); //Inspectorから設定したい場合
    [SerializeField] public AILogicDetecting aiLogicDetecting = new(); //Inspectorから設定したい場合
    [SerializeField] public AILogicCarryCatched aiLogicCarryCatched = new(); //Inspectorから設定したい場合
    [SerializeField] public AILogicPatrol aiLogicPatrol = new(); //Inspectorから設定したい場合
    [NonSerialized] public NavMeshAgent agent;
    Cooldown aiTick = new(0.2f); //毎フレームをチェックではなく、決めた時間にチェック
    [SerializeField] private float maxConeDistance = 20.0f;
    [SerializeField] private float coneAngle = 50.0f;
    #region unity
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetAILogic(aiLogicPatrol); //初期化モードはパトロール
        //SetAILogic(aiLogicLoiter); //初期化モードは徘徊
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!agent) Debug.LogWarning("NavMeshAgentが持ってないオブジェクトです。");
#endif
        aiLogic.Update();
        if (!aiTick.IsCooldown)
        {
            aiLogic.Action();
            aiTick.StartCooldown();
        }
    }
    private void OnDrawGizmos()
    {
        if (aiLogic != null)
            aiLogic.DrawDebug();
    }

    #endregion
    #region public
    public void SetAILogic(AILogic newAILogic)
    {
        //前のAIを終わらせる
        if (aiLogic != null) aiLogic.Exit();

        //新しいAIがエンター
        newAILogic.SetLogicController(this);
        newAILogic.Enter();

        //前のAIを上書き
        aiLogic = newAILogic;
    }

    public GameObject CheckUncaughtTargetsInCone() //捕まえてないものをチェック
    {
        Func<GameObject, bool> hasCaught = (obj) => { return obj.GetComponent<PlayerInfo>().hasCaught; };
        return ConeHelper.CheckClosestTargetInCone //視野角に、チェック
      (
        GetConeInfo(),
        targetCandidateObjects,
        hasCaught //捕まえたものを除外する
      );
    }
    public ConeInfo GetConeInfo()
    {
        ConeInfo coneInfo = new ConeInfo(
            transform.forward,
            transform.position,
            maxConeDistance,
            coneAngle
            );

        return coneInfo;
    }


    #endregion

}
#region AILogic

[System.Serializable]
public abstract class AILogic
///We want this class to be loosely coupled
///何のゲームオブジェクトが実行できるように、依存性を少なくにします。
{
    protected AILogic(string name)
    {
        logicName = name;
    } //for debug
    protected AILogicController logicCon = null; //set on child
    protected NavMeshAgent agent => logicCon.agent;
    [SerializeField, ReadOnly] private string logicName; //Debug display
    public void SetLogicController(AILogicController aiLogicController)
    {
        this.logicCon = aiLogicController;
    }
    protected void SetLogicName(string name) => logicName = name;
    public abstract void Enter(); ///can change animation here
    public abstract void Action(); ///Main loop per cooldown tick
    public abstract void Exit();
    public virtual void Update() { } ///Frequent call
    public virtual void DrawDebug() { }// optional override in child classes
    public void SetAILogic(AILogic aiLogic) => logicCon.SetAILogic(aiLogic);
    public bool FoundTarget()
    {
        return logicCon.currentTargetObj != null;
    }
}
[System.Serializable]
public class AILogicCarryCatched : AILogic ///検知する / 追いかける / 尾行 Chase
{
    public AILogicCarryCatched() : base("AI has caught an object") { }
    [SerializeField, ReadOnly][Header("捕まえたオブジェクト")] private GameObject caughtObject;
    [SerializeField][Header("捕獲ネットのスロット場所")] private Transform catchNetSlotT;
    [SerializeField] private List<Jail> jails; ///牢屋
    Vector3 dropPos;

    public void CatchObject(GameObject objectToCatch)
    {
        caughtObject = objectToCatch;
        objectToCatch.GetComponent<PlayerInfo>().hasCaught = true;
    }
    public override void Enter()
    {
        MoveToDropInClosestJail();
    }
    public override void Update()
    {
        UpdateCatchedObjectPosRot();
    }
    public override void Action()
    {
        const float jailCellSize = 2.8f;
        if (AgentHelper.HasArrivedSuccess(agent, jailCellSize)) //牢屋の近くに到着
        {
            DropCatchedObject(dropPos);
            SetAILogic(logicCon.aiLogicPatrol); //restore
        }
    }
    Jail GetClosestJail()
    {
        List<Vector3> jailPositions = jails.Select(obj => obj.transform.position).ToList();
        Vector3 closestJailPos = Vector3Helper.GetClosest(logicCon.transform.position, jailPositions, out int index);
        return jails[index];
    }
    void MoveToDropInClosestJail()
    {
        Jail closestJail = GetClosestJail(); //store for dropping later to prevent accidents
        dropPos = closestJail.jailedObjectSlotT.position;
        AgentHelper.MoveTo(agent, dropPos);
    }
    void DropCatchedObject(Vector3 dropPos)
    {
        caughtObject.transform.position = dropPos;
        caughtObject = null;
    }
    void UpdateCatchedObjectPosRot()
    {
        caughtObject.transform.position = catchNetSlotT.position;
        caughtObject.transform.rotation = catchNetSlotT.rotation;
    }

    public override void Exit()
    {
        AgentHelper.ClearPath(agent);
        if (caughtObject) DropCatchedObject(logicCon.transform.position);
    }
}
[System.Serializable]
public class AILogicDetecting : AILogic ///検知する / 追いかける / 尾行 Chase
{
    [SerializeField] private GameObject exclamationMarkTextObj; //"!!!" テキスト
    [SerializeField] private float maxChaseDistance = 25.0f; //遠すぎたら、辞める。徘徊に戻す
    [SerializeField] private float catchRange = 3.5f; //遠すぎたら、辞める。徘徊に戻す
    public AILogicDetecting() : base("AI is Detecting 検知した") { }

    public override void Enter()
    {
        exclamationMarkTextObj.SetActive(true);
        //スプリント anim
    }
    public override void Action()
    {
        //////////////////////////////////
        //他の候補したオブジェクトの中、もっと近いターゲットがいれば、それを今のターゲットにする
        GameObject closerFoundObject = logicCon.CheckUncaughtTargetsInCone();

        if (closerFoundObject)
        {
#if UNITY_EDITOR
            // Debug.Log(
            //     logicCon.currentTargetObj.name + "　の尾行をやめて、" +
            //      closerFoundObject.name + "　を尾行してます！"
            //     );
#endif
            logicCon.currentTargetObj = closerFoundObject;
        }
        //////////////////////////////////

        if (logicCon.currentTargetObj && IsTargetClose(maxChaseDistance))
        {
            ChaseTarget(); //追いかける
            if (IsWithinCatchRange(logicCon.currentTargetObj))///捕獲の距離に入るかどうか
            {
                SetAILogic(logicCon.aiLogicCarryCatched);
                logicCon.aiLogicCarryCatched.CatchObject(logicCon.currentTargetObj);
                return;
            }
        }
        else
        {
            SetAILogic(logicCon.aiLogicPatrol); //やめる。また巡回する。
            //SetAILogic(logicCon.aiLogicLoiter); //やめる。徘徊する。
        }
    }
    public override void Exit()
    {
        exclamationMarkTextObj.SetActive(false);
        AgentHelper.ClearPath(agent); //Stop chasing after losing target
    }

    bool IsWithinCatchRange(GameObject objectToCheck)
    {
        return Vector3.Distance(objectToCheck.transform.position, logicCon.transform.position) <= catchRange;
    }
    public override void DrawDebug()
    {
        if (logicCon.currentTargetObj == null) return;

        Vector3 center = logicCon.transform.position;
        float radius = maxChaseDistance;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius); //ターゲットが逃げる距離

        ConeHelper.DrawConeGizmo(logicCon.GetConeInfo());
    }

    void ChaseTarget()
    {
        Vector3 targetPos = logicCon.currentTargetObj.transform.position;
        AgentHelper.MoveTo(agent, targetPos);
    }

    #region private
    public bool IsTargetClose(float maxDistance)
    {
        float dist = Vector3.Distance(logicCon.transform.position, logicCon.currentTargetObj.transform.position);
        return dist <= maxDistance;
    }
    #endregion

}

[System.Serializable]
public class AILogicPatrol : AILogic ///決めた場所にパトロール / 巡回
{
    public AILogicPatrol() : base("AI is Patrolling to places 決めた場所に巡回してます") { }
    [SerializeField] private List<Transform> patrolSpotTransforms;
    Transform currentPatrolSpotT;
    int mode = 0;
    [SerializeField, ReadOnly][Header("現在のパトロールインデックス")] private int patrolIndex = 0;
    public override void Enter()
    {
        GetClosestPatrolSpot(out int nextIndex); //最も近いパトロール場所に巡回し始める
        patrolIndex = nextIndex;
    }

    public override void Action()
    {
        GameObject closestTarget = logicCon.CheckUncaughtTargetsInCone();
        if (closestTarget)
        {
            logicCon.currentTargetObj = closestTarget;
            SetAILogic(logicCon.aiLogicDetecting);
            return;
        }
        else
        {
            if (patrolSpotTransforms.Count == 0) { return; }

            switch (mode)
            {
                case 0: //Moving 
                    if (AgentHelper.HasArrivedSuccess(agent))
                    {
                        mode = 1;
                    }
                    break;

                case 1://Recalculate next spot
                    ChangeToNextPatrol();
                    AgentHelper.MoveTo(agent, currentPatrolSpotT.position);
                    mode = 0;
                    break;
            }
        }
    }

    public override void Exit()
    {

    }

    public override void DrawDebug()
    {
        ConeHelper.DrawConeGizmo(logicCon.GetConeInfo());
        DrawPatrolLines();
    }
    void ChangeToNextPatrol()
    {
        patrolIndex++;
        if (patrolIndex > patrolSpotTransforms.Count - 1) patrolIndex = 0;
        currentPatrolSpotT = patrolSpotTransforms[patrolIndex];
    }

    Vector3 GetClosestPatrolSpot(out int nextIndex)
    {
        nextIndex = -1;
        float closestDist = float.MaxValue;
        Vector3 closestPos = Vector3.zero;

        Vector3 myPos = logicCon.transform.position;

        for (int i = 0; i < patrolSpotTransforms.Count; i++)
        {
            float dist = Vector3.SqrMagnitude(patrolSpotTransforms[i].position - myPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPos = patrolSpotTransforms[i].position;
                nextIndex = i;
            }
        }

        return closestPos;
    }

    private void DrawPatrolLines()
    {
        if (patrolSpotTransforms == null || patrolSpotTransforms.Count == 0)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < patrolSpotTransforms.Count; i++)
        {
            Transform current = patrolSpotTransforms[i];
            Transform next = patrolSpotTransforms[(i + 1) % patrolSpotTransforms.Count]; // loops back to 0

            if (current != null && next != null)
            {
                Gizmos.DrawLine(current.position, next.position);
            }
        }
    }


}

[System.Serializable]
public class AILogicLoiter : AILogic ///ランダム徘徊行動
{
    public AILogicLoiter() : base("AI is Loitering 徘徊中") { }

    [SerializeField][Header("待機時間(秒)")] private Cooldown movePositionCD = new Cooldown(2.0f); //秒
    enum EMode
    {
        Randomizing,
        Moving,
        Waiting
    }
    EMode mode = EMode.Randomizing;

    public override void Enter()
    {
    }
    public override void Action()
    {
        logicCon.currentTargetObj = logicCon.CheckUncaughtTargetsInCone(); //視野角に、チェック


        ////////////////////////
        //Go to AIActionDetected if detect a player
        //プレイヤーを検知したら、return。
        if (FoundTarget())//検知した!!!
        {
            SetAILogic(logicCon.aiLogicDetecting);
            return;
        }
        ////////////////////////

        Loiter(); //Loiter / Patrol
    }

    private void Loiter()
    {
        switch (mode)
        {
            case EMode.Randomizing: //ランダム計算、終わったら徘徊する。
                Vector3 randomNearbyPos = GetRandomPositionNearbyXZ(logicCon.transform.position, 5.0f);
                AgentHelper.MoveTo(agent, randomNearbyPos);
                mode = EMode.Moving;
                break;

            case EMode.Moving: //Moving、終わったら、待機。
                if (AgentHelper.HasArrivedSuccess(agent))
                {
                    movePositionCD.StartCooldown();
                    mode = EMode.Waiting;
                }
                break;

            case EMode.Waiting: //待機、終わったらまたランダム計算。
                if (!movePositionCD.IsCooldown)
                {
                    mode = EMode.Randomizing;
                }
                break;
        }
    }

    public override void Exit() { }

    private Vector3 GetRandomPositionNearbyXZ(Vector3 middlePos, float radius)
    {
        Vector2 randomCirclePos = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomXZPos = middlePos + new Vector3(randomCirclePos.x, 0.0f, randomCirclePos.y);
        return randomXZPos;
    }

    public override void DrawDebug()
    {
        ConeHelper.DrawConeGizmo(logicCon.GetConeInfo());
    }
}
#endregion
