using Unity.Cinemachine;
using UnityEngine;

public class PlayerDistanceManager : MonoBehaviour
{
    [SerializeField] Transform _player1; public Transform Player1 => _player1;
    [SerializeField] Transform _player2; public Transform Player2 => _player2;
    [SerializeField] float _maxDistance = 15f;

    void Start()
    {
        var group = GetComponent<CinemachineTargetGroup>();
        if (group != null)
        {
            if (_player1 == null && group.Targets.Count > 0)
                _player1 = group.Targets[0].Object;
            if (_player2 == null && group.Targets.Count > 1)
                _player2 = group.Targets[1].Object;
        }
    }

    void LateUpdate()
    {
        // If only one player ¨ do nothing
        if (_player1 == null || _player2 == null) return;

        Vector3 dir = _player2.position - _player1.position;
        float distance = dir.magnitude;

        if (distance > _maxDistance)
        {
            Vector3 midpoint = (_player1.position + _player2.position) / 2f;
            dir.Normalize();

            _player1.position = midpoint - dir * _maxDistance / 2f;
            _player2.position = midpoint + dir * _maxDistance / 2f;
        }
    }
}
