using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateStunnedSO", menuName = "State/EnemyState/EnemyStateStunnedSO")]
public class EnemyStateStunnedSO : EnemyStateBaseSO
{
    [SerializeField] Cooldown stunDur = new(2.0f);
    EnemyStateBaseSO previousState = null;
    public override void EnterState()
    {
        if (previousState == null || _logicController.CurrentState.GetType() != GetType())
        {
            previousState = _logicController.CurrentState; //It shouldn't return from Stun state to Stun state, so we want to prevent infinite stun.
        }
        _logicController.rbNavMesh.Pause();
        stunDur.StartCooldown();
    }

    public override void UpdateState()
    {
        if (!stunDur.IsCooldown)
        {
            _logicController.SetState(previousState);
            return;
        }
    }

    public override void ExitState()
    {
        _logicController.rbNavMesh.Resume();
    }

}