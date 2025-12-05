using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private ClearTime _clearTimeSO;
    [SerializeField] private GameObject finishImageObject;

    //[SerializeField] private Text _clearTimeText;
    [SerializeField] private string nextSceneName = "NextScene"; // 次に移動するシーン�?
    bool isFinish = false;
    private void Start()
    {
        Debug.Log($"Clear Time: {_clearTimeSO.TimeInSeconds} seconds");
        //  if(_clearTimeText) _clearTimeText.text = $"使用時間\n{_clearTimeSO.TimeInSeconds:F2}s";
    }

    void OnCollisionEnter(Collision collision)
    {
        // Player タグに当たったらシーン遷移
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);
            playerFinishCount++;
            if (playerFinishCount >= 2) //2 Players are in
                ClearGameByFinish();
        }
    }
    void ClearGameByFinish()
    {
        if (isFinish) return;
        isFinish = true;
        finishImageObject.SetActive(true);
        Invoke("LoadNextScene", 3.0f);
    }
    void LoadNextScene() { SceneManager.LoadScene(nextSceneName); }

    int playerFinishCount = 0;
}
