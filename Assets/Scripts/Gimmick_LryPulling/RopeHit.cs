using UnityEngine;

public class RopeHit : MonoBehaviour
{
    public GameObject targetPlayer; // Inspector�œ���v���C���[���w��

    // �v���C���[�����ɂ��邩�ǂ���
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
        if (other.CompareTag("Player")) // "Player" �^�O�Ŕ���
        {
            playerInside = true;
           // Debug.Log($"{other.name} ��BOX�ɓ���܂���");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
           //Debug.Log($"{other.name} ��BOX����o�܂���");
        }
}
}
