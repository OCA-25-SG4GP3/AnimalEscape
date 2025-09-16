using UnityEngine;

public class RotatingLightArea : MonoBehaviour
{

    private Light redLight;// 回転のライト
    private AudioSource audioSource;// 音を鳴らすためのAudioSource
    public AudioClip alarmSound; // アラーム音を格納する変数
    public GameObject item; // アイテムのオブジェクト

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 回転灯のライトコンポーネントを取得
        redLight = GetComponentInChildren<Light>();

        // 音を鳴らすためのAudioSourceを取得
        audioSource = GetComponent<AudioSource>();

        // 最初は光を消しておく
        redLight.enabled = false;  // 光を無効化
        // または
        // redLight.intensity = 0f;  // 光の強度をゼロに設定
       }

        // プレイヤーが回転灯のエリアに入ったとき
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーが入った場合
        {
            // 回転灯の光を再度有効にする
            redLight.enabled = true;  // 光を有効化
            // または
            // redLight.intensity = 5f;  // 光を強くする

            // アラーム音を鳴らす
            audioSource.PlayOneShot(alarmSound);

            Debug.Log("プレイヤーがエリアに入った！");
        }
    }

    // アイテムを拾ったとき
    private void OnTriggerEnterItem(Collider other)
    {
        if (other.CompareTag("Item")) // アイテムが触れた場合
        {
            // アイテムを消す（非表示にする）
            Destroy(other.gameObject);

            // 回転灯を消す
            redLight.enabled = false;  // 光を無効化

            Debug.Log("アイテムを拾ったので回転灯が消えました");
        }
    }

    // プレイヤーが回転灯のエリアを出たとき
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーのタグをチェック
        {

            // 赤い光を有効化
            redLight.enabled = false;
            // 赤い光を再開
            redLight.intensity = 5f; // 元の光の強度に戻す
            Debug.Log("プレイヤーがエリアから出た！");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
