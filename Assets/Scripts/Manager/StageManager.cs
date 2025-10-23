using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    private bool _startTimer = false;
    private float _elapsedTime = 0f;
    private bool _stageCleared = false;
    [SerializeField] private VoidEventSO _stageClearEvent;
    [SerializeField] private ClearTime _clearTimeSO;

    [SerializeField] private RankingManager _rankingManager;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (_startTimer)
        {
            _elapsedTime += Time.deltaTime;
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
        // SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }
}
