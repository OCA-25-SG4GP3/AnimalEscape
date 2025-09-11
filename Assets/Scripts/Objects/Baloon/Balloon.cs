using UnityEngine;

public class Balloon : MonoBehaviour
{
    private bool _isLucky = false;       // �����蕗�D���ǂ���
    private GameObject _keyPrefab;       // ���v���n�u

    // BalloonManager ���瓖�����ݒ肷��
    public void SetAsLucky(GameObject key)
    {
        _isLucky = true;
        _keyPrefab = key;
    }

    // ���̃I�u�W�F�N�g�i�Ⴆ��Shot�j�ƂԂ������Ƃ��Ă΂��
    void OnCollisionEnter(Collision collision)
    {
        // �Ԃ��������肪 "Shot" �^�O�������Ă�����
        if (collision.gameObject.CompareTag("Shot"))
        {
            DestroyByPlayerShot(); //���Ƃ���Ă΂��\��������̂ŁA�֐����ɂ��܂����iZan�j
        }
    }

    public void DestroyByPlayerShot()
    {
        // �����蕗�D�Ȃ献�𐶐�����
        if (_isLucky && _keyPrefab != null)
        {
            SpawnKey(); //���Ƃ���Ă΂��\��������̂ŁA�֐����ɂ��܂����iZan�j
        }

        // �����i���D�j���폜����
        Destroy(gameObject);
    }

    public GameObject SpawnKey()
    {
        return Instantiate(_keyPrefab, transform.position, Quaternion.identity);
    }
}
