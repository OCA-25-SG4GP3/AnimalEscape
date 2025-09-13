using UnityEngine;

public class WarningLight : MonoBehaviour
{

    [Header("回転設定")]
    public float rotationSpeed = 100f; // 親ごと回転させる速度

    [Header("ライト設定")]
    public Light spotLight;             // 回転させたい Spot Light
    public Light pointLight;           // 点滅させたい Point Light
    public float blinkInterval = 0.5f; // SpotLight の点滅間隔（秒）

    [Header("サウンド設定")]
    public AudioSource audioSource;
    public AudioClip alarmSound;

    [SerializeField] private VoidEventSO _onCageOpenEvent;
    [SerializeField] private VoidEventSO _onAlertTriggerEvent;
    [SerializeField] private VoidEventSO _onGimmickClearEvent;
    [SerializeField] private TransformEventSO _onEnemyTriggerEvent;


    private float blinkTimer = 0f;
    [SerializeField] private bool isActive = false; // 常時動作させるなら true
    [SerializeField] private bool alarmCleared = false;   // アイテムを取って警報解除されたか

    private void OnEnable()
    {
        _onCageOpenEvent.OnEventInvoked += TurnOff;
    }

    private void OnDisable()
    {
        _onCageOpenEvent.OnEventInvoked -= TurnOff;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 最初は両方消灯
        if (pointLight != null) pointLight.enabled = false;
        if (spotLight != null) spotLight.enabled = false;

        if (audioSource != null)
        {
            audioSource.loop = true;      // ループ再生を有効化
            audioSource.playOnAwake = false; // 勝手に再生しないように
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        // spotLight だけ回転
        if (spotLight != null)
        {
            spotLight.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
        // pointLight の点滅
        if (pointLight != null)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInterval)
            {
                pointLight.enabled = !pointLight.enabled;
                blinkTimer = 0f;
            }
        }
    }

    // プレイヤーが部屋に入ったとき
    private void OnTriggerEnter(Collider other)
    {
        if (alarmCleared) return; //警報解除済みなら無視
        if (other.CompareTag("Player"))
        {
            _onAlertTriggerEvent.InvokeEvent();
            _onEnemyTriggerEvent.InvokeEvent(transform);
            TurnOn();
        }
    }

    // プレイヤーが部屋から出たとき
    private void OnTriggerExit(Collider other)
    {
        //if (alarmCleared) return; //警報解除済みなら無視
        //                          // if (other.CompareTag("Player"))
        //                          // {
        //                          //     TurnOff();
        //                          // }
        if (other.CompareTag("Player"))
        {
            if (!isActive && !alarmCleared)
        {
            _onGimmickClearEvent.InvokeEvent();
            alarmCleared = true;
        }
        }
    }

    // 外部から呼び出す（アイテム側から通知）
    public void ClearAlarm()
    {
        alarmCleared = true; //永久解除フラグON
        TurnOff();
        Debug.Log("アイテムを取った → 警報解除");
    }

    // 警告灯ON
    public void TurnOn()
    {
        if (alarmCleared) return; // 念のためチェック
        isActive = true;
        if (pointLight != null) pointLight.enabled = true;
        if (spotLight != null) spotLight.enabled = true;
        blinkTimer = 0f;

        if (audioSource != null && alarmSound != null)
        {
            audioSource.clip = alarmSound;
            if (!audioSource.isPlaying) audioSource.Play();
        }
        Debug.Log("⚠ プレイヤーが部屋に入った → 警告灯ON");
    }

    // 警告灯OFF
    public void TurnOff()
    {
        isActive = false;
        if (pointLight != null) pointLight.enabled = false;
        if (spotLight != null) spotLight.enabled = false;

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        Debug.Log("✅ プレイヤーが部屋から出た → 警告灯OFF");
    }

    // 外部からON/OFF制御したい場合
    public void StartLight()
    {
        isActive = true;
        if (spotLight != null) spotLight.enabled = true;
        blinkTimer = 0f;

        if (audioSource != null && alarmSound != null)
            audioSource.PlayOneShot(alarmSound);
    }

    public void StopLight()
    {
        isActive = false;
        if (spotLight != null) spotLight.enabled = false;
    }

}
