using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.SerializableAttribute]
public class GateAndButtonsRequired
{
    public GameObject gate;
    public int panelsRequired = 0;
}
public class ColorPanelManager : MonoBehaviour
{
    [SerializeField] private List<GateAndButtonsRequired> gatesInOrder = new(); //If these objects are activated together, trigger the event 
    int point = 0;
    int curentGateIndex = 0;

    public void AccumulatePoint()
    {
        point++;
        int numbersOfPanelsRequired = gatesInOrder[curentGateIndex].panelsRequired;
        if (point >= numbersOfPanelsRequired)
        {
            OpenGate();
            point = 0;
        }
    }
    void OpenGate()
    {
        GameObject gate = gatesInOrder[curentGateIndex].gate;
        if (gate) Destroy(gate);
        curentGateIndex++;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnColorPanelsActivatedTogether()
    {

    }

}
