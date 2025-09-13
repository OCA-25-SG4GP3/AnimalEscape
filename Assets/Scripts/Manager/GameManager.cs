using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _totalGimmickCount;
    [SerializeField] private int _clearedGimmickCount = 0;
    [SerializeField] private GameObject _exitArea;
    [SerializeField] private VoidEventSO _onGimmickClearEvent;
    [SerializeField] private VoidEventSO _onAllGimmickClearEvent;
    [SerializeField] private VoidEventSO _onGameClearEvent;



    private void OnEnable()
    {
        _onGimmickClearEvent.OnEventInvoked += GimmickClear;
        _onGameClearEvent.OnEventInvoked += GameClear;
    }

    private void OnDisable()
    {
        _onGimmickClearEvent.OnEventInvoked -= GimmickClear;
        _onGameClearEvent.OnEventInvoked -= GameClear;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _totalGimmickCount = GameObject.FindGameObjectsWithTag("Gimmick").Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (_clearedGimmickCount >= _totalGimmickCount)
        {
            _onAllGimmickClearEvent.InvokeEvent();
        }
    }

    private void GimmickClear()
    {
        _clearedGimmickCount++;


    }

    private void GameClear()
    {
        Debug.Log("GameClear");
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }
}
