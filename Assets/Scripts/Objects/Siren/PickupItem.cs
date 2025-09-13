using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onPickupKeyEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _onPickupKeyEvent.InvokeEvent();
            Destroy(gameObject);
        }
    }
}
