using UnityEngine;

//EnemyStateBaseSO.cs → 敵の基本状態（ベースクラス）

[CreateAssetMenu(fileName = "EnemyStateBaseSO", menuName = "State/EnemyStateBaseSO")]
public class EnemyStateBaseSO : StateBaseSO
{
    [SerializeField] protected AILogicController _logicController;
    public GameObject Owner => _logicController.gameObject;
    public Animator animator => _logicController.gameObject.GetComponent<Zookeeper>().animator;
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