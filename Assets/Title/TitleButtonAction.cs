using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonAction : MonoBehaviour
{
    [SerializeField] private string _loadGameScene;
    [SerializeField] private string _loadCreditScene;
    public int _delay; //遅延させたい秒数

    public void TimeLag()
    {
        Invoke("SceneChange", _delay);
    }

    public void SceneChange(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    //スタートボタンが押されると
    public void StartOnClick()
    {
        SceneChange(_loadGameScene);
        Debug.Log("ゲーム画面へ!");  // ログを出力
    }
    //クレジットボタンが押されると
    public void CreditOnClick()
    {
        SceneChange(_loadCreditScene);
        Debug.Log("クレジット画面へ!");  // ログを出力
    }
    //エンドボタンが押されると
    public void EndOnClick()
    {
        Debug.Log("ゲームを終了!");  // ログを出力
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

}
