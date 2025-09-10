using UnityEngine;

public class BalloonManager : MonoBehaviour
{
    [SerializeField] private int        _balloonMax;            // ���D�̍ő吔
    [SerializeField] private GameObject _balloonPrefab;        // ���D�v���n�u
    [SerializeField] private GameObject _keyPrefab;            // ���v���n�u

    [Header("�X�|�[���͈�")]
    [SerializeField] private Vector3 _spawnMin;   // �X�|�[���͈͂̍ŏ��l
    [SerializeField] private Vector3 _spawnMax;   // �X�|�[���͈͂̍ő�l

    void Start()
    {
        // �����_���œ����蕗�D�̃C���f�b�N�X�����߂�
        int luckyIndex = Random.Range(0, _balloonMax);

        for (int i = 0; i < _balloonMax; i++)
        {
            // �����_�����W������
            Vector3 pos = new Vector3(
                Random.Range(_spawnMin.x, _spawnMax.x),
                Random.Range(_spawnMin.y, _spawnMax.y),
                Random.Range(_spawnMin.z, _spawnMax.z)
            );

            // ���D�𐶐�
            GameObject balloon = Instantiate(_balloonPrefab, pos, Quaternion.identity);

            // Balloon �X�N���v�g���擾���ď�����
            Balloon balloonScript = balloon.GetComponent<Balloon>();


            // �I�΂ꂽ�����̕��D�𓖂���ɐݒ�
            if (i == luckyIndex)
            { 
                balloonScript.SetAsLucky(_keyPrefab);
            }
        }
    }
}
