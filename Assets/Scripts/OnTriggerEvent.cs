using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent ev;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
        ev.Invoke();
        Destroy(this.gameObject);
        }
    }
}
