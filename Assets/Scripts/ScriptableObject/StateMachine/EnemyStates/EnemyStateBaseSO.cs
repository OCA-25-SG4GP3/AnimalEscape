using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateBaseSO", menuName = "State/EnemyStateBaseSO")]
public class EnemyStateBaseSO : StateBaseSO
{
    [SerializeField] protected AILogicController _logicController;
    public virtual void SetLogicController(AILogicController logicController)
    {
        _logicController = logicController;
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }
    public override void UpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }

    public override void LateUpdateState()
    {
    }

    public override void DrawStateGizmo()
    {
    }

    public virtual bool FoundTarget()
    {
        return _logicController.CurrentTarget != null;
    }
}