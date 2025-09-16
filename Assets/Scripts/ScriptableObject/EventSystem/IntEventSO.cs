using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Int Event")]
public class IntEventSO: DescriptionBaseSO
{
    public UnityAction<int> OnEventInvoked;

    public void InvokeEvent(int value)
    {
        OnEventInvoked?.Invoke(value);
    }
}
