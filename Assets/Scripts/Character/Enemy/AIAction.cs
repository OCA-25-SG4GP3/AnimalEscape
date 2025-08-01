using System;
using UnityEngine;
using UnityEngine.AI;


#region AIAction
public class AIAction : MonoBehaviour
{
    [SerializeReference] public AILogic aiLogic; //行動パターン抽象データー
    [NonSerialized] public NavMeshAgent agent;
    public GameObject targetObj; //そのターゲットに何かをしたいとき
    #region unity
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetAIAction<AIActionLoiter>(); //初期化モードは徘徊
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!agent) Debug.LogWarning("NavMeshAgentが持ってないオブジェクトです。");
#endif
        aiLogic.Action();
    }
    private void OnDrawGizmos()
    {
        if (aiLogic != null)
            aiLogic.DrawDebug();
    }

    #endregion
    #region public
    public void SetAIAction<T>() where T : AILogic, new()
    {
        //前のAIを終わらせる
        if (aiLogic != null) aiLogic.Exit();

        //新しいAIがエンター
        T newAIAction = new T();
        newAIAction.Enter(this);

        //前のAIを上書き
        aiLogic = newAIAction;
    }
    #endregion
}
[System.Serializable]
public abstract class AILogic
///We want this class to be loosely coupled
///何のゲームオブジェクトが実行できるように、依存性を少なくにします。
{
    protected AILogic(string name) { logicName = name; } //for debug
    protected AIAction aiAction = null; //set on child
    protected NavMeshAgent agent => aiAction.agent;
    [SerializeField, ReadOnly] private string logicName; //Debug display
    protected void SetLogicName(string name) => logicName = name;
    public abstract void Enter(AIAction aiAction); ///Change animation here
    public abstract void Action(); ///Main loop
    public abstract void Exit();
    public virtual void DrawDebug() { }// optional override in child classes
}
[System.Serializable]
public class AIActionDetecting : AILogic ///検知する
{
    public AIActionDetecting() : base("AI is Detecting 検知した") { }

    public override void Enter(AIAction aiAction)
    {
        this.aiAction = aiAction;
    }

    public override void Action()
    {
        if (aiAction.targetObj)
        {

        }
        else if (aiAction.targetObj == null || IsTargetTooFar(20.0f))
        {
            aiAction.SetAIAction<AIActionLoiter>();
        }
    }
    public override void Exit() { }
    public override void DrawDebug()
    {

    }

    bool IsTargetTooFar(float maxDistance)
    {
        float dist = Vector3.Distance(aiAction.transform.position, aiAction.targetObj.transform.position);
        return dist <= maxDistance;
    }
}
[System.Serializable]
public class AIActionLoiter : AILogic ///徘徊行動
{
    public AIActionLoiter() : base("AI is Loitering 徘徊中") { }

    Cooldown movePositionCD = new Cooldown(2.0f); //秒
    enum EMode
    {
        Randomizing,
        Moving,
        Waiting
    }
    EMode mode = EMode.Randomizing;

    public override void Enter(AIAction aiAction)
    {
        this.aiAction = aiAction;
    }
    public override void Action()
    {
        ////////////////////////
        //Go to AIActionDetected if detect a player
        //プレイヤーを検知したら、return。
        if (PlayerSpotted())
        {
            aiAction.SetAIAction<AIActionDetecting>();
            return;
        }
        ////////////////////////

        switch (mode)
        {
            case EMode.Randomizing: //ランダム計算、終わったら徘徊する。
                Vector3 randomNearbyPos = GetRandomPositionNearbyXZ(aiAction.transform.position, 5.0f);
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
    bool PlayerSpotted()
    {
        return false;
    }
}
#endregion
