using System.Collections.Generic;
using UnityEngine;

public class CameraWallCuller : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float fadeAlpha = 0.3f;

    private Dictionary<Renderer, Color> fadedWalls = new Dictionary<Renderer, Color>();

    void Start()
    {
        target = FindAnyObjectByType<PlayerInfo>().transform;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 dir = target.position - transform.position;
        float distance = dir.magnitude;

        // Restore all previously faded walls
        foreach (var kvp in fadedWalls)
        {
            if (kvp.Key)
            {
                Color c = kvp.Key.material.color;
                c.a = kvp.Value.a;
                kvp.Key.material.color = c;
            }
        }
        fadedWalls.Clear();

        // Cast ray and collect all walls along the path
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, distance, wallLayer);
        Debug.DrawLine(transform.position, target.position, Color.green, 1f); // visualize the ray
        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (!rend) continue;

            // Make a material instance to avoid affecting others
            if (!rend.material.name.EndsWith("(Instance)"))
                rend.material = new Material(rend.material);

            // Save original color
            if (!fadedWalls.ContainsKey(rend))
                fadedWalls[rend] = rend.material.color;

            Color col = rend.material.color;
            col.a = fadeAlpha;
            rend.material.color = col;
        }
    }
}
