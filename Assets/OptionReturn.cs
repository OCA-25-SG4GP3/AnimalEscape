using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionReturn : MonoBehaviour
{
    // PlayerPrefsに保存したキー
    private const string BEFORE_SCENE_KEY = "BeforeSceneName";
    //前のシーンの名前を格納する
    string beforeSceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // PlayerPrefsから前のシーンの名前を読み込み
        beforeSceneName = PlayerPrefs.GetString(BEFORE_SCENE_KEY, "DefaultName");

        Debug.Log($"前のシーンの名前: {beforeSceneName}");       
    }
    public void LoadBeforeScene()
    {        
        //前回のシーンをロードする
        SceneManager.LoadScene(beforeSceneName);
        // （オプション）データを受け取ったらPlayerPrefsから削除することが推奨されます
        PlayerPrefs.DeleteKey(BEFORE_SCENE_KEY);
    }

    public void OptionToEnd()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
    #else
        Application.Quit();//ゲームプレイ終了
    #endif
        Debug.Log("ゲームを終了します");
    }
    public void OptionToTitle()
    {
        //前回のシーンをロードする
        SceneManager.LoadScene("Title");

        Debug.Log("タイトル画面に移動します");
    }
}
