using Unity.Cinemachine;
using UnityEngine;

public class PlayerDistanceManager : MonoBehaviour
{
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;
    [SerializeField] private float _maxDistance = 15f;

    private void Start()
    {
        if (_player1 == null || _player2 == null)
        {
            var target = GetComponent<CinemachineTargetGroup>();
            _player1 = target.Targets[0].Object;
            _player2 = target.Targets[1].Object;
        }
    }

    private void LateUpdate()
    {
        Vector3 dir = _player2.position - _player1.position;
        float distance = dir.magnitude;

        if (distance > _maxDistance)
        {
            Vector3 midpoint = (_player1.position + _player2.position) / 2f;
            dir.Normalize();
            
            // �v���C���[���m�̋������ő勗���𒴂����ꍇ�A2�l�̈ʒu���ő勗�����Ɏ��߂�悤��������
            _player1.position = midpoint - dir * _maxDistance / 2f;
            _player2.position = midpoint + dir * _maxDistance / 2f;
        }
    }
}
