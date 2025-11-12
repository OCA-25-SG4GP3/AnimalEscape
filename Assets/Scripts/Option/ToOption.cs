using UnityEngine;
using UnityEngine.SceneManagement;

public class ToOption : MonoBehaviour
{
    //PlayerPrefsに保存するためのキー
    private const string BEFORE_SCENE_KEY = "BeforeSceneName";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {       
        
    }

    //ボタンが押された場合、今回呼び出される関数
    public void  OnClick()
    {        
        LoadOptionScene();       
    }

    public void LoadOptionScene()
    {
        //現在のシーンの名前を取得
        string currentSceneName = SceneManager.GetActiveScene().name;
        //シーンの名前をデバッグログに出力
        Debug.Log("前回のシーンは" + currentSceneName);

        //PlayerPrefsに名前を保存
        PlayerPrefs.SetString(BEFORE_SCENE_KEY, currentSceneName);
        // 確実に保存
        PlayerPrefs.Save(); 

        //次のシーンをロード
        SceneManager.LoadScene("OptionMenu");
    }
}
