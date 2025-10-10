using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "EnemyStateCarryCaughtSO", menuName = "State/EnemyState/EnemyStateCarryCaughtSO")]
public class EnemyStateCarryCaughtSO : EnemyStateBaseSO
{
    //THIS STATE IS OBSOLOTE
    [SerializeField, ReadOnly][Header("捕まえたオブジェク�?")] private GameObject _caughtObject;
    //[SerializeField][Header("牢屋�?�半�?。到着際、�?�レイヤーをドロ�?�?")] private float jailCellRadius = 5.2f;
    Vector3 movePos;   // where agent should stop (edge of radius)
    Vector3 dropPos;

    public override void EnterState()
    {
        //CatchObject();
        //MoveToDropInClosestJail();
    }

    public override void UpdateState()
    {
        Debug.Log("THIS STATE IS OBSOLOTE!");
        return;

        //UpdateCatchedObjectPosRot();
        //
        //if (AgentHelper.HasArrivedSuccess(_logicController.Agent, jailCellRadius)) //牢屋�?�近くに到着
        //{
        //    DropCaughtObject(dropPos);
        //    _logicController.SetState(_logicController.LoiterStateInstance); //restore
        //}
    }

    public override void ExitState()
    {
        AgentHelper.ClearPath(_logicController.Agent);
        if (_caughtObject) DropCaughtObject(_logicController.transform.position);
    }

    public void CatchObject()
    {
        _caughtObject = _logicController.CurrentTarget;
        _caughtObject.GetComponent<PlayerInfo>().hasCaught = true;
    }

    [System.Obsolete("Jail has been removed from the game")]
    void MoveToDropInClosestJail()
    {
        //Jail jailFound = FindClosestJail(); //store for dropping later to prevent accidents
        //if (!jailFound) return;
        //AgentHelper.MoveTo(_logicController.Agent, movePos);
    }
    [System.Obsolete("Jail has been removed from the game")]
    bool CheckJailExistence() //return success
    {
        return false;
        //if (_logicController.Jails.Count == 0)
        //{
        //    Debug.Log("牢屋�?�配�?�が0サイズ�?");
        //    return false;
        //}
        //foreach (var Jail in _logicController.Jails)
        //{
        //    if (!Jail)
        //    {
        //        Debug.Log("Jail is not assigned in the array! 牢屋�?��?�に、牢屋が設定されてな�??�?");
        //        return false;
        //    }
        //}

        //return true;
    }
    [System.Obsolete("Jail has been removed from the game")]
    Jail FindClosestJail()
    {
        return null;
        //bool success = CheckJailExistence(); //Debug checker
        //if (!success) return null;
        //
        //List<Vector3> jailPositions = _logicController.Jails.Select(obj => obj.transform.position).ToList();
        //Vector3 closestJailPos = Vector3Helper.GetClosest(_logicController.transform.position, jailPositions, out int index);
        //Jail jail = _logicController.Jails[index];
        //
        //// offset: stop at the edge of the radius, not the center
        //Vector3 dir = (closestJailPos - _logicController.transform.position).normalized;
        //Vector3 stopPos = closestJailPos - dir * jailCellRadius; // distance from center to edge
        //
        //movePos = stopPos; // update drop position for MoveToDropInClosestJail
        //dropPos = jail.jailedObjectSlotT.position;
        //return jail;
    }

    void DropCaughtObject(Vector3 dropPos)
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