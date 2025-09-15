using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private CinemachineTargetGroup _targetGroup;


    [Header("Listening to")]
    [SerializeField] protected VoidEventSO _onEnterGameEvent;
    [SerializeField] protected VoidEventSO _onFinishIntroEvent;

    private PlayerInput _player1;
    private PlayerInput _player2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _player1 = PlayerInput.Instantiate(_playerPrefab[0], controlScheme: "Player1", pairWithDevices: new[] { Keyboard.current });
        _player1.transform.position = _spawnPoints[0].position;

        _targetGroup.AddMember(_player1.transform, 1f, 2f);

        _player2 = PlayerInput.Instantiate(_playerPrefab[1], controlScheme: "Player2", pairWithDevices: new[] { Keyboard.current });
        _player2.transform.position = _spawnPoints[1].position;

        _targetGroup.AddMember(_player2.transform, 1f, 2f);
    }

    protected virtual void OnEnable()
    {
        _onEnterGameEvent.OnEventInvoked += DisableInput;
        _onFinishIntroEvent.OnEventInvoked += EnableInput;
    }

    protected virtual void OnDisable()
    {
        _onEnterGameEvent.OnEventInvoked -= DisableInput;
        _onFinishIntroEvent.OnEventInvoked -= EnableInput;
    }

        protected virtual void DisableInput()
    {
        _player1.enabled = false;
        _player2.enabled = false;
    }

    protected virtual void EnableInput()
    {
        _player1.enabled = true;
        _player2.enabled = true;
    }

}
