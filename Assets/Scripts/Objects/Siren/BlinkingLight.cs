using UnityEngine;

public class BlinkingLight : MonoBehaviour
{

    private Light redLight; // ���C�g�R���|�[�l���g
    public float blinkSpeed = 1f; // �_�ő��x�i1�b��1��_�Łj
    public float maxIntensity = 5f; // �ő�̌��̋��x
    public float minIntensity = 0f; // �ŏ��̌��̋��x

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ���C�g���擾
        redLight = GetComponent<Light>();
        redLight.color = Color.red; // ���C�g�̐F��Ԃɐݒ�
    }

    // Update is called once per frame
    void Update()
    {
        // ���Ԍo�߂Ɋ�Â��Č��̋��x��ω�������
        redLight.intensity = Mathf.PingPong(Time.time * blinkSpeed, maxIntensity - minIntensity) + minIntensity;
    }
}
