using UnityEngine;

public class RotatingLight : MonoBehaviour
{

    [Header("���C�g�ݒ�")]
    public Light targetLight;          // �_�ł����郉�C�g
    public float rotationSpeed = 100f; // ��]���x
    public float blinkInterval = 0.5f; // �_�ŊԊu�i�b�j

    [Header("�T�E���h�ݒ�")]
    public AudioSource audioSource;
    public AudioClip alarmSound;

    private bool isActive = false;     // ��]���_�Œ����ǂ���
    private float blinkTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (targetLight != null)
        {
            targetLight.enabled = false; // �ŏ��͏���
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        // �e���Ɖ�]����̂ŃJ�o�[�{���C�g���ꏏ�ɉ��
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // �_�ŏ���
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            targetLight.enabled = !targetLight.enabled;
            blinkTimer = 0f;
        }
    }

    // �v���C���[���G���A�ɓ�������J�n
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartLight();

            if (audioSource != null && alarmSound != null)
            {
                audioSource.PlayOneShot(alarmSound);
            }

            Debug.Log("�v���C���[���G���A�ɓ����� �� ��]��ON");
        }
    }

    // �v���C���[���G���A���o�����~
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopLight();
            Debug.Log("�v���C���[���G���A����o�� �� ��]��OFF");
        }
    }

    // ��]���J�n
    public void StartLight()
    {
        isActive = true;
        if (targetLight != null) targetLight.enabled = true;
        blinkTimer = 0f;
    }

    // ��]����~
    public void StopLight()
    {
        isActive = false;
        if (targetLight != null) targetLight.enabled = false;
    }

}
