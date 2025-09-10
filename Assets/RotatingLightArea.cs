using UnityEngine;

public class RotatingLightArea : MonoBehaviour
{

    private Light redLight;// ��]�̃��C�g
    private AudioSource audioSource;// ����炷���߂�AudioSource
    public AudioClip alarmSound; // �A���[�������i�[����ϐ�
    public GameObject item; // �A�C�e���̃I�u�W�F�N�g

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ��]���̃��C�g�R���|�[�l���g���擾
        redLight = GetComponentInChildren<Light>();

        // ����炷���߂�AudioSource���擾
        audioSource = GetComponent<AudioSource>();

        // �ŏ��͌��������Ă���
        redLight.enabled = false;  // ���𖳌���
        // �܂���
        // redLight.intensity = 0f;  // ���̋��x���[���ɐݒ�
       }

        // �v���C���[����]���̃G���A�ɓ������Ƃ�
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �v���C���[���������ꍇ
        {
            // ��]���̌����ēx�L���ɂ���
            redLight.enabled = true;  // ����L����
            // �܂���
            // redLight.intensity = 5f;  // ������������

            // �A���[������炷
            audioSource.PlayOneShot(alarmSound);

            Debug.Log("�v���C���[���G���A�ɓ������I");
        }
    }

    // �A�C�e�����E�����Ƃ�
    private void OnTriggerEnterItem(Collider other)
    {
        if (other.CompareTag("Item")) // �A�C�e�����G�ꂽ�ꍇ
        {
            // �A�C�e���������i��\���ɂ���j
            Destroy(other.gameObject);

            // ��]��������
            redLight.enabled = false;  // ���𖳌���

            Debug.Log("�A�C�e�����E�����̂ŉ�]���������܂���");
        }
    }

    // �v���C���[����]���̃G���A���o���Ƃ�
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // �v���C���[�̃^�O���`�F�b�N
        {

            // �Ԃ�����L����
            redLight.enabled = false;
            // �Ԃ������ĊJ
            redLight.intensity = 5f; // ���̌��̋��x�ɖ߂�
            Debug.Log("�v���C���[���G���A����o���I");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
