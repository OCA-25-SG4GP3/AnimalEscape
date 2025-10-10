using System.Collections.Generic;
using UnityEngine;

public class OpenDoorEvent : MonoBehaviour
{
    [SerializeField] private List<AILogicController> aiLogics;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TriggerEventPlayDoorAnim()
    {
        Animator animator;
        animator = GetComponent<Animator>();
        animator.Play("MoveDoors");
    }
    public void AnimEventSetAIDetectingState()
    {
        foreach(var aiLogic in aiLogics)
        {
            aiLogic.SetStateByEnum(AILogicController.SelectedState.Detecting);
        }
    }
}
