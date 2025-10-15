using UnityEngine;
using System.Collections.Generic;

public class CheckpointTeleporter : MonoBehaviour
{
    [SerializeField][HeaderAttribute("場所の空っぽオブジェクト")] public List<Transform> checkpoints; // assign in inspector
    private int currentIndex = 0;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var playerInfos = PlayerInfoSystem.GetPlayerInfos();
            foreach (PlayerInfo playerInfo in playerInfos)
            {
                TeleportToNextCheckpoint(playerInfo.transform);
                // Increment index and loop back to 0 if past last
            }
            currentIndex++;
            if (currentIndex >= checkpoints.Count)
                currentIndex = 0;
        }
    }

    void TeleportToNextCheckpoint(Transform playerT)
    {
        if (checkpoints.Count == 0 || playerT == null) return;

        // Teleport player to current checkpoint
        playerT.position = checkpoints[currentIndex].position;


    }
}
