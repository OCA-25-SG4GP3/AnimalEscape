using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public WarningLight warningLight; // �x�����ւ̎Q�Ƃ�Inspector�Őݒ肷��

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �A�C�e���폜
            Destroy(gameObject);

            // �x�����ɒm�点��
            if (warningLight != null)
            {
                warningLight.ClearAlarm();
            }

            Debug.Log("�A�C�e�����E���� �� �x�����");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
