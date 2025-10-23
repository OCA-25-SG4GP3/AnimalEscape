using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class StageManager : MonoBehaviour
{
    private bool _startTimer = false;
    private float _elapsedTime = 0f;
    private bool _stageCleared = false;
    [SerializeField] private VoidEventSO _stageClearEvent;
    [SerializeField] private ClearTime _clearTimeSO;
    [SerializeField] private AudioClip _mainBGM;
    [SerializeField] private AudioClip _gameClearBGM;
    [SerializeField] private RankingManager _rankingManager;
    [SerializeField] private AudioClipEventSO _playBGMEvent;

    [SerializeField] private bool _isLoading = false;
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField, ReadOnly] private float _progress;

    void OnEnable()
    {
        _stageClearEvent.OnEventInvoked += StageClear;
    }

    void OnDisable()
    {
        _stageClearEvent.OnEventInvoked -= StageClear;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _startTimer = true;
        _playBGMEvent.InvokeEvent(_mainBGM, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (_startTimer)
        {
            _elapsedTime += Time.deltaTime;
        }

        if (_stageCleared)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_isLoading)
            {
                _isLoading = true;
                LoadNextScene();
            }
        }
    }

    async void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        var x = SceneUtility.GetScenePathByBuildIndex(currentIndex + 1);
        Debug.Log(x);
        AsyncOperation loadingScene;
        if (x == "")
        {
            loadingScene = SceneManager.LoadSceneAsync(0);
        }
        else
        {
            loadingScene = SceneManager.LoadSceneAsync(currentIndex+1);
        }
        loadingScene.allowSceneActivation = false;

        _loadingScreen.SetActive(true);
        while (!loadingScene.isDone)
        {
            Debug.Log($"Loading progress: {loadingScene.progress * 100}%");
            _progress = loadingScene.progress;

            if (loadingScene.progress >= 0.9f)
            {
                _progress = 1;
                await Task.Delay(1000);

                loadingScene.allowSceneActivation = true;
            }
            await Task.Yield();
        }
    }

    void StageClear()
    {
        _startTimer = false;
        _clearTimeSO.TimeInSeconds = _elapsedTime;
        _stageCleared = true;
        // _playBGMEvent.InvokeEvent(_gameClearBGM, false);

        _rankingManager.AddTime(_elapsedTime);
        _rankingManager.ShowRanking();
        _playBGMEvent.InvokeEvent(_gameClearBGM, false);
    }
}
