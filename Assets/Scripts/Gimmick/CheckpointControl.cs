using System;
using UnityEngine;

public class CheckpointControl : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private IntEventSO _onCheckpointTriggerEvent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _onCheckpointTriggerEvent.InvokeEvent(_id);
        }
    }
}
