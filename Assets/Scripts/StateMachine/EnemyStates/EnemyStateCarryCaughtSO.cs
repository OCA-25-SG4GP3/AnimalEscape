using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "EnemyStateCarryCaughtSO", menuName = "State/EnemyState/EnemyStateCarryCaughtSO")]
public class EnemyStateCarryCaughtSO : EnemyStateBaseSO
{
    [SerializeField, ReadOnly][Header("捕まえたオブジェクト")] private GameObject _caughtObject;
    Vector3 dropPos;

    public override void EnterState()
    {
        CatchObject();
        MoveToDropInClosestJail();
    }

    public override void UpdateState()
    {
        UpdateCatchedObjectPosRot();

        const float jailCellSize = 2.8f;
        if (AgentHelper.HasArrivedSuccess(_logicController.Agent, jailCellSize)) //牢屋の近くに到着
        {
            DropCatchedObject(dropPos);
            _logicController.SetState(_logicController.PatrolState); //restore
        }
    }

    public override void ExitState()
    {
        AgentHelper.ClearPath(_logicController.Agent);
        if (_caughtObject) DropCatchedObject(_logicController.transform.position);
    }

    public void CatchObject()
    {
        _caughtObject = _logicController.CurrentTarget;
        _caughtObject.GetComponent<PlayerInfo>().hasCaught = true;
    }

    void MoveToDropInClosestJail()
    {
        Jail closestJail = GetClosestJail(); //store for dropping later to prevent accidents
        dropPos = closestJail.jailedObjectSlotT.position;
        AgentHelper.MoveTo(_logicController.Agent, dropPos);
    }

    Jail GetClosestJail()
    {
        List<Vector3> jailPositions = _logicController.Jails.Select(obj => obj.transform.position).ToList();
        Vector3 closestJailPos = Vector3Helper.GetClosest(_logicController.transform.position, jailPositions, out int index);
        return _logicController.Jails[index];
    }
    
        void DropCatchedObject(Vector3 dropPos)
    {
        _caughtObject.transform.position = dropPos;
        _caughtObject = null;
    }
    void UpdateCatchedObjectPosRot()
    {
        _caughtObject.transform.position = _logicController.CatchSlot.position;
        _caughtObject.transform.rotation = _logicController.CatchSlot.rotation;
    }
}