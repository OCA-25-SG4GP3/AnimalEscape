using UnityEngine;

public class PlayerInfoSystem : MonoBehaviour
{
    public static PlayerInfo[] playerInfos = new PlayerInfo[2];
    float closestDistance = 6.0f;
    float furthestDistance = 20.0f;
    float distance = -1;
    void Awake()
    {
        distance = furthestDistance;
    }
    void Start()
    {
    }
    void Update()
    {
        distance = GetTwoPlayersDistance();
    }
    public static PlayerInfo GetAny()
    {
        return GameObject.FindAnyObjectByType<PlayerInfo>();
    }
    public static PlayerInfo[] GetPlayerInfos()
    {
        return playerInfos;
    }
    public static bool Is2PlayersConnected()
    {
        return playerInfos != null
            && playerInfos.Length >= 2
            && playerInfos[0] != null
            && playerInfos[1] != null;
    }

    public float GetTwoPlayersDistance()
    {
        var infos = GetPlayerInfos();
        if (infos.Length < 2)
            return closestDistance; // only one player, return default

        PlayerInfo p1 = infos[0];
        PlayerInfo p2 = infos[1];

        if (p1 != null && p2 != null)
            return Vector3.Distance(p1.transform.position, p2.transform.position);

        return closestDistance; //テストのため、一人で遊んでいる場合は、このペナルティはいりません。
    }

    //距離が影響されます
    public float GetDistanceAffectedPlayerSpeed(float currentSpeed)
    {
        float minSpeedModifier = 1.5f; // Very close, up to 6 meter, 150% faster
        float maxSpeedModifier = 0.5f; // Very far, up to 20 meter, 50% speed

        // Clamp distance to range
        distance = Mathf.Clamp(distance, closestDistance, furthestDistance);

        // Normalize distance to 0-1
        float t = (distance - closestDistance) / (furthestDistance - closestDistance);

        // Interpolate speed modifier
        float modifier = Mathf.Lerp(minSpeedModifier, maxSpeedModifier, t);

        return currentSpeed * modifier;
    }
}
