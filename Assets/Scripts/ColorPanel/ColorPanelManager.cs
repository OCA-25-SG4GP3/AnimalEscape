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
}
public class ColorPanelManager : MonoBehaviour
{
    //このギミックは、色で分けるではないです。
    //まずは、
    //1.「マテリアルは同じですか？」からチェックする。
    //2.「両方は、上側と下側ですか？。同じ側なら、だめ。」

    [SerializeField] private List<GateAndButtonsRequired> gatesInOrder = new(); //If these objects are activated together, trigger the event 
    [SerializeField] private ColorPanelRoomTimer colorPanelRoomTimer;
    [SerializeField] private CinemachineCamera cm;
    int point = 0;
    int curentGateIndex = 0;
    private List<ColorPanelPuzzle> allPanels = new();
    private List<ColorPanelPuzzle> steppedPanels = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) OpenGateAndNextCamera();
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
                if (panelA.panelMaterial.name == panelB.panelMaterial.name)
                {
                    if (IsSameSide(panelA, panelB)) return;
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
        int numbersOfPanelsRequired = gatesInOrder[curentGateIndex].panelsRequired;
        if (point >= numbersOfPanelsRequired)
        {
            OpenGateAndNextCamera();
            
        }
    }
    void OpenGateAndNextCamera()
    {
        GameObject gate = gatesInOrder[curentGateIndex].gate;
        cm.Follow = gatesInOrder[curentGateIndex].cameraFollowObjectT;
        colorPanelRoomTimer.AddTime();
        Destroy(gate);
        curentGateIndex++;

    }

}
