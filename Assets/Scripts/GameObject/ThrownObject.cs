using System;
using UnityEngine;
using UnityEngine.Events;

public class ThrownObject : MonoBehaviour
{
    [SerializeField] private GameObject VFXObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 3.0f); //Safeguard
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ActivateVFX();
            Destroy(gameObject);
        }
    }

    void ActivateVFX()
    {
        VFXObj.SetActive(true);
    }
}
