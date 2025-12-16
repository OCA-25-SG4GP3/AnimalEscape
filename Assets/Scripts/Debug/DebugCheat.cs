using UnityEngine;

public class DebugCheat : MonoBehaviour
{
    [SerializeField] private PlayerDistanceManager playerDistanceManager;
    [Header("Toggle Key")] public KeyCode toggleKey = KeyCode.Return;

    void Update()
    {
        if (Input.GetKey(toggleKey))
        {
            //playerDistanceManager.Player1.GetComponent<>();
        }

        // Debug cheat: Press Alpha2 to open the current active gate
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            var allGates = FindObjectsByType<ColorPanelGate>(FindObjectsSortMode.None);
            var activeGate = System.Array.Find(allGates, g => g.CanAcceptPanels() && !g.IsGateOpened());

            if (activeGate != null)
            {
                Debug.Log($"DEBUG CHEAT : Alpha2 - Opening {activeGate.gameObject.name}");
                activeGate.OpenGate();
            }
        }
    }
}
