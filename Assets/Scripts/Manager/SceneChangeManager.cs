using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChangeManager : MonoBehaviour
{
    // どこからでもアクセスするための静的プロパティ
    public static SceneChangeManager Instance { get; private set; }
    // 次のシーン番号(現状: 0タイトル 1ゲーム 2スコア)
    private int _nextScene = 0;

    private void Awake()
    {
        // インスタンスがすでに存在している場合は自分を消す
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // シングルトン化
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void SceneChange()
    {
        _nextScene++;
        if (_nextScene > SceneManager.sceneCountInBuildSettings - 1)
        {
            _nextScene = 0;
        }
        SceneManager.LoadScene(_nextScene);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
