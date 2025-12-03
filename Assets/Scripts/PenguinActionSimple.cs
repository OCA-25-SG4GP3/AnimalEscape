using TMPro;
using UnityEngine;

public class PenguinActionSimple : MonoBehaviour
{
    [Header("Slide Parameters")]
    [SerializeField] private float slideSpeed = 8f;
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private float slideHeightOffset = 0.5f; // how much the model hovers
    [SerializeField] private Cooldown slideCooldown = new(1.8f);

    [Header("References")]
    [SerializeField] private Transform model;
    [SerializeField] private GameObject impactCollider;
    [SerializeField] private TMP_Text cooldownText;
    private PlayerInfoSystem playerInfoSystem;
    private AnimalControlSimple animalControlSimple;
    private PlayerInfo playerInfo;
    private CapsuleCollider capsule;

    private Vector3 slideDirection;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float originalMoveSpeed;
    private Quaternion originalLocalRotation;
    private Vector3 originalModelLocalPos;
    private Vector3 originalCapsuleCenter;
    private float originalCapsuleHeight;

    public GameObject slideEffect;

    void Awake()
    {
        playerInfo = GetComponent<PlayerInfo>();
        playerInfoSystem = FindAnyObjectByType<PlayerInfoSystem>();
        animalControlSimple = GetComponent<AnimalControlSimple>();
        capsule = GetComponent<CapsuleCollider>();

        if (!model) model = transform;
        if (impactCollider) impactCollider.SetActive(false);

        originalModelLocalPos = model.localPosition;

        if (capsule)
        {
            originalCapsuleCenter = capsule.center;
            originalCapsuleHeight = capsule.height;
        }
    }
    void Update()
    {
        // trigger slide only if requested
        if (Input.GetKeyDown(animalControlSimple.inputKeys.specialAction)
        &&
        !slideCooldown.IsCooldown
        &&
        !animalControlSimple.IsAIControlled
        )
        {
            StartSlide();
            slideCooldown.StartCooldown();
        }

        if (isSliding)
        {
            slideTimer += Time.deltaTime;
            if (slideTimer >= slideDuration)
                EndSlide();
        }

        PlayerInfo.UpdateCDText(cooldownText, slideCooldown);
    }

    void FixedUpdate()
    {
        if (!isSliding) return;

        // Move along initial slide direction
        transform.position += slideDirection * slideSpeed * Time.fixedDeltaTime;

        // Apply safe tilt: add 90Åã X on top of original local rotation
        model.localRotation = originalLocalRotation * Quaternion.Euler(90f, 0f, 0f);

        // Slightly hover the model above the ground
        model.localPosition = originalModelLocalPos + Vector3.up * slideHeightOffset;
    }

    public void StartSlide()
    {
        if (isSliding) return;


        // Effect
        Instantiate(slideEffect, transform.position, transform.rotation);

        isSliding = true;
        slideTimer = 0f;

        slideDirection = transform.forward;

        originalMoveSpeed = animalControlSimple.baseMoveSpeed;
        animalControlSimple.SetMoveSpeed(playerInfoSystem.GetDistanceAffectedPlayerSpeed(originalMoveSpeed));

        playerInfo.hasCaught = true;

        if (impactCollider) impactCollider.SetActive(true);

        originalLocalRotation = model.localRotation;

        if (capsule)
        {
            capsule.height *= 0.25f;
            capsule.center = originalCapsuleCenter * 0.25f;
        }

    }

    private void EndSlide()
    {
        isSliding = false;
        animalControlSimple.SetMoveSpeed(originalMoveSpeed); // restore base speed
        playerInfo.hasCaught = false;

        // Effect
        Instantiate(slideEffect, transform.position, transform.rotation);

        if (impactCollider) impactCollider.SetActive(false);

        model.localRotation = originalLocalRotation;
        model.localPosition = originalModelLocalPos;

        if (capsule)
        {
            capsule.height = originalCapsuleHeight;
            capsule.center = originalCapsuleCenter;
        }
    }

}
