using UnityEngine;
using UnityEngine.AI;

public static class AgentHelper
{
    public static void Resume(NavMeshAgent agent)
    {
        agent.isStopped = false;
    }
    public static void Stop(NavMeshAgent agent)
    {
        agent.isStopped = true;
    }
    public static void StopAndClear(NavMeshAgent agent)
    {
        agent.isStopped = true;
        agent.ResetPath(); // clears destination
    }

    public static void MoveTo(NavMeshAgent agent, Vector3 destination)
    {
        agent.destination = destination;
    }
    public static bool HasArrivedSuccess(NavMeshAgent agent)
    {
        if (!agent.pathPending) //path has finished calculating (or never requested)
        {
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                return agent.remainingDistance <= agent.stoppingDistance && !agent.hasPath;
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("Does not found path!");
#endif
                return false;
            }
        }

        return false;
    }
}
