using UnityEngine;

public class BlinkingLight : MonoBehaviour
{

    private Light redLight; // ライトコンポーネント
    public float blinkSpeed = 1f; // 点滅速度（1秒で1回点滅）
    public float maxIntensity = 5f; // 最大の光の強度
    public float minIntensity = 0f; // 最小の光の強度

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ライトを取得
        redLight = GetComponent<Light>();
        redLight.color = Color.red; // ライトの色を赤に設定
    }

    // Update is called once per frame
    void Update()
    {
        // 時間経過に基づいて光の強度を変化させる
        redLight.intensity = Mathf.PingPong(Time.time * blinkSpeed, maxIntensity - minIntensity) + minIntensity;
    }
}
