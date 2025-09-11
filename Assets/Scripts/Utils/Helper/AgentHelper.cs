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

}
