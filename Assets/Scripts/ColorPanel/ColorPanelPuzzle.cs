using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class ColorPanelPuzzle : MonoBehaviour
{
    ColorPanelGate colorPanelGate;
    Animator animator;
    [SerializeField] public MeshRenderer meshRen;
    [NonSerializedAttribute] public Material correctPanelMaterial;
    [SerializeField] private Material pressedMaterial;
    public bool upSide = true; //is this upside or downside (to prevent double press / exploit)
    [SerializeField, Header("髫縺励◆縺・ｴ蜷医・繝・Μ繧｢繝ｫ")] private Material hidingMaterial;
    [SerializeField, Header("菴慕ｧ偵∪縺ｧ繝ｪ繧ｻ繝・ヨ")] private float autoResetTimer = 1.0f;

    public AudioClip pushSound;      // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾌフ・ｽ@・ｽC・ｽ・ｽ
    private AudioSource audioSource; // AudioSource・ｽ・ｽ・ｽg・ｽ・ｽ・ｽ・ｽ・ｽﾟの変撰ｿｽ


    public bool isStepped = false; //謚ｼ縺輔ｌ縺ｦ縺・ｋ縺九←縺・°
    [SerializeField] private Cooldown returnToWhiteCD = new(3.0f);
    private Coroutine resetCoroutine;

    public GameObject steppedEffect;


    void Awake()
    {
        colorPanelGate = GetActiveGate();
        animator = GetComponent<Animator>();
        correctPanelMaterial = meshRen.sharedMaterial;
        colorPanelGate.RegisterPanel(this);
        if (hidingMaterial) meshRen.material = hidingMaterial;
        audioSource = GetComponent<AudioSource>();
    }

    ColorPanelGate GetActiveGate()
    {
        var allGates = FindObjectsByType<ColorPanelGate>(FindObjectsSortMode.None);
        foreach (var gate in allGates)
        {
            if (!gate.IsGateOpened() && gate.CanAcceptPanels())
            {
                return gate;
            }
        }
        // If all gates are opened, return the first one (fallback)
        return allGates.Length > 0 ? allGates[0] : null;
    }

    void Update()
    {
        if (hidingMaterial && IsCurrentlyUsingCorrectMaterial() && !returnToWhiteCD.IsCooldown)
        {
            RestoreToHidingMaterial();
        }

        // Switch gates if current gate is opened
        if (colorPanelGate != null && colorPanelGate.IsGateOpened())
        {
            var newGate = GetActiveGate();
            if (newGate != null && newGate != colorPanelGate)
            {
                colorPanelGate = newGate;
                colorPanelGate.RegisterPanel(this);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            //TODO need cache to reduce lag ?
            var playerInfo = other.GetComponent<PlayerInfo>();
            if (!playerInfo.IsFallingDown()) return;
            if (!playerInfo.CanStepButton) return;

            isStepped = true;
            animator.Play("ColorPanelPressedAnim");
            //audioSource.PlayOneShot(pushSound); //・ｽj・ｽｳゑｿｽ・ｽﾆバ・ｽO・ｽ・ｽ
            Instantiate(steppedEffect, transform.position, transform.rotation);
            //audioSource.PlayOneShot(pushSound); //破壊されるとバグる
            //AudioSource.PlayClipAtPoint(pushSound, transform.position, 10000.0f); this volume is capped at 1
            PlayPushedSFX();

            meshRen.material = pressedMaterial;

            if (IsCurrentlyUsingHidingMaterial()) //If any hiding mat is assigned
            {
                RestoreToCorrectMaterial();
                returnToWhiteCD.StartCooldown();
            }

            colorPanelGate.PanelStepped(this); //last order so it change first then checked

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            resetCoroutine = StartCoroutine(ResetStepAfterDelay(autoResetTimer));
        }
    }
    private bool IsCurrentlyUsingHidingMaterial()
    {
        return hidingMaterial && meshRen.material == hidingMaterial;
    }
    private bool IsCurrentlyUsingCorrectMaterial()
    {
        return meshRen.material == correctPanelMaterial;
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
            colorPanelGate.PanelReleased(this);
            if (hidingMaterial)
            {
                RestoreToHidingMaterial();
            }
            else
            {
                meshRen.material = correctPanelMaterial;
            }
        }
    }
    private IEnumerator ResetStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isStepped = false;
        animator.Play("ColorPanelReleasedAnim");
        colorPanelGate.PanelReleased(this);
        if (hidingMaterial)
        {
            RestoreToHidingMaterial();
        }
        else
        {
            meshRen.material = correctPanelMaterial;
        }
    }

}
