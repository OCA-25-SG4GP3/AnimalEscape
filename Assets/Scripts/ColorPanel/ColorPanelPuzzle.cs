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
    [SerializeField] private Cooldown returnToWhiteCD = new(3.0f);
    void Awake()
    {
        colorPanelManager = FindAnyObjectByType<ColorPanelManager>();
        animator = GetComponent<Animator>();
        correctPanelMaterial = meshRen.sharedMaterial;
        colorPanelManager.RegisterPanel(this);
        if (hidingMaterial) meshRen.sharedMaterial = hidingMaterial;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (hidingMaterial && IsCurrentlyUsingCorrectMaterial() && !returnToWhiteCD.IsCooldown)
        {
            RestoreToHidingMaterial();
        }
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
            //audioSource.PlayOneShot(pushSound); //破壊されるとバグる
            //AudioSource.PlayClipAtPoint(pushSound, transform.position, 10000.0f); this volume is capped at 1
            PlayPushedSFX();

            if (IsCurrentlyUsingHidingMaterial()) //If any hiding mat is assigned
            {
                RestoreToCorrectMaterial();
                returnToWhiteCD.StartCooldown();
            }

            colorPanelManager.PanelStepped(this); //last order so it change first then checked
        }
    }

    private bool IsCurrentlyUsingHidingMaterial()
    {
        return hidingMaterial && meshRen.sharedMaterial == hidingMaterial;
    }
    private bool IsCurrentlyUsingCorrectMaterial()
    {
        return meshRen.sharedMaterial == correctPanelMaterial;
    }

    private void PlayPushedSFX()
    {
        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.position = transform.position;
        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = pushSound;
        source.volume = 2f;        // can exceed 1 if using an AudioMixer or normalized later
        source.Play();
        Destroy(tempAudio, pushSound.length);
    }

    void RestoreToCorrectMaterial()
    {
        meshRen.material = correctPanelMaterial;
    }

    void RestoreToHidingMaterial()
    {
        meshRen.material = hidingMaterial;
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
