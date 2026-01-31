using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private ClearTime _clearTimeSO;
    //編集: 江頭 プレイヤーが捕まった時点では「脱出失敗」のUIを出さないようにします
    //[SerializeField] private GameObject finishImageObject;
    private GameObject fadeObject;
    [SerializeField] private bool setFinishImageOnClearGame = false;

    //[SerializeField] private Text _clearTimeText;
    [SerializeField] private string nextSceneName = "NextScene"; // 次に移動するシーン�?
    [NonSerializedAttribute] public bool isFinish = false;
    static public bool FadeOut = false;

    private void Start()
    {
        FadeOut = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Player タグに当たったらシーン遷移
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);
            playerFinishCount++;
            if (playerFinishCount >= 2) //2 Players are in
                SetClearGameByFinish();
        }
    }
    [SerializeField] private float fadeOutDelay = 0.0f;
    [SerializeField] private float loadNextSceneDelay = 0.0f;
    public void SetClearGameByFinish()
    {
        if (isFinish) return;

        isFinish = true;
        Invoke("SetFadeOut", fadeOutDelay);
        //編集:江頭 プレイヤーがクリアした時点では「脱出成功」のUIを出さないようにしました
        //if (setFinishImageOnClearGame) finishImageObject.SetActive(true);
        Invoke("LoadNextScene", loadNextSceneDelay);
    }

    private void SetFadeOut()
    {
        //FadeInImgManagerへ
        FadeOut = true;
    }

    void LoadNextScene() { SceneManager.LoadScene(nextSceneName); }

    int playerFinishCount = 0;
}
