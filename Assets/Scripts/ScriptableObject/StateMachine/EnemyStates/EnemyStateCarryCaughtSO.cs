using UnityEngine;
using System.Collections.Generic;
using System.Linq;


//EnemyStateCarryCaughtSO.cs Å® ìGÇ™éùÇøè„Ç∞ÇÁÇÍÇΩÅ^ÉvÉåÉCÉÑÅ[Ç™ïﬂÇ‹ÇÍÇΩèÛë‘


[CreateAssetMenu(fileName = "EnemyStateCarryCaughtSO", menuName = "State/EnemyState/EnemyStateCarryCaughtSO")]
public class EnemyStateCarryCaughtSO : EnemyStateBaseSO
{
    //THIS STATE IS OBSOLOTE
    [SerializeField, ReadOnly][Header("Êçï„Åæ„Åà„Åü„Ç™„Éñ„Ç∏„Çß„ÇØ„É?")] private GameObject _caughtObject;
    //[SerializeField][Header("Áâ¢Â±ã„?ÆÂçäÂæ?„ÄÇÂà∞ÁùÄÈöõ„ÄÅ„?ó„É¨„Ç§„É§„Éº„Çí„Éâ„É≠„É?„É?")] private float jailCellRadius = 5.2f;
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
        //if (AgentHelper.HasArrivedSuccess(_logicController.Agent, jailCellRadius)) //Áâ¢Â±ã„?ÆËøë„Åè„Å´Âà∞ÁùÄ
        //{
        //    DropCaughtObject(dropPos);
        //    _logicController.SetState(_logicController.LoiterStateInstance); //restore
        //}
    }

    public override void ExitState()
    {
        _logicController.rbNavMesh.ClearPath();
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
        //    Debug.Log("Áâ¢Â±ã„?ÆÈÖçÂ?ó„Åå0„Çµ„Ç§„Ç∫„Ä?");
        //    return false;
        //}
        //foreach (var Jail in _logicController.Jails)
        //{
        //    if (!Jail)
        //    {
        //        Debug.Log("Jail is not assigned in the array! Áâ¢Â±ãÈ?çÂ?ó„Å´„ÄÅÁâ¢Â±ã„ÅåË®≠ÂÆö„Åï„Çå„Å¶„Å™„Å??º?");
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