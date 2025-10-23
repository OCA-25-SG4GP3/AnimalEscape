using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CheckpointTeleporter : MonoBehaviour
{
    [SerializeField] private IntEventSO _onCheckpointTriggerEvent;
    [SerializeField] private VoidEventSO _onFailEvent;
    [SerializeField][HeaderAttribute("�ꏊ�̋���ۃI�u�W�F�N�g")] public List<Transform> checkpoints; // assign in inspector
    [SerializeField]private int currentIndex = 0;

    void OnEnable()
    {
        _onCheckpointTriggerEvent.OnEventInvoked += SetCheckpoint;
        _onFailEvent.OnEventInvoked += TeleportTriggered;
    }

    void OnDisable()
    {
        _onCheckpointTriggerEvent.OnEventInvoked -= SetCheckpoint;
        _onFailEvent.OnEventInvoked -= TeleportTriggered;
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            currentIndex++;
            if (currentIndex >= checkpoints.Count)
                currentIndex = 0;
            TeleportTriggered();
        }
    }

    void TeleportToNextCheckpoint(Transform playerT)
    {
        if (checkpoints.Count == 0 || playerT == null) return;

        // Teleport player to current checkpoint
        playerT.position = checkpoints[currentIndex].position;


    }

    void SetCheckpoint(int id)
    {
        currentIndex = id;
    }

    void TeleportTriggered()
    {
        Debug.Log("aa");
        var playerInfos = PlayerInfoSystem.GetPlayerInfos();
        foreach (PlayerInfo playerInfo in playerInfos)
        {
            TeleportToNextCheckpoint(playerInfo.transform);
            // Increment index and loop back to 0 if past last
        }
    }
}
