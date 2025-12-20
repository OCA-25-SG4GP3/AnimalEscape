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
    [SerializeField, Header("隠したぁE��合�EチE��アル")] private Material hidingMaterial;
    [SerializeField, Header("何秒までリセチE��")] private float autoResetTimer = 1.0f;
    [SerializeField, Header("何秒までリセチE��")] private float autoHideTimer = 3.0f;

    public AudioClip pushSound;
    private AudioSource audioSource;


    public bool isStepped = false; //押されてぁE��かどぁE��
    private Coroutine resetCoroutine;
    private Coroutine hideCoroutine;

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
    GameObject playerInside;
    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            //TODO need cache to reduce lag ?
            var playerInfo = other.GetComponent<PlayerInfo>();
            if (!playerInfo.IsFallingDown()) return;

            isStepped = true;
            animator.Play("ColorPanelPressedAnim");
            //audioSource.PlayOneShot(pushSound); //�E�j�E�󂳂��E�ƃo�E�O�E��E�
            Instantiate(steppedEffect, transform.position, transform.rotation);
            //audioSource.PlayOneShot(pushSound); //�j�󂳂��ƃo�O��
            //AudioSource.PlayClipAtPoint(pushSound, transform.position, 10000.0f); this volume is capped at 1
            PlayPushedSFX();

            meshRen.material = pressedMaterial;

            if (hidingMaterial) //If hiding material is assigned
            {
                RestoreToCorrectMaterial();

                // Start hide timer to restore to hiding material later
                if (hideCoroutine != null)
                {
                    StopCoroutine(hideCoroutine);
                }
                hideCoroutine = StartCoroutine(HideAfterDelay(autoHideTimer));
            }

            colorPanelGate.PanelStepped(this); //last order so it change first then checked

            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            playerInside = other.gameObject;
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

            // Don't change material here, let hideCoroutine handle it if using hiding material
            if (!hidingMaterial)
            {
                meshRen.material = correctPanelMaterial;
            }

            // Clear playerInside since they left
            playerInside = null;
        }
    }

    [SerializeField] private float upResetPush = 20.0f;
    [SerializeField] private float horizontalResetPush = 20.0f;
    private IEnumerator ResetStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        isStepped = false;
        animator.Play("ColorPanelReleasedAnim");
        colorPanelGate.PanelReleased(this);

        // Don't change material here, let hideCoroutine handle it
        if (!hidingMaterial)
        {
            meshRen.material = correctPanelMaterial;
        }

        if(playerInside)
        {
            // Apply random force in one of 4 directions (forward, back, left, right) plus upward
            AnimalControlSimple animalControl = playerInside.GetComponent<AnimalControlSimple>();
            if (animalControl != null)
            {
                // Choose random horizontal direction (0=forward, 1=right, 2=back, 3=left)
                int randomDir = UnityEngine.Random.Range(0, 4);
                Vector3 horizontalForce = Vector3.zero;

                switch (randomDir)
                {
                    case 0: horizontalForce = Vector3.forward; break;
                    case 1: horizontalForce = Vector3.right; break;
                    case 2: horizontalForce = Vector3.back; break;
                    case 3: horizontalForce = Vector3.left; break;
                }

                // Combine horizontal and upward force
                Vector3 totalForce = (horizontalForce * horizontalResetPush) + (Vector3.up * upResetPush);
                animalControl.ApplyExternalForce(totalForce, 0.5f);

                Debug.Log($"Panel reset pushed player with force: {totalForce}");
            }

            playerInside = null;
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // After timer expires, hide the panel again (regardless of current material)
        if (hidingMaterial)
        {
            RestoreToHidingMaterial();
        }
    }

}
