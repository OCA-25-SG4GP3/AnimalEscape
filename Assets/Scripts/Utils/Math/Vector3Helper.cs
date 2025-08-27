using System.Collections.Generic;
using UnityEngine;

public static class Vector3Helper
{
    public static Vector3 GetClosest(Vector3 origin, IList<Vector3> positions, out int closestIndex)
    {
        closestIndex = -1;
        if (positions == null || positions.Count == 0)
            return Vector3.zero;

        float closestDist = float.MaxValue;
        Vector3 closestPos = Vector3.zero;

        for (int i = 0; i < positions.Count; i++)
        {
            float dist = Vector3.SqrMagnitude(positions[i] - origin);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPos = positions[i];
                closestIndex = i;
            }
        }

        return closestPos;
    }
}
