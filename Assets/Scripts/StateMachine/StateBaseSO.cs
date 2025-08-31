using System.Diagnostics.Contracts;
using UnityEngine;

[CreateAssetMenu(fileName = "StateBaseSO", menuName = "StateBaseSO")]
public abstract class StateBaseSO : DescriptionBaseSO
{
    public bool IsComplete { get; protected set; } = false;
    public bool CanBeInterrupted { get; protected set; } = true;

    protected float _startTime;
    public float ElapsedTime => Time.time - _startTime;

    public virtual void Initialize() 
    { 
        IsComplete = false; 
        StartTimer();
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void LateUpdateState();
    public abstract void ExitState();

    public abstract void DrawStateGizmo();

    protected void StartTimer()
    {
        _startTime = Time.time;
    }
}
