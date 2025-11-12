using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

[System.SerializableAttribute]
public class GateAndButtonsRequired
{
    [SerializeField] public GameObject gate;
    [SerializeField] public int panelsRequired = 0;
    [SerializeField] public Transform cameraFollowObjectT;
    [SerializeField, Header("扉が開いたら、どこに動く")] public Transform[] playerAIMoveToTransform = new Transform[2];
}
public class ColorPanelManager : MonoBehaviour
{
    [SerializeField] bool usingSides = true;
    [SerializeField] private List<GateAndButtonsRequired> gatesInOrder = new(); //If these objects are activated together, trigger the event 
    [SerializeField] private ColorPanelRoomTimer colorPanelRoomTimer;
    [SerializeField] private CinemachineCamera cm;
    int point = 0;
    int currentGateIndex = 0;
    private List<ColorPanelPuzzle> allPanels = new();
    private List<ColorPanelPuzzle> steppedPanels = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) NextGateIndex();
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
                if (panelA.correctPanelMaterial.name == panelB.correctPanelMaterial.name)
                {
                    if (usingSides && IsSameSide(panelA, panelB)) return;
                    // Matched pair!
                    AccumulatePoint();

                    // Destroy matched panels safely
                    Destroy(activePanels[i].gameObject);
                    Destroy(activePanels[j].gameObject);

                    // Remove from steppedPanels
                    steppedPanels.Remove(activePanels[i]);
                    steppedPanels.Remove(activePanels[j]);
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

    public void AccumulatePoint()
    {
        point++;
        int numbersOfPanelsRequired = gatesInOrder[currentGateIndex].panelsRequired;
        if (point >= numbersOfPanelsRequired)
        {
            NextGateIndex();
        }
    }

    private void NextGateIndex()
    {
        OpenGateAndNextCamera();
        AutoWalkPlayersToSpot();
        currentGateIndex++;
    }

    void AutoWalkPlayersToSpot()
    {
        GateAndButtonsRequired gate = gatesInOrder[currentGateIndex];
        var playerDistManager = GameObject.FindAnyObjectByType<PlayerDistanceManager>();
        var player1AnimalControl = playerDistManager.Player1.GetComponent<AnimalControlSimple>();
        if (player1AnimalControl) player1AnimalControl.SetMoveTo(gate.playerAIMoveToTransform[0].position);
        var player2AnimalControl = playerDistManager.Player2.GetComponent<AnimalControlSimple>();
        if (player2AnimalControl) player2AnimalControl.SetMoveTo(gate.playerAIMoveToTransform[1].position);
    }
    void OpenGateAndNextCamera()
    {
        GameObject gate = gatesInOrder[currentGateIndex].gate;
        cm.Follow = gatesInOrder[currentGateIndex].cameraFollowObjectT;
        colorPanelRoomTimer.AddTime();
        //Destroy(gate);
        gate.GetComponent<Animator>().Play("GateLift");
    }

}
