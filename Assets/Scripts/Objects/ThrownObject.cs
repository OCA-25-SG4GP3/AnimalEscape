using System;
using UnityEngine;
using UnityEngine.Events;

public class ThrownObject : MonoBehaviour
{
    [SerializeField] private GameObject VFXObj;
    [NonSerializedAttribute] public UnityEvent OnHitEnemy = new();
    bool hitOnce = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 3.0f); //Safeguard
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !hitOnce)
        {
            OnHitEnemy.Invoke();
            ActivateVFX();
            Destroy(gameObject, 3.0f); //Safeguard
            hitOnce = true;
            AILogicController logicCont = collision.gameObject.GetComponent<AILogicController>();
            logicCont.SetState(logicCont.StunState);
            //Destroy(gameObject);
        }
    }

    void ActivateVFX()
    {
        VFXObj.SetActive(true);
    }
}
