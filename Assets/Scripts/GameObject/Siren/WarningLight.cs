using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class WarningLight : MonoBehaviour
{

    [Header("回転設定")]
    public float rotationSpeed = 100f; // 親ごと回転させる速度

    [Header("ライト設定")]
    public Light _warningSpotLight;             // 回転させたい Spot Light
    public Light _warningPointLight;           // 点滅させたい Point Light
    public Light _spotLight;
    public float blinkInterval = 0.5f; // SpotLight の点滅間隔（秒）

    [Header("サウンド設定")]
    public AudioSource audioSource;
    public AudioClip alarmSound;

    [SerializeField] private IntEventSO _onCageOpenEvent;
    [SerializeField] private VoidEventSO _onAlertTriggerEvent;
    [SerializeField] private VoidEventSO _onGimmickClearEvent;
    [SerializeField] private TransformEventSO _onEnemyTriggerEvent;
    [SerializeField] private AudioClipEventSO _playBGMEvent;
    [SerializeField] private AudioClipEventSO _playSFXEvent;
    [SerializeField] private VoidEventSO _stopSFXEvent;
    [SerializeField] private AudioClip _gimmickBGM;


    private float blinkTimer = 0f;
    [SerializeField] private bool isActive = false; // 常時動作させるなら true
    [SerializeField] private bool alarmCleared = false;   // アイテムを取って警報解除されたか
    [SerializeField] private int _gimmickID = 0;
    [SerializeField] private GameObject _animal;

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
        if (_warningSpotLight != null) _warningSpotLight.enabled = false;
        if (_warningPointLight != null) _warningPointLight.enabled = false;
        _spotLight.enabled = false;
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
        if (_warningSpotLight != null)
        {
            _warningSpotLight.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
        }
        // pointLight の点滅
        if (_warningPointLight != null)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= blinkInterval)
            {
                _warningPointLight.enabled = !_warningPointLight.enabled;
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
            // _onGimmickClearEvent.InvokeEvent();
            // alarmCleared = true;
        }
        }
    }

    // 外部から呼び出す（アイテム側から通知）
    // public void ClearAlarm()
    // {
    //     alarmCleared = true; //永久解除フラグON
    //     TurnOff();
    //     Debug.Log("アイテムを取った → 警報解除");
    // }

    // 警告灯ON
    public void TurnOn()
    {
        if (alarmCleared) return; // 念のためチェック
        isActive = true;
        if (_warningPointLight != null) _warningPointLight.enabled = true;
        if (_warningSpotLight != null) _warningSpotLight.enabled = true;
        _spotLight.enabled = true;
        
        blinkTimer = 0f;

        if (audioSource != null && alarmSound != null)
        {
            _playSFXEvent.InvokeEvent(alarmSound, true);
            _playBGMEvent.InvokeEvent(_gimmickBGM, true);
            // audioSource.clip = alarmSound;
            // if (!audioSource.isPlaying) audioSource.Play();
        }
        Debug.Log("⚠ プレイヤーが部屋に入った → 警告灯ON");
    }

    // 警告灯OFF
    public void TurnOff(int id)
    {
        isActive = false;
        if (_warningPointLight != null) _warningPointLight.enabled = false;
        if (_warningSpotLight != null) _warningSpotLight.enabled = false;
        _spotLight.enabled = false;
        // if (audioSource != null && audioSource.isPlaying)
            // audioSource.Stop();
        _stopSFXEvent.InvokeEvent();
        if (id != _gimmickID) return; // 違うギミックの解除通知なら無視
        _animal.GetComponent<EscapeAnimalAction>().IsFree = true;
        _onGimmickClearEvent.InvokeEvent();
        alarmCleared = true;
        Debug.Log("✅ プレイヤーが部屋から出た → 警告灯OFF");
    }

    // 外部からON/OFF制御したい場合
    public void StartLight()
    {
        isActive = true;
        if (_warningSpotLight != null) _warningSpotLight.enabled = true;
        blinkTimer = 0f;

        if (audioSource != null && alarmSound != null)
            audioSource.PlayOneShot(alarmSound);
    }

    public void StopLight()
    {
        isActive = false;
        if (_warningSpotLight != null) _warningSpotLight.enabled = false;
    }

}
