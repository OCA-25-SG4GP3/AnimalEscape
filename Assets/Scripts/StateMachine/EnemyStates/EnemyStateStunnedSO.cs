using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStateStunnedSO", menuName = "State/EnemyState/EnemyStateStunnedSO")]
public class EnemyStateStunnedSO : EnemyStateBaseSO
{
    [SerializeField] Cooldown stunDur = new(2.0f);
    [SerializeField,ReadOnly] EnemyStateBaseSO previousState;

    public override void EnterState()
    {
        if(previousState != this) previousState = _logicController.CurrentState; //It shouldn't return from Stun state to Stun state, so we want to prevent infinite stun.
        AgentHelper.Pause(_logicController.Agent);
        stunDur.StartCooldown();
    }

    public override void UpdateState()
    {
        if (!stunDur.IsCooldown) //˜S‰®‚Ì‹ß‚­‚É“ž’…
        {
            _logicController.SetState(previousState);
            return;
        }
    }

    public override void ExitState()
    {
        AgentHelper.Resume(_logicController.Agent);
    }

}