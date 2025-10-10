using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] public bool hasCaught = false;
    public static PlayerInfo GetAny()
    {
        return GameObject.FindAnyObjectByType<PlayerInfo>();
    }
}
