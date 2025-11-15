using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    [SerializeField] private GameObject[] _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private CinemachineTargetGroup _targetGroup;


    [Header("Listening to")]
    [SerializeField] protected VoidEventSO _onEnterGameEvent;
    [SerializeField] protected VoidEventSO _onFinishIntroEvent;

    private PlayerInput _player1; public PlayerInput Player1 => _player1;
    private PlayerInput _player2; public PlayerInput Player2 => _player2;
    protected override void Awake()
    {
        base.Awake();
        SpawnPlayers();
    }

    [NonSerializedAttribute] public List<PlayerInput> players = new List<PlayerInput>();
    void SpawnPlayers()
    {
        if (_playerPrefab.Length < 1 || _spawnPoints.Length < 1)
        {
            Debug.LogError("Need at least 1 prefab and 1 spawn point.");
            return;
        }

        // Player 1
        _player1 = PlayerInput.Instantiate(_playerPrefab[0], controlScheme: "Player1", pairWithDevices: new[] { Keyboard.current });
        _player1.transform.position = _spawnPoints[0].position;
        _player1.transform.Rotate(0, 180, 0);
        _targetGroup.AddMember(_player1.transform, 1f, 2f);
        PlayerInfoSystem.playerInfos[0] = _player1.GetComponent<PlayerInfo>();
        players.Add(_player1); // add to list

        // Player 2
        if (_playerPrefab.Length >= 2 && _spawnPoints.Length >= 2)
        {
            _player2 = PlayerInput.Instantiate(_playerPrefab[1], controlScheme: "Player2", pairWithDevices: new[] { Keyboard.current });
            _player2.transform.position = _spawnPoints[1].position;
            _targetGroup.AddMember(_player2.transform, 1f, 2f);
            PlayerInfoSystem.playerInfos[1] = _player2.GetComponent<PlayerInfo>();
            players.Add(_player2); // add to list
        }
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
        _player1.SwitchCurrentControlScheme("Player1", devices: new[] { Keyboard.current });
        _player2.enabled = true;
        _player2.SwitchCurrentControlScheme("Player2", devices: new[] { Keyboard.current });
    }

}
