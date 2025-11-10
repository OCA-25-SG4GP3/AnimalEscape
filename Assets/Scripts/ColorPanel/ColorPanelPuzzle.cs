using System;
using UnityEngine.Audio;
using UnityEngine;

public class ColorPanelPuzzle : MonoBehaviour
{
    ColorPanelManager colorPanelManager;
    Animator animator;
    [SerializeField] public MeshRenderer meshRen;
    [NonSerializedAttribute] public Material correctPanelMaterial;
    public bool upSide = true; //is this upside or downside (to prevent double press / exploit)
    [SerializeField, Header("このスロットにつけると、マテリアルが隠しになる")] private Material hidingMaterial;

    public AudioClip pushSound;      // ジャンプ音のファイル
    private AudioSource audioSource; // AudioSourceを使うための変数


    public bool isStepped = false; //踏まえたかどうか

    void Awake()
    {
        colorPanelManager = FindAnyObjectByType<ColorPanelManager>();
        animator = GetComponent<Animator>();
        correctPanelMaterial = meshRen.material;
        colorPanelManager.RegisterPanel(this);
        if (hidingMaterial) meshRen.material = hidingMaterial;
        audioSource = GetComponent<AudioSource>();

    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            //TODO need cache to reduce lag ?
            var playerInfo = other.GetComponent<PlayerInfo>();
            if (!playerInfo.IsFallingDown()) return;

            isStepped = true;
            animator.Play("ColorPanelPressedAnim");
            audioSource.PlayOneShot(pushSound);
            colorPanelManager.PanelStepped(this);
            if (hidingMaterial && meshRen.material != correctPanelMaterial) RestoreToCorrectMaterial();

        }
    }

    void RestoreToCorrectMaterial()
    {
        meshRen.material = correctPanelMaterial;
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
