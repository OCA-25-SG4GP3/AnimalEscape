using UnityEngine;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private ClearTime _clearTimeSO;
    [SerializeField] private Text _clearTimeText;
    private void Start()
    {
        Debug.Log($"Clear Time: {_clearTimeSO.TimeInSeconds} seconds");
        _clearTimeText.text = $"使用時間\n{_clearTimeSO.TimeInSeconds:F2}s";
    }
}
