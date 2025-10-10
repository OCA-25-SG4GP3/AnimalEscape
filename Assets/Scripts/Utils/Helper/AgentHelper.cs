using UnityEngine;
using UnityEngine.AI;

public static class AgentHelper
{
    public static void Resume(NavMeshAgent agent)
    {
        agent.isStopped = false;
    }
    public static void Pause(NavMeshAgent agent)
    {
        agent.isStopped = true;
    }
    public static void ClearPath(NavMeshAgent agent)
    {
        agent.ResetPath(); // clears destination, stop moving to that pos
    }

    public static void MoveTo(NavMeshAgent agent, Vector3 destination)
    {
        agent.destination = destination;
    }
    public static bool HasArrivedSuccess(NavMeshAgent agent, float tolerance = 0.1f)
    {
        if (agent.pathPending) return false; // still calculating

        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            return false; // invalid or partial path

        // Arrival check with tolerance
        return agent.remainingDistance <= (agent.stoppingDistance + tolerance);
    }

    static public Vector3 GetRandomFarPoint(Vector3 origin, float minDistance, float maxDistance)
    {
        // Random direction on XZ plane
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 dir = new Vector3(dir2D.x, 0f, dir2D.y);

        // Random distance between min and max
        float distance = Random.Range(minDistance, maxDistance);

        // Final point
        return origin + dir * distance;
    }

    public static Vector3 GetFleePointFromTarget(
        Vector3 enemyPos,
        Vector3 targetPos,
        float minRadius,
        float maxRadius,
        float maxAngleDeviationDeg = 30f,
        float navMeshSample = 6f) //Run against target direction, but with tolerance of cone
    {
        // base direction: away from target on XZ
        Vector3 away = enemyPos - targetPos;
        away.y = 0f;

        if (away.sqrMagnitude < 0.0001f)
            away = Random.insideUnitSphere; // fallback
        away.y = 0f;
        away.Normalize();

        // random cone around "away" direction
        float angle = Random.Range(-maxAngleDeviationDeg, maxAngleDeviationDeg);
        Vector3 dir = Quaternion.Euler(0f, angle, 0f) * away;

        float r = Random.Range(minRadius, maxRadius);
        Vector3 p = enemyPos + dir * r;

        // optional: project to NavMesh (recommended)
        if (NavMesh.SamplePosition(p, out var hit, navMeshSample, NavMesh.AllAreas))
            p = hit.position;
        else
            p.y = enemyPos.y; // flat fallback

        return p;
    }

}

