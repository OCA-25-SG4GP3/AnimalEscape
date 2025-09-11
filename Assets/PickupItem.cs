using UnityEngine;

public class PickupItem : MonoBehaviour
{

    public WarningLight warningLight; // 警告灯への参照をInspectorで設定する

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // アイテム削除
            Destroy(gameObject);

            // 警告灯に知らせる
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
