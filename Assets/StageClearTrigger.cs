using UnityEngine;

public class StageClearTrigger : MonoBehaviour
{
    [SerializeField] private VoidEventSO _stageClearEvent;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _stageClearEvent.InvokeEvent();
        }
    }
}
