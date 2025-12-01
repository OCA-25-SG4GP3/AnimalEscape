using System;
using UnityEngine;


public class Zookeeper : MonoBehaviour
{
    [SerializeField] public Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        UpdateAnim();
    }

    void UpdateAnim()
    {
        //if () animator.SetBool("IsRunning");
    }
}
