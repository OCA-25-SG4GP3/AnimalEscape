using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SphereCollider))]
public class CameraWallCuller : MonoBehaviour
{
    [SerializeField, Header("自動的にプレイヤータグを見つけます")] private Transform[] targets;
    [SerializeField] private LayerMask wallLayer1;
    [SerializeField] private LayerMask wallLayer2;

    private Dictionary<Renderer, ShadowCastingMode> hiddenWalls = new();
    private HashSet<Renderer> currentFrameHits = new();
    private List<Collider> triggerHits1 = new();
    private List<Collider> triggerHits2 = new();

    void Start()
    {
        PlayerInfo[] playerInfos = PlayerInfoSystem.GetPlayerInfos();
        targets = new Transform[playerInfos.Length];
        for (int i = 0; i < playerInfos.Length; i++)
        {
            targets[i] = playerInfos[i].gameObject.transform;
        }

        SphereCollider col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        if (col.radius <= 0f)
            col.radius = 0.5f;
    }

    void LateUpdate()
    {
        currentFrameHits.Clear();

        foreach (var target in targets)
        {
            if (!target) continue;

            Vector3 dir = target.position - transform.position;
            float distance = dir.magnitude;

            // Raycast for both wall layers
            RaycastHit[] hits1 = Physics.RaycastAll(transform.position, dir, distance, wallLayer1);
            foreach (var hit in hits1) AddHit(hit.collider);

            RaycastHit[] hits2 = Physics.RaycastAll(transform.position, dir, distance, wallLayer2);
            foreach (var hit in hits2) AddHit(hit.collider);
        }

        // Trigger hits for both layers
        foreach (var col in triggerHits1) AddHit(col);
        foreach (var col in triggerHits2) AddHit(col);

        // Apply ShadowsOnly for all hits this frame
        foreach (var rend in currentFrameHits) HideWallRenderer(rend);

        // Restore walls not hit this frame
        List<Renderer> toRestore = new();
        foreach (var kvp in hiddenWalls)
            if (!currentFrameHits.Contains(kvp.Key))
                toRestore.Add(kvp.Key);

        foreach (var r in toRestore)
        {
            r.shadowCastingMode = hiddenWalls[r];
            hiddenWalls.Remove(r);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & wallLayer1) != 0)
        {
            if (!triggerHits1.Contains(other))
                triggerHits1.Add(other);
        }
        else if (((1 << other.gameObject.layer) & wallLayer2) != 0)
        {
            if (!triggerHits2.Contains(other))
                triggerHits2.Add(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & wallLayer1) != 0)
            triggerHits1.Remove(other);
        else if (((1 << other.gameObject.layer) & wallLayer2) != 0)
            triggerHits2.Remove(other);
    }

    private void AddHit(Collider col)
    {
        Renderer rend = col.GetComponent<Renderer>();
        if (rend) currentFrameHits.Add(rend);
    }

    private void HideWallRenderer(Renderer rend)
    {
        if (!hiddenWalls.ContainsKey(rend))
            hiddenWalls[rend] = rend.shadowCastingMode;

        rend.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    }
}
