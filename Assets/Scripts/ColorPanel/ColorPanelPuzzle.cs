using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.ProBuilder;
[SelectionBase]
public class ColorPanelPuzzle : MonoBehaviour
{
    ColorPanelGate colorPanelGate;
    Animator animator;
    [SerializeField] public MeshRenderer meshRen;
    [SerializeField] public MeshFilter btnMeshFilter;
    [SerializeField] public MeshFilter frameMeshFilter;
    [NonSerializedAttribute] public Material correctPanelMaterial;
    private Material pressedMaterial;
    public bool upSide = true; //is this upside or downside (to prevent double press / exploit)
    [SerializeField, Header("隠したぁE��合�EチE��アル")] private Material hidingMaterial;
    [SerializeField, Header("何秒までリセチE��")] private float autoResetTimer = 1.0f;
    [SerializeField, Header("何秒までリセチE��")] private float autoHideTimer = 3.0f;

    [SerializeField] private AudioClip bouncingSfx;
    [SerializeField] float bouncingSoundVolume = 1.0f;
    public AudioClip pushSound;
    private AudioSource audioSource;


    public bool isStepped = false; //押されてぁE��かどぁE��
    private Coroutine resetCoroutine;
    private Coroutine hideCoroutine;

    public GameObject steppedEffect;

    public enum EButtonType
    {
        Box, Circle, Star, Triang
    };
    [SerializeField] public EButtonType buttonType;
    [SerializeField] Mesh boxMesh;
    [SerializeField] Mesh starMesh;
    [SerializeField] Mesh triangleMesh;
    [SerializeField] Mesh circleMesh;
    [SerializeField] Mesh frameBoxMesh;
    [SerializeField] Mesh frameTriangMesh;
    [SerializeField] Mesh frameStarMesh;
    [SerializeField] Mesh frameCircleMesh;
    void Awake()
    {
        colorPanelGate = GetActiveGate();
        animator = GetComponent<Animator>();
        correctPanelMaterial = meshRen.sharedMaterial;
        colorPanelGate.RegisterPanel(this);
        if (hidingMaterial) meshRen.material = hidingMaterial;
        audioSource = GetComponent<AudioSource>();

        UpdateMeshByEnum();

        // Copy from correctPanelMaterial and darken
        pressedMaterial = new Material(correctPanelMaterial);
        pressedMaterial.color *= 0.4f; // darken

        pressedMaterial.EnableKeyword("_EMISSION");

        // Darken emission based on the base color instead of the emission color
        Color baseColor = pressedMaterial.color;
        pressedMaterial.SetColor("_EmissionColor", baseColor * 0.4f);
    }

    private void UpdateMeshByEnum()
    {
        switch (buttonType)
        {
            case EButtonType.Box:
                btnMeshFilter.mesh = boxMesh;
                frameMeshFilter.mesh = frameBoxMesh;
                break;
            case EButtonType.Circle:
                btnMeshFilter.mesh = circleMesh;
                frameMeshFilter.mesh = frameCircleMesh;
                break;
            case EButtonType.Triang:
                btnMeshFilter.mesh = triangleMesh;
                frameMeshFilter.mesh = frameTriangMesh;
                break;
            case EButtonType.Star:
                btnMeshFilter.mesh = starMesh;
                frameMeshFilter.mesh = frameStarMesh;
                break;
        }
    }

    void OnValidate()
    {
        UpdateMeshByEnum();
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
            Debug.Log(other.name + " ontrigerenter");
            var playerInfo = other.GetComponent<PlayerInfo>();
            if (playerInfo != null && playerInfo.justLandedFromJump)
            {
                ActivatePanel(other.gameObject);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Handle case where player enters trigger while in air, then lands
        if (other.CompareTag("Player") && !isStepped)
        {
            var playerInfo = other.GetComponent<PlayerInfo>();
            if (playerInfo != null && playerInfo.justLandedFromJump)
            {
                ActivatePanel(other.gameObject);
            }
        }
    }

    private void ActivatePanel(GameObject player)
    {
        if (isStepped) return;

        Debug.Log(player.name + " stepped");

        isStepped = true;

        animator.Play("ColorPanelPressedAnim");
        Instantiate(steppedEffect, transform.position, transform.rotation);
        PlayPushedSFX();

        meshRen.material = pressedMaterial;

        if (hidingMaterial)
        {
            RestoreToCorrectMaterial();

            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            hideCoroutine = StartCoroutine(HideAfterDelay(autoHideTimer));
        }

        colorPanelGate.PanelStepped(this);

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        playerInside = player;
        resetCoroutine = StartCoroutine(ResetStepAfterDelay(autoResetTimer));
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
        source.volume = 0.3f;        // can exceed 1 if using an AudioMixer or normalized later
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
        AnimalControlSimple animal = other.GetComponent<AnimalControlSimple>();
        if (animal != null)
        {
            //animal.UnlockInput();
            //animal.SetMoveSpeed(animal.baseMoveSpeed);
        }
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

        if (playerInside)
        {
            AnimalControlSimple animalControl = playerInside.GetComponent<AnimalControlSimple>();
            if (animalControl != null)
            {
                // 🔓 unlock FIRST so physics can move
                animalControl.LockInput();

                animalControl.SetMoveSpeed(animalControl.baseMoveSpeed);

                // Choose random horizontal direction
                int randomDir = UnityEngine.Random.Range(0, 4);
                Vector3 horizontalForce = Vector3.zero;

                switch (randomDir)
                {
                    case 0: horizontalForce = Vector3.forward; break;
                    case 1: horizontalForce = Vector3.right; break;
                    case 2: horizontalForce = Vector3.back; break;
                    case 3: horizontalForce = Vector3.left; break;
                }

                Vector3 totalForce =
                    (horizontalForce * horizontalResetPush) +
                    (Vector3.up * upResetPush);

                // 💥 push player out
                animalControl.ApplyExternalForce(totalForce, 0.5f);
                PlayBounceSound();
            }

            playerInside = null;
        }
    }

    private void PlayBounceSound()
    {
        GameObject bouncingAudio = new GameObject("Bouncing");
        AudioSource source = bouncingAudio.AddComponent<AudioSource>();
        source.clip = bouncingSfx;
        source.volume = bouncingSoundVolume;
        source.Play();
        Destroy(bouncingAudio, bouncingSfx.length);
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
