using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Transform Event")]

public class TransformEventSO: DescriptionBaseSO
{
    public UnityAction<Transform> OnEventInvoked;

    public void InvokeEvent(Transform value)
    {
        OnEventInvoked?.Invoke(value);
    }
}
