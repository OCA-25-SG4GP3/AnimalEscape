using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Float Event")]
public class FloatEventSO: DescriptionBaseSO
{
    public UnityAction<float> OnEventInvoked;

    public void InvokeEvent(float value)
    {
        OnEventInvoked?.Invoke(value);
    }
}
