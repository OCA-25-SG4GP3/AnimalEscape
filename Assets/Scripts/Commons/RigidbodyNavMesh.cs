using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyNavMesh : MonoBehaviour
{
    //Please respect the size of the baked radius
    Rigidbody rb;
    NavMeshPath navMeshPath;
    float moveSpeed = 1.5f;
    int cornerIndex = 0;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        navMeshPath = new NavMeshPath();
    }
    void Update()
    {
        MoveAlongPath();
    }
    public void Resume()
    {
        enabled = true;
    }
    public void Pause()
    {
        enabled = false;
    }
    public void ClearPath()
    {
        if (navMeshPath == null) return;
        navMeshPath.ClearCorners();
        cornerIndex = 0;
    }

    public void MoveTo(Vector3 targetPos)
    {
        NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, navMeshPath);
        cornerIndex = 0;
    }
    void MoveAlongPath()
    {
        if (navMeshPath == null || navMeshPath.corners.Length <= cornerIndex) return;

        Vector3 dir = navMeshPath.corners[cornerIndex] - transform.position;
        dir.y = 0;

        const float arriveTolerance = 0.1f;
        if (dir.magnitude < arriveTolerance)
        {
            cornerIndex++;
            return;
        }

        rb.MovePosition(rb.position + dir.normalized * moveSpeed * Time.deltaTime);
    }

    public bool HasArrived(float tolerance = 0.3f)
    {
        if (navMeshPath == null || navMeshPath.corners.Length == 0) return true; // treat no path as arrived
        Vector3 last = navMeshPath.corners[navMeshPath.corners.Length - 1];
        return Vector3.Distance(transform.position, last) <= tolerance;
    }


    public static Vector3 GetRandomFarPoint(Vector3 origin, float minDistance, float maxDistance)
    {
        Vector2 dir2D = Random.insideUnitCircle.normalized;
        Vector3 dir = new Vector3(dir2D.x, 0f, dir2D.y);
        float distance = Random.Range(minDistance, maxDistance);
        return origin + dir * distance;
    }

    public static Vector3 GetFleePointFromTarget(
        Vector3 enemyPos,
        Vector3 targetPos,
        float minRadius,
        float maxRadius,
        float maxAngleDeviationDeg = 30f,
        float navMeshSample = 6f)
    {
        Vector3 away = enemyPos - targetPos;
        away.y = 0f;

        if (away.sqrMagnitude < 0.0001f)
            away = Random.insideUnitSphere;

        away.y = 0f;
        away.Normalize();

        float angle = Random.Range(-maxAngleDeviationDeg, maxAngleDeviationDeg);
        Vector3 dir = Quaternion.Euler(0f, angle, 0f) * away;

        float r = Random.Range(minRadius, maxRadius);
        Vector3 p = enemyPos + dir * r;

        if (NavMesh.SamplePosition(p, out var hit, navMeshSample, NavMesh.AllAreas))
            return hit.position;

        return enemyPos; // fallback
    }
    public Vector3 GetNextDirection()
    {
        if (navMeshPath == null || cornerIndex >= navMeshPath.corners.Length)
            return Vector3.zero;

        Vector3 dir = navMeshPath.corners[cornerIndex] - rb.position;
        dir.y = 0f;
        return dir.normalized;
    }
    void OnDrawGizmos()
    {
        if (navMeshPath == null || navMeshPath.corners.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
        {
            Gizmos.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
            Gizmos.DrawSphere(navMeshPath.corners[i], 0.1f);
        }
        Gizmos.DrawSphere(navMeshPath.corners[navMeshPath.corners.Length - 1], 0.1f);
    }

}
