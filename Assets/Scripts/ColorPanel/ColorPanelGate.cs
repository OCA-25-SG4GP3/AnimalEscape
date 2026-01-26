using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Cinemachine;
using System;
using static ColorPanelPuzzle;

[SelectionBase]
public class ColorPanelGate : MonoBehaviour
{    
    [SerializeField] private bool usingSides = true;
    [SerializeField, Header("ペアーのことでご注意。")] private int panelPairsRequired = 0;
    [SerializeField] private Transform cameraFollowObjectT;
    [SerializeField, Header("扉が開いたら、どこに動く")] private Transform[] playerAIMoveToTransform = new Transform[2];
    [SerializeField] bool isAddTimerOnClear = true;//追加:イザンさん,江頭 このゲートでタイマーを追加するかどうか
    [SerializeField, Header("前のゲート")] private ColorPanelGate previousGate;

    // Auto-detected components
    private GateDropController gateDropController;
    private ColorPanelRoomTimer colorPanelRoomTimer;
    [SerializeField] private CinemachineCamera sidewayCm; //not front
    private CinemachineCamera frontCm;
    private Animator gateAnimator;

    [SerializeField, ReadOnly, Header("ボタンの成功数（すべて）")] private int successfulPresses = 0;
    private bool gateOpened = false;
    private readonly List<ColorPanelPuzzle> allPanels = new();
    private readonly List<ColorPanelPuzzle> steppedPanels = new();

    [SerializeField] private GameObject correctEffect;
    GameManager gameManager;

    void Awake()
    {
        // Auto-detect components
        gateDropController = GetComponentInChildren<GateDropController>();
        gateAnimator = GetComponent<Animator>();
        colorPanelRoomTimer = FindAnyObjectByType<ColorPanelRoomTimer>();
        gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            sidewayCm = gameManager.SidewayCm;
            frontCm = gameManager.FrontCm;
        }
    }


    public void RegisterPanel(ColorPanelPuzzle panel)
    {
        if (!allPanels.Contains(panel))
            allPanels.Add(panel);
    }

    public void PanelStepped(ColorPanelPuzzle panel)
    {
        if (!steppedPanels.Contains(panel))
            steppedPanels.Add(panel);

        // Only panels currently stepped
        var activePanels = steppedPanels.Where(p => p.isStepped).ToList();

        // Check for matches by material
        for (int i = 0; i < activePanels.Count; i++)
        {
            for (int j = i + 1; j < activePanels.Count; j++)
            {
                ColorPanelPuzzle panelA = activePanels[i];
                ColorPanelPuzzle panelB = activePanels[j];
                bool isSameMesh = panelA.buttonType == panelB.buttonType;
                if (panelA.correctPanelMaterial.name == panelB.correctPanelMaterial.name && isSameMesh)
                {
                    if (usingSides && IsSameSide(panelA, panelB)) return;
                    // Matched pair!
                    OnSuccessfulMatch(panelA, panelB);
                    GameObject instans_A = Instantiate(correctEffect, panelA.transform.position, panelA.transform.rotation);
                    GameObject instans_B = Instantiate(correctEffect, panelB.transform.position, panelB.transform.rotation);

                    Destroy(instans_A, 2f);
                    Destroy(instans_B, 2f);

                    return; // stop after first pair
                }
            }
        }
    }

    private static bool IsSameSide(ColorPanelPuzzle panelA, ColorPanelPuzzle panelB)
    {
        return panelA.upSide && panelB.upSide || !panelA.upSide && !panelB.upSide;
    }

    public void PanelReleased(ColorPanelPuzzle panel)
    {
        steppedPanels.Remove(panel);
    }

    public bool IsGateOpened()
    {
        return gateOpened;
    }

    public bool CanAcceptPanels()
    {
        // Can accept panels if there's no previous gate, or if the previous gate is opened
        return previousGate == null || previousGate.IsGateOpened();
    }
    [SerializeField] private AudioClip correctSFX;

    private void OnSuccessfulMatch(ColorPanelPuzzle panelA, ColorPanelPuzzle panelB)
    {
        if (gateOpened) return; //so it doesnt process on all gates

        successfulPresses++;
        bool isFinalStep = successfulPresses >= panelPairsRequired;

        // Destroy matched panels safely (if not null - for debug cheat)
        if (panelA != null && panelB != null)
        {
            Destroy(panelA.gameObject);
            Destroy(panelB.gameObject);

            // Remove from both lists
            steppedPanels.Remove(panelA);
            steppedPanels.Remove(panelB);
            allPanels.Remove(panelA);
            allPanels.Remove(panelB);

            //
            GameObject audioObj = new GameObject("");
            //
            audioObj.transform.position = Camera.main.transform.position;
            //
            var audioSrc = audioObj.AddComponent<AudioSource>();

            ////////////
            audioSrc.clip = correctSFX;
            audioSrc.Play();

            //audioSrc.PlayOneShot(correctSFX);
            //audioSrc.PlayOneShot(correct2SFX);
            //audioSrc.PlayOneShot(correct3SFX);
            ////////////

            //2秒後消す
            Destroy(audioObj, 2.0f);

            //Instantiate(audioPrefab,,);
        }

        //  [SerializeField] GameObject audioPrefab;

        // Drop the gate step by step
        if (gateDropController)
        {
            gateDropController.DropStep(false);
        }

        if (isFinalStep)
        {
            OpenGateFully();
        }
    }
    public void OpenGateFully()
    {
        gateOpened = true;

        // Add time bonus
        if (colorPanelRoomTimer && isAddTimerOnClear)
        { colorPanelRoomTimer.AddTime(); }

        // Open gate animation
        //if (gateAnimator)        {            gateAnimator.Play("GateLift");        }

        // Auto walk players
        //AutoWalkPlayersToSpot();

        gateDropController.DropStep(true, OnGateFullyOpened);
    }

    private void OnGateFullyOpened()
    {
        // Move cameras to follow the next area (only after gate is fully opened)
        if (cameraFollowObjectT)
        {
            if (sidewayCm)
            {
                sidewayCm.Follow = cameraFollowObjectT;
            }

            if (frontCm)
            {
                frontCm.Follow = cameraFollowObjectT;
            }
        }

        if (playerAIMoveToTransform == null || playerAIMoveToTransform.Length < 2) return;

        var playerDistManager = FindAnyObjectByType<PlayerDistanceManager>();
        if (!playerDistManager) return;

        bool isLastGate = GameObject.FindObjectsByType<ColorPanelGate>(FindObjectsSortMode.None).All(g => g == this || g.IsGateOpened());

        var player1AnimalControl = playerDistManager.Player1.GetComponent<AnimalControlSimple>();
        if (player1AnimalControl && playerAIMoveToTransform[0])
        {
            player1AnimalControl.UnlockInput(); // Unlock before setting AI control
            player1AnimalControl.SetMoveTo(playerAIMoveToTransform[0].position, isLastGate);
        }

        var player2AnimalControl = playerDistManager.Player2.GetComponent<AnimalControlSimple>();
        if (player2AnimalControl && playerAIMoveToTransform[1])
        {
            player2AnimalControl.UnlockInput(); // Unlock before setting AI control
            player2AnimalControl.SetMoveTo(playerAIMoveToTransform[1].position, isLastGate);
        }


    }
}
