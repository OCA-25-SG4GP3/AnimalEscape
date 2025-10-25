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
    public void OnClick()
    {        
        //前回のシーンをロードする
        SceneManager.LoadScene(beforeSceneName);
        // （オプション）データを受け取ったらPlayerPrefsから削除することが推奨されます
        PlayerPrefs.DeleteKey(BEFORE_SCENE_KEY);
    }
}
