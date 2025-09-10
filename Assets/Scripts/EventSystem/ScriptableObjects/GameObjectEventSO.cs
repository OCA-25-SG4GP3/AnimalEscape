using UnityEngine;
using UnityEngine.Events;

public class GameObjectEventSO
{
    public UnityAction<GameObject> OnEventInvoked;

    public void InvokeEvent(GameObject value)
    {
        OnEventInvoked?.Invoke(value);
    }
}
