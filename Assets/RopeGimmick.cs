using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class RopeGimmick : MonoBehaviour
{
    [SerializeField] private Transform playerA;
    private Transform playerARopePointT;
    [SerializeField] private Transform playerB;
    private Transform playerBRopePointT;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float pullForce = 20f;
    [SerializeField] private int ropeSegments = 20;
    [SerializeField] private float ropeSag = 0.3f;

    private LineRenderer line;
    private Vector3[] ropePoints;

    void Start()
    {
        PlayerInfo[] infos = PlayerInfoSystem.GetPlayerInfos();
        if (!PlayerInfoSystem.Is2PlayersConnected())
        {
            enabled = false;
            return;
        }

        playerA = infos[0].transform;
        playerB = infos[1].transform;

        playerARopePointT = infos[0].ropePointT;
        playerBRopePointT = infos[1].ropePointT;

        line = GetComponent<LineRenderer>();
        line.positionCount = ropeSegments;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.useWorldSpace = true;

        ropePoints = new Vector3[ropeSegments];
    }

    void LateUpdate()
    {
        if (!playerA || !playerB) return;

        ApplyPullForce();
        UpdateRopeVisual();
    }

    private void ApplyPullForce()
    {
        Vector3 diff = playerB.position - playerA.position;
        float distance = diff.magnitude;

        if (distance <= maxDistance)
            return;

        Vector3 dir = diff.normalized;
        dir.y = 0; // keep horizontal

        float stretch = distance - maxDistance;
        float forceAmount = stretch * pullForce;

        if (playerA.TryGetComponent(out Rigidbody rbA))
            rbA.AddForce(dir * forceAmount * Time.deltaTime, ForceMode.VelocityChange);

        if (playerB.TryGetComponent(out Rigidbody rbB))
            rbB.AddForce(-dir * forceAmount * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void UpdateRopeVisual()
    {
        for (int i = 0; i < ropeSegments; i++)
        {
            float t = i / (float)(ropeSegments - 1);
            Vector3 pos = Vector3.Lerp(playerARopePointT.position, playerBRopePointT.position, t);

            // add a slight curve (rope sag)
            pos.y -= Mathf.Sin(t * Mathf.PI) * ropeSag;

            ropePoints[i] = pos;
        }

        line.SetPositions(ropePoints);
    }
}
