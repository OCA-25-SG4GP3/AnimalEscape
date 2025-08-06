using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Void Event")]
public class VoidEventSO: DescriptionBaseSO
{
    public UnityAction OnEventInvoked;

    public void InvokeEvent()
    {
        OnEventInvoked?.Invoke();
    }
}
