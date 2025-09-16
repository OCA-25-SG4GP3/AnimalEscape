using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _totalGimmickCount;
    [SerializeField] private int _clearedGimmickCount = 0;
    [SerializeField] private GameObject _exitArea;
    [SerializeField] private VoidEventSO _onEnterGameEvent;
    [SerializeField] private VoidEventSO _onFinishIntroEvent;
    [SerializeField] private VoidEventSO _onGimmickClearEvent;
    [SerializeField] private VoidEventSO _onAllGimmickClearEvent;
    [SerializeField] private VoidEventSO _onGameClearEvent;
    [SerializeField] private ClearTime _clearTimeSO;
    [SerializeField] private AudioClip _mainBGM;
    [SerializeField] private AudioClip _gameClearBGM;
    [SerializeField] private AudioClipEventSO _playBGMEvent;

    private bool _gameCleared = false;
    private bool _startTimer = false;
    private float _elapsedTime = 0f;

    private void OnEnable()
    {
        _onFinishIntroEvent.OnEventInvoked += StartTimer;
        _onGimmickClearEvent.OnEventInvoked += GimmickClear;
        _onGameClearEvent.OnEventInvoked += GameClear;
    }

    private void OnDisable()
    {
        _onFinishIntroEvent.OnEventInvoked -= StartTimer;
        _onGimmickClearEvent.OnEventInvoked -= GimmickClear;
        _onGameClearEvent.OnEventInvoked -= GameClear;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _totalGimmickCount = GameObject.FindGameObjectsWithTag("Gimmick").Length;
        _onEnterGameEvent.InvokeEvent();
        _playBGMEvent.InvokeEvent(_mainBGM, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (_startTimer)
        {
            _elapsedTime += Time.deltaTime;
            Debug.Log($"elapsed Time: {_elapsedTime} seconds");

        }

        if (_clearedGimmickCount >= _totalGimmickCount)
        {
            _onAllGimmickClearEvent.InvokeEvent();
        }
    }

    private void StartTimer()
    {
        _elapsedTime = 0f;
        _startTimer = true;
    }

    private void GimmickClear()
    {
        _clearedGimmickCount++;
        _playBGMEvent.InvokeEvent(_mainBGM, true);

    }

    private void GameClear()
    {
        if (_gameCleared) return;
        _startTimer = false;
        _clearTimeSO.TimeInSeconds = _elapsedTime;
        _gameCleared = true;
        _playBGMEvent.InvokeEvent(_gameClearBGM, false);
        SceneManager.LoadScene(2, LoadSceneMode.Additive);
    }
}
