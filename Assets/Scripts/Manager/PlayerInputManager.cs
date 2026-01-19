using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    [SerializeField] private GameObject[] _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private CinemachineTargetGroup _targetGroup;
    [SerializeField] private bool followZAxis = true;
    [SerializeField] private bool autoTargetToTargetGroup = true;

    [Header("Listening to")]
    [SerializeField] protected VoidEventSO _onEnterGameEvent;
    [SerializeField] protected VoidEventSO _onFinishIntroEvent;

    private PlayerInput _player1; public PlayerInput Player1 => _player1;
    private PlayerInput _player2; public PlayerInput Player2 => _player2;
    private GameObject _player1CameraTarget;
    private GameObject _player2CameraTarget;

    protected override void Awake()
    {
        base.Awake();

        // Set the camera to track the TargetGroup (only if enabled)
        if (autoTargetToTargetGroup && _gameManager != null && _gameManager.FrontCm != null && _targetGroup != null)
        {
            _gameManager.FrontCm.Target.TrackingTarget = _targetGroup.transform;
        }

        SpawnPlayers();
    }

    [NonSerializedAttribute] public List<PlayerInput> players = new List<PlayerInput>();
    void SpawnPlayers()
    {
        if (_playerPrefab.Length == 0 || _spawnPoints.Length == 0)
        {
            //When not using any players
           // Debug.LogError("Need at least 1 prefab and 1 spawn point.");
            return;
        }

        // Player 1
        _player1 = PlayerInput.Instantiate(_playerPrefab[0], controlScheme: "Player1", pairWithDevices: new[] { Keyboard.current });
        _player1.transform.position = _spawnPoints[0].position;
        _player1.transform.Rotate(0, 180, 0);

        // Create camera target for Player 1 (follows horizontal position only)
        _player1CameraTarget = new GameObject("Player1_CameraTarget");
        _player1CameraTarget.transform.position = _player1.transform.position;
        var follow1 = _player1CameraTarget.AddComponent<FollowPlayerXZ>();
        follow1.playerTransform = _player1.transform;
        follow1.SetFollowZ(followZAxis);

        if (_targetGroup != null)
        {
            _targetGroup.AddMember(_player1CameraTarget.transform, 1f, 2f);
        }

        PlayerInfoSystem.playerInfos[0] = _player1.GetComponent<PlayerInfo>();
        players.Add(_player1); // add to list

        // Player 2
        if (_playerPrefab.Length >= 2 && _spawnPoints.Length >= 2)
        {
            _player2 = PlayerInput.Instantiate(_playerPrefab[1], controlScheme: "Player2", pairWithDevices: new[] { Keyboard.current });
            _player2.transform.position = _spawnPoints[1].position;

            // Create camera target for Player 2 (follows horizontal position only)
            _player2CameraTarget = new GameObject("Player2_CameraTarget");
            _player2CameraTarget.transform.position = _player2.transform.position;
            var follow2 = _player2CameraTarget.AddComponent<FollowPlayerXZ>();
            follow2.playerTransform = _player2.transform;
            follow2.SetFollowZ(followZAxis);

            if (_targetGroup != null)
            {
                _targetGroup.AddMember(_player2CameraTarget.transform, 1f, 2f);
            }

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
