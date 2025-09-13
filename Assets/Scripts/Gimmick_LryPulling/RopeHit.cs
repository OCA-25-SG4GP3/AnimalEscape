using UnityEngine;

public class RopeHit : MonoBehaviour
{
    public GameObject targetPlayer; // Inspectorで特定プレイヤーを指定

    // プレイヤーが中にいるかどうか
    public bool playerInside = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

        

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // "Player" タグで判定
        {
            playerInside = true;
           // Debug.Log($"{other.name} がBOXに入りました");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
           //Debug.Log($"{other.name} がBOXから出ました");
        }
}
}
