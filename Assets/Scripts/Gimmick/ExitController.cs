using UnityEngine;

public class ExitController : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onGameClearEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _onGameClearEvent.InvokeEvent();
        }
    }
}
