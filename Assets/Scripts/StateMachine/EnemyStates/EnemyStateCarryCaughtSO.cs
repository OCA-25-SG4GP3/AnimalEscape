using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "EnemyStateCarryCaughtSO", menuName = "State/EnemyState/EnemyStateCarryCaughtSO")]
public class EnemyStateCarryCaughtSO : EnemyStateBaseSO
{
    [SerializeField, ReadOnly][Header("捕まえたオブジェクト")] private GameObject _caughtObject;
    [SerializeField][Header("牢屋の半径。到着際、プレイヤーをドロップ")] private float jailCellRadius = 5.2f;
    Vector3 movePos;   // where agent should stop (edge of radius)
    Vector3 dropPos;

    public override void EnterState()
    {
        CatchObject();
        MoveToDropInClosestJail();
    }

    public override void UpdateState()
    {
        UpdateCatchedObjectPosRot();

        if (AgentHelper.HasArrivedSuccess(_logicController.Agent, jailCellRadius)) //牢屋の近くに到着
        {
            DropCaughtObject(dropPos);
            _logicController.SetState(_logicController.PatrolState); //restore
        }
    }

    public override void ExitState()
    {
        AgentHelper.ClearPath(_logicController.Agent);
        if (_caughtObject) DropCaughtObject(_logicController.transform.position);
    }

    public void CatchObject()
    {
        _caughtObject = _logicController.CurrentTarget.transform.parent.gameObject;
        _caughtObject.GetComponent<PlayerInfo>().hasCaught = true;
    }

    void MoveToDropInClosestJail()
    {
        Jail jailFound = FindClosestJail(); //store for dropping later to prevent accidents
        if (!jailFound) return;
        AgentHelper.MoveTo(_logicController.Agent, movePos);
    }

    bool CheckJailExistence() //return success
    {
        if (_logicController.Jails.Count == 0)
        {
            Debug.Log("牢屋の配列が0サイズ。");
            return false;
        }
        foreach (var Jail in _logicController.Jails)
        {
            if (!Jail)
            {
                Debug.Log("Jail is not assigned in the array! 牢屋配列に、牢屋が設定されてない！");
                return false;
            }
        }

        return true;
    }
    Jail FindClosestJail()
    {
        bool success = CheckJailExistence(); //Debug checker
        if (!success) return null;

        List<Vector3> jailPositions = _logicController.Jails.Select(obj => obj.transform.position).ToList();
        Vector3 closestJailPos = Vector3Helper.GetClosest(_logicController.transform.position, jailPositions, out int index);
        Jail jail = _logicController.Jails[index];

        // offset: stop at the edge of the radius, not the center
        Vector3 dir = (closestJailPos - _logicController.transform.position).normalized;
        Vector3 stopPos = closestJailPos - dir * jailCellRadius; // distance from center to edge

        movePos = stopPos; // update drop position for MoveToDropInClosestJail
        dropPos = jail.jailedObjectSlotT.position;
        return jail;
    }

    void DropCaughtObject(Vector3 dropPos)
    {
        _caughtObject.transform.parent.position = dropPos;
        _caughtObject = null;
    }
    void UpdateCatchedObjectPosRot()
    {
        _caughtObject.transform.parent.position = _logicController.CatchSlot.position;
        _caughtObject.transform.parent.rotation = _logicController.CatchSlot.rotation;
    }
}