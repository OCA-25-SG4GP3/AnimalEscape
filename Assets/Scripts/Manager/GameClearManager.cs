using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private ClearTime _clearTimeSO;
    [SerializeField] private Text _clearTimeText;
    [SerializeField] private string nextSceneName = "NextScene"; // 次に移動するシーン名
    private void Start()
    {
        Debug.Log($"Clear Time: {_clearTimeSO.TimeInSeconds} seconds");
        if(_clearTimeText) _clearTimeText.text = $"使用時間\n{_clearTimeSO.TimeInSeconds:F2}s";
    }

    void OnCollisionEnter(Collision collision)
    {
        // Player タグに当たったらシーン遷移
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
