using UnityEngine;

public class BlinkingLight : MonoBehaviour
{

    private Light redLight;
    public float blinkSpeed = 1f;
    public float maxIntensity = 5f;
    public float minIntensity = 0f;

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
