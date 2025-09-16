using UnityEngine;

public class RotatingLight : MonoBehaviour
{

    [Header("ライト設定")]
    public Light targetLight;          // 点滅させるライト
    public float rotationSpeed = 100f; // 回転速度
    public float blinkInterval = 0.5f; // 点滅間隔（秒）

    [Header("サウンド設定")]
    public AudioSource audioSource;
    public AudioClip alarmSound;

    private bool isActive = false;     // 回転＆点滅中かどうか
    private float blinkTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (targetLight != null)
        {
            targetLight.enabled = false; // 最初は消灯
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        // 親ごと回転するのでカバー＋ライトが一緒に回る
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // 点滅処理
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            targetLight.enabled = !targetLight.enabled;
            blinkTimer = 0f;
        }
    }

    // プレイヤーがエリアに入ったら開始
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartLight();

            if (audioSource != null && alarmSound != null)
            {
                audioSource.PlayOneShot(alarmSound);
            }

            Debug.Log("プレイヤーがエリアに入った → 回転灯ON");
        }
    }

    // プレイヤーがエリアを出たら停止
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopLight();
            Debug.Log("プレイヤーがエリアから出た → 回転灯OFF");
        }
    }

    // 回転灯開始
    public void StartLight()
    {
        isActive = true;
        if (targetLight != null) targetLight.enabled = true;
        blinkTimer = 0f;
    }

    // 回転灯停止
    public void StopLight()
    {
        isActive = false;
        if (targetLight != null) targetLight.enabled = false;
    }

}
