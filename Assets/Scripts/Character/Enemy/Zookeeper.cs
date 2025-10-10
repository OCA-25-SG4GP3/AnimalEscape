using UnityEngine;


public class Zookeeper : MonoBehaviour
{
    Animator animator;
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
