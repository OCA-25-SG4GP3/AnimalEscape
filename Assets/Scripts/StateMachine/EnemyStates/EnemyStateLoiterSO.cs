using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateLoiterSO", menuName = "State/EnemyState/EnemyStateLoiterSO")]
public class EnemyStateLoiterSO : EnemyStateBaseSO ///ランダム徘徊行動
{
    [Header("待機時間(秒)")]
    [SerializeField] private Cooldown movePositionCD = new(2.0f); //秒

    enum EMode
    {
        Randomizing,
        Moving,
        Waiting,
    }
    private EMode _mode = EMode.Randomizing;

    public override void UpdateState()
    {
        _logicController.CurrentTarget = _logicController.CheckUncaughtTargetsInCone(); //視野角に、チェック


        ////////////////////////
        //Go to AIActionDetected if detect a player
        //プレイヤーを検知したら、SetState, return。
        if (FoundTarget())//検知した!!!
        {
            _logicController.SetState(_logicController.DetectingStateInstance);
            return;
        }
        ////////////////////////

        //検知しない場合、巡回更新続ける
        LoiterState(); //Loiter / Patrol
    }

    private void LoiterState()
    {
        switch (_mode)
        {
            case EMode.Randomizing: //ランダム計算、終わったら徘徊する。
                Vector3 randomNearbyPos = GetRandomPositionNearbyXZ(_logicController.transform.position, 5.0f);
                AgentHelper.MoveTo(_logicController.Agent, randomNearbyPos);
                _mode = EMode.Moving;
                break;

            case EMode.Moving: //Moving、終わったら、待機。
                if (AgentHelper.HasArrivedSuccess(_logicController.Agent))
                {
                    movePositionCD.StartCooldown();
                    _mode = EMode.Waiting;
                }
                break;

            case EMode.Waiting: //待機、終わったらまたランダム計算。
                if (!movePositionCD.IsCooldown)
                {
                    _mode = EMode.Randomizing;
                }
                break;
        }
    }

    private Vector3 GetRandomPositionNearbyXZ(Vector3 middlePos, float radius)
    {
        Vector2 randomCirclePos = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomXZPos = middlePos + new Vector3(randomCirclePos.x, 0.0f, randomCirclePos.y);
        return randomXZPos;
    }

    public override void DrawStateGizmo()
    {
        ConeHelper.DrawConeGizmo(_logicController.GetConeInfo());
    }
}