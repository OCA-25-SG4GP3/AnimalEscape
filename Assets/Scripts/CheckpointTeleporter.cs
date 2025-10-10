using UnityEngine;
using System.Collections.Generic;

public class CheckpointTeleporter : MonoBehaviour
{
    public List<Transform> checkpoints; // assign in inspector
    private int currentIndex = 0;
    public Transform player; // assign the player transform

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if(!player) player = GameObject.FindGameObjectWithTag("Player").transform;
            TeleportToNextCheckpoint();
        }
    }

    void TeleportToNextCheckpoint()
    {
        if (checkpoints.Count == 0 || player == null) return;

        // Teleport player to current checkpoint
        player.position = checkpoints[currentIndex].position;

        // Increment index and loop back to 0 if past last
        currentIndex++;
        if (currentIndex >= checkpoints.Count)
            currentIndex = 0;
    }
}
