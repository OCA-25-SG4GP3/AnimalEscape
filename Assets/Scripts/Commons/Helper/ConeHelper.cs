using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.SerializableAttribute]
public struct ConeInfo
{
    public ConeInfo(Vector3 coneForward, Vector3 coneMiddle, float maxDistance, float coneAngle)
    {
        this.coneForward = coneForward;
        this.coneMiddle = coneMiddle;
        this.maxDistance = maxDistance;
        this.coneAngle = coneAngle;
    }
    public Vector3 coneForward;
    public Vector3 coneMiddle;
    public float maxDistance;
    public float coneAngle;
}

public static class ConeHelper
{
    public static GameObject CheckTargetsInCone(ConeInfo coneInfo, List<GameObject> targetsToCheck)
    {
        //準備したオブジェクトリストから、視野角にいるかどうかをチェック。
        //いたら、保存。
        GameObject foundTarget = null;
        float closestDistanceFound = float.MaxValue;
        foreach (GameObject potentialTarget in targetsToCheck)
        {
            Vector3 toTarget = potentialTarget.transform.position - coneInfo.coneMiddle;
            float distance = toTarget.magnitude;

            if (distance > coneInfo.maxDistance) { continue; }

            toTarget.Normalize();

            // Check angle between forward and target direction
            float angle = Vector3.Angle(coneInfo.coneForward, toTarget);

            if (angle <= coneInfo.coneAngle && distance < closestDistanceFound) // Inside cone　視野角の中
            {
                foundTarget = potentialTarget;
                closestDistanceFound = distance; //store
            }
        }

        return foundTarget;
    }

    public static void DrawConeGizmo(ConeInfo coneInfo) //DEBUG only
    {
        
        Vector3 pos = coneInfo.coneMiddle;

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(pos, 0.1f);

        Vector3 forward = coneInfo.coneForward;

        // Draw cone lines
        Vector3 rightBoundary = Quaternion.Euler(0, coneInfo.coneAngle, 0) * forward * coneInfo.maxDistance;
        Vector3 leftBoundary = Quaternion.Euler(0, -coneInfo.coneAngle, 0) * forward * coneInfo.maxDistance;

        Gizmos.DrawLine(pos, pos + rightBoundary);
        Gizmos.DrawLine(pos, pos + leftBoundary);

        // Draw arc
        int segments = 20;
        Vector3 lastPoint = pos + rightBoundary;
        for (int i = 1; i <= segments; i++)
        {
            float angleStep = coneInfo.coneAngle * 2 / segments;
            float angle = -coneInfo.coneAngle + angleStep * i;
            Vector3 nextPoint = pos + Quaternion.Euler(0, angle, 0) * forward * coneInfo.maxDistance;
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}