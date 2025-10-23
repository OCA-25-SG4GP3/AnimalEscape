using System;
using UnityEngine;

public class FailPoint : MonoBehaviour
{
    [SerializeField] private VoidEventSO _onFailTrigger;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _onFailTrigger.InvokeEvent();
        }
    }
}
