using UnityEngine;
using UnityEngine.Events;

public class TransformEventSO
{
    public UnityAction<Transform> OnEventInvoked;

    public void InvokeEvent(Transform value)
    {
        OnEventInvoked?.Invoke(value);
    }
}
