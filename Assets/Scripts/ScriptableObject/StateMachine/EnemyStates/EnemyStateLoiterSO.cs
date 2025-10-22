using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateLoiterSO", menuName = "State/EnemyState/EnemyStateLoiterSO")]
public class EnemyStateLoiterSO : EnemyStateBaseSO ///ランダ�?徘徊行動
{
    [Header("�?機時�?(�?)")]
    [SerializeField] private Cooldown movePositionCD = new(2.0f); //�?

    enum EMode
    {
        Randomizing,
        Moving,
        Waiting,
    }
    private EMode _mode = EMode.Randomizing;

    public override void UpdateState()
    {
        _logicController.CurrentTarget = _logicController.CheckUncaughtTargetsInCone(); //視野角に、チェ�?ク


        ////////////////////////
        //Go to AIActionDetected if detect a player
        //プレイヤーを検知したら、SetState, return�?
        if (FoundTarget())//検知した!!!
        {
            _logicController.SetState(_logicController.DetectingStateInstance);
            return;
        }
        ////////////////////////

        //検知しな�?場合、巡回更新続け�?
        LoiterState(); //Loiter / Patrol
    }

    private void LoiterState()
    {
        switch (_mode)
        {
            case EMode.Randomizing: //ランダ�?計算、終わったら徘徊する�?
                Vector3 randomNearbyPos = GetRandomPositionNearbyXZ(_logicController.transform.position, 5.0f);
                //AgentHelper.MoveTo(_logicController.Agent, randomNearbyPos);
                _logicController.rbNavMesh.MoveTo(randomNearbyPos);
                _mode = EMode.Moving;
                break;

            case EMode.Moving: //Moving、終わったら、�?機�?
                
                if (_logicController.rbNavMesh.HasArrived())
                {
                    movePositionCD.StartCooldown();
                    _mode = EMode.Waiting;
                }
                break;

            case EMode.Waiting: //�?機、終わったらまたランダ�?計算�?
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