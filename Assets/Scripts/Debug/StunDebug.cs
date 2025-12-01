using UnityEngine;

public class StunDebug : MonoBehaviour
{
    [SerializeField] private PlayerDistanceManager playerDistanceManager;
    [Header("Toggle Key")] public KeyCode toggleKey = KeyCode.Return;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(toggleKey))
        {
            //playerDistanceManager.Player1.GetComponent<>();
        }
    }
}
