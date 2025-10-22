using System;
using UnityEngine;

public class ColorPanelPuzzle : MonoBehaviour
{
    ColorPanelManager colorPanelManager;
    Animator animator;
    [SerializeField] public MeshRenderer meshRen;
    [NonSerializedAttribute] public Material panelMaterial;
    public bool upSide = true; //is this upside or downside (to prevent double press / exploit)

    public bool isStepped = false; //“¥‚Ü‚¦‚½‚©‚Ç‚¤‚©

    void Awake()
    {
        colorPanelManager = FindAnyObjectByType<ColorPanelManager>();
        animator = GetComponent<Animator>();
        panelMaterial = meshRen.material;
        colorPanelManager.RegisterPanel(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //TODO need cache to reduce lag ?
            var playerInfo = other.GetComponent<PlayerInfo>();
            if (!playerInfo.IsFallingDown()) return;
            
            print("pressed");
            isStepped = true;
            animator.Play("ColorPanelPressedAnim");
            colorPanelManager.PanelStepped(this);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isStepped) return;
            isStepped = false;
            animator.Play("ColorPanelReleasedAnim");
            colorPanelManager.PanelReleased(this);
        }
    }



}
