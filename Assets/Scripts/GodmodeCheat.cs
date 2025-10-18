using UnityEngine;

public class GodmodeCheat : MonoBehaviour
{
    [Header("Toggle Key")]
    public KeyCode toggleKey = KeyCode.P;

    [Header("Movement Speed")]
    public float speed = 20f;

    private bool godmode = false;
    private Rigidbody[] playerRBs;
    private Collider[] playerCols;
    private Transform[] players;

    private void Start()
    {
        var infos = PlayerInfoSystem.GetPlayerInfos();
        int len = infos.Length;

        players = new Transform[len];
        playerRBs = new Rigidbody[len];
        playerCols = new Collider[len];

        for (int i = 0; i < len; i++)
        {
            var go = infos[i].gameObject;
            players[i] = go.transform;
            playerRBs[i] = go.GetComponent<Rigidbody>();
            playerCols[i] = go.GetComponent<Collider>();
        }
    }

    private void Update()
    {
        // Toggle Godmode
        if (Input.GetKeyDown(toggleKey))
        {
            godmode = !godmode;
            for (int i = 0; i < playerRBs.Length; i++)
            {
                if (!playerRBs[i]) continue;
                playerRBs[i].useGravity = !godmode;
                playerRBs[i].isKinematic = godmode;
                playerCols[i].enabled = !godmode;
            }
        }

        if (godmode)
            HandleMovement();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0;

        for (int i = 0; i < players.Length; i++)
        {
            players[i].position += move * speed * Time.deltaTime;
        }
    }
}
