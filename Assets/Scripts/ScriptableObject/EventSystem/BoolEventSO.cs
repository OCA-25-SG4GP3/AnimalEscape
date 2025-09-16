using UnityEngine;
using UnityEngine.Events;

public class BoolEventSO: DescriptionBaseSO
{
    public UnityAction<bool> OnEventInvoked;

    public void InvokeEvent(bool value)
    {
        OnEventInvoked?.Invoke(value);
    }
}
